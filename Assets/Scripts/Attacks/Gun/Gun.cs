using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class GunFiringMode
{
    public string name = "Placeholder";
    public string description = "Placeholder, insert description here";

    [Header("Stats")]
    public GunGeneralStats general;
    public GunFireControlStats fireControls;
    public GunMagazineStats magazine;
    public GunOpticsStats optics;

    [Header("Other")]
    public Sprite hudIcon;
    public float switchSpeed;
}

public class Gun : MonoBehaviour
{
    public string description = "Placeholder, insert description here";
    [Header("Firing modes and stats")]
    public GunFiringMode[] firingModes;
    public int firingModeIndex;

    [Header("Switching")]
    public float switchSpeed;
    public Transform holsterPosition;

    [Header("Cosmetics")]
    public Animator animator;
    public Transform weaponModel;
    public AudioSource weaponSoundSource;

    [Header("Other")]
    public float attackMessageLimitDelay = 1;

    #region Additional variables for weapon function
    GunGeneralStats general;
    GunFireControlStats fireControls;
    GunMagazineStats magazine;
    GunOpticsStats optics;

    // Additional optics stats
    [HideInInspector] public bool isAiming;
    [HideInInspector] public float zoomVariable;
    [HideInInspector] public float zoomTimer;
    [HideInInspector] public PercentageModifier sensitivityWhileAiming;
    [HideInInspector] public PercentageModifier speedWhileAiming;
    float adsTimer;

    // Additional recoil stats
    [HideInInspector] public float recoilToApply;
    [HideInInspector] public Vector2 aimOrigin;
    [HideInInspector] public Vector2 aimCurrent;

    // Additional magazine stats
    [HideInInspector] public bool isReloading;
    [HideInInspector] public float reloadTimer;
    #endregion

    #region Universal variables
    [HideInInspector] public WeaponHandler playerHolding;

    // Switching modes
    [HideInInspector] public bool isSwitchingWeapon;
    [HideInInspector] public bool isSwitchingFireMode;

    // Moving weapon model
    bool isAnimating = false;

    float attackMessageLimitTimer = float.MaxValue;
    #endregion

    
    private void OnValidate()
    {
        if (firingModes.Length > 0)
        {
            foreach (GunFiringMode fm in firingModes)
            {
                bool generalStatsMissing = fm.general == null;
                bool fireControlsMissing = fm.fireControls == null;

                if (generalStatsMissing || fireControlsMissing)
                {
                    string error = "Error: ";

                    if (generalStatsMissing)
                    {
                        error += "general ";
                        if (fireControlsMissing)
                        {
                            error += "and ";
                        }
                    }

                    if (fireControlsMissing)
                    {
                        error += "fire control ";
                    }

                    error += "stats are missing from " + fm.name + "!";
                    Debug.LogError(error);
                }
            }
        }
        else
        {
            Debug.LogError("No firing modes present!");
        }
    }

    private void Start()
    {
        playerHolding = GetComponentInParent<WeaponHandler>();

        sensitivityWhileAiming = new PercentageModifier(0, true, false);
        playerHolding.ph.pc.sensitivityModifier.Add(sensitivityWhileAiming, this);
        speedWhileAiming = new PercentageModifier();
        playerHolding.ph.pc.movementSpeed.Add(speedWhileAiming, this);

        AssignFiringModes(firingModeIndex);
    }


    void Update()
    {
        AssignFiringModes(firingModeIndex);

        attackMessageLimitTimer += Time.deltaTime;

        // If the player is not switching weapons or fire modes, and is therefore able to operate the weapon
        if (isSwitchingWeapon == false && isSwitchingFireMode == false && playerHolding.weaponSelector.MenuIsActive() == false)
        {

            #region Firing mode controls
            if (Input.GetAxis("Mouse ScrollWheel") != 0 && firingModes.Length > 1) // Switch firing modes with the scroll wheel
            {
                int i = 0;
                if (Input.GetAxis("Mouse ScrollWheel") < 0)
                {
                    i = 1;
                }
                else if (Input.GetAxis("Mouse ScrollWheel") > 0)
                {
                    i = -1;
                }

                i += firingModeIndex;

                i = (int)Misc.InverseClamp(i, 0, firingModes.Length - 1);
                /*
                if (i > firingModes.Length - 1)
                {
                    i = 0;
                }
                else if (i < 0)
                {
                    i = firingModes.Length - 1;
                }
                */
                
                StartCoroutine(SwitchMode(i));
            }
            #endregion

            fireControls.fireTimer += Time.deltaTime;

            // If player is active
            // If player is pressing fire button
            // If fireTimer has finished
            // If burst count has not exceeded the limit OR there is no burst limit
            // If ammo is available OR not required
            // If magazine is not empty OR null
            if (playerHolding.ph.PlayerState() == GameState.Active
                && Input.GetButton("Fire")
                && fireControls.fireTimer >= 60 / fireControls.roundsPerMinute
                && (fireControls.burstCounter < fireControls.maxBurst || fireControls.maxBurst <= 0)
                && (general.consumesAmmo == false || (playerHolding.ph.a.GetStock(general.ammoType) >= general.ammoPerShot))
                && (magazine == null || (magazine.data.current >= general.ammoPerShot && isReloading == false)))
            {
                #region Adjust fire control variables
                fireControls.fireTimer = 0; // Reset fire timer to count up to next shot
                fireControls.burstCounter += 1;
                #endregion

                #region Consume ammo if supply is present
                if (general.consumesAmmo == true)
                {
                    playerHolding.ph.a.Spend(general.ammoType, general.ammoPerShot);
                }
                #endregion

                #region Deplete magazine if present
                if (magazine != null)
                {
                    magazine.data.current -= general.ammoPerShot;
                }
                #endregion

                #region Apply recoil
                recoilToApply += general.recoil;
                #endregion

                #region Calculate and fire shot
                Transform head = playerHolding.ph.pc.head;
                Vector3 aimDirection = head.forward;
                if (optics == null || isAiming == false)
                {
                    float accuracy = playerHolding.standingAccuracy.Calculate();
                    aimDirection = Quaternion.Euler(Random.Range(-accuracy, accuracy), Random.Range(-accuracy, accuracy), Random.Range(-accuracy, accuracy)) * aimDirection;
                }

                Damage.ShootProjectile(general.projectilePrefab, general.projectileCount, general.projectileSpread, general.range, playerHolding.ph, head.position, aimDirection, head.up, general.muzzle.position);
                #endregion

                #region Play firing animations
                //animator.SetTrigger("Fire_" + firingModes[firingModeIndex].name);
                general.effectsOnFire.Invoke();
                #endregion

                #region Send attack message if the timer is right
                if (attackMessageLimitTimer >= attackMessageLimitDelay) // Sends attack message
                {
                    AttackMessage am = AttackMessage.Ranged(playerHolding.ph, transform.position, transform.forward, general.range, general.projectilePrefab.diameter, playerHolding.standingAccuracy.Calculate() + general.projectileSpread, general.projectilePrefab.velocity, general.projectilePrefab.hitDetection);
                    EventObserver.TransmitAttack(am);
                    attackMessageLimitTimer = 0;
                }
                #endregion
            }
            else if (!Input.GetButton("Fire"))
            {
                fireControls.burstCounter = 0;
            }

            #region ADS code
            ADSHandler(optics);
            /*
            if (optics != null)
            {
                AimHandler(optics, general.heldPosition, playerHolding.toggleAim);
            }
            */
            #endregion

            #region Reloading code
            if (magazine != null)
            {
                if (general.consumesAmmo == true)
                {
                    ReloadHandler(magazine.reloadTime, fireControls.fireTimer, fireControls.roundsPerMinute, magazine.roundsReloadedPerCycle, magazine.data, playerHolding, general.ammoType);
                }
                else
                {
                    ReloadHandler(magazine.reloadTime, fireControls.fireTimer, fireControls.roundsPerMinute, magazine.roundsReloadedPerCycle, magazine.data);
                }
            }
            #endregion
        }

        RecoilHandler(general.recoilApplyRate, playerHolding);
    }

    #region Switching functions
    public IEnumerator SwitchMode(int index)
    {
        // Checks if the firing mode is actually changing, otherwise code is not unnecessarily run
        if (index == firingModeIndex)
        {
            yield break;
        }

        isSwitchingFireMode = true;

        // Check if the weapon being switched to has different optics, and cancel out if so.
        GunFiringMode newMode = firingModes[index];
        if (optics != null && (newMode.optics == null || newMode.optics != optics))
        {
            /*
            isAiming = false;
            zoomTimer = 0;
            LerpSights(optics, 0, newMode.general.heldPosition);
            */
            CancelADS();
        }

        if (newMode.optics != null)
        {
            sensitivityWhileAiming.percentageValue = 100 / newMode.optics.magnification;
            speedWhileAiming.percentageValue = newMode.optics.moveSpeedReductionPercentage;
        }

        if (magazine != null)
        {
            CancelReload();
            /*
            reloadTimer = 0;
            isReloading = false;
            print("Reload sequence cancelled");
            */
        }

        yield return new WaitForSeconds(newMode.switchSpeed);

        firingModeIndex = index;

        isSwitchingFireMode = false;
    }

    public IEnumerator Draw()
    {
        isSwitchingWeapon = true;

        AssignFiringModes(firingModeIndex);

        gameObject.SetActive(true);
        // Draw weapon animation
        weaponModel.transform.SetPositionAndRotation(holsterPosition.position, holsterPosition.rotation);

        Transform newMoveTransform = general.heldPosition;
        Debug.Log(newMoveTransform);
        StartCoroutine(MoveWeaponModel(newMoveTransform.localPosition, newMoveTransform.localRotation, switchSpeed));
        yield return new WaitForSeconds(switchSpeed);
        isSwitchingWeapon = false;
    }

    public IEnumerator Holster()
    {
        isSwitchingWeapon = true;

        #region Cancel running weapon functions
        if (optics != null) // DOES NOT WORK PROPERLY
        {
            CancelADS();
            /*
            isAiming = false;
            zoomTimer = 0;
            LerpSights(optics, 0, general.heldPosition);
            */
        }

        if (magazine != null)
        {
            CancelReload();
        }
        #endregion

        // Begin holster animation
        weaponModel.transform.SetPositionAndRotation(general.heldPosition.position, general.heldPosition.rotation);
        StartCoroutine(MoveWeaponModel(holsterPosition.localPosition, holsterPosition.localRotation, switchSpeed));

        yield return new WaitForSeconds(switchSpeed);

        gameObject.SetActive(false);
        isSwitchingWeapon = false;
    }

    void AssignFiringModes(int index)
    {
        general = firingModes[index].general;
        fireControls = firingModes[index].fireControls;
        magazine = firingModes[index].magazine;
        optics = firingModes[index].optics;
    }

    #endregion




    IEnumerator MoveWeaponModel(Vector3 newLocalPosition, Quaternion newLocalRotation, float time, AnimationCurve curve = null, bool interruptsOtherAnimations = true)
    {
        #region Check for existing animations and cancel/override
        if (isAnimating == true)
        {
            if (interruptsOtherAnimations == false)
            {
                // Abort coroutine if another animation is already running
                yield break;
            }
            else
            {
                // Unless interruptsOtherAnimations is enabled, in which case it cancels the currently running animation.
                // It disables isAnimating, then other coroutines will notice and end prematurely.
                isAnimating = false;
                yield return new WaitForEndOfFrame();
            }
        }
        #endregion

        #region Setup
        isAnimating = true;
        if (curve == null)
        {
            curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        }
        Vector3 oldPosition = weaponModel.localPosition;
        Quaternion oldRotation = weaponModel.localRotation;
        float timer = 0;
        #endregion

        #region Loop
        while (timer < 1)
        {
            // End loop prematurely if isAnimating is remotely declared false
            if (isAnimating == false)
            {
                yield break;
            }

            float percentage = curve.Evaluate(timer);

            weaponModel.localPosition = Vector3.Lerp(oldPosition, newLocalPosition, percentage);
            weaponModel.localRotation = Quaternion.Lerp(oldRotation, newLocalRotation, percentage);

            timer += Time.deltaTime / time;
            yield return new WaitForEndOfFrame();
        }
        #endregion

        #region End
        weaponModel.localPosition = Vector3.Lerp(oldPosition, newLocalPosition, curve.Evaluate(1));
        weaponModel.localRotation = Quaternion.Lerp(oldRotation, newLocalRotation, curve.Evaluate(1));

        isAnimating = false;
        #endregion
    }

    #region Firing functions
    public void RecoilHandler(float recoilApplyRate, WeaponHandler playerHolding)
    {
        if (recoilToApply > 0)
        {
            float r = recoilToApply * recoilApplyRate * Time.deltaTime;
            Vector2 rd = new Vector2(Random.Range(-1f, 1f), 1);
            if (rd.magnitude > 1)
            {
                rd.Normalize();
            }
            playerHolding.ph.pc.LookAngle(r * rd); // Add recoil
            recoilToApply -= r;
        }
        else if (!Input.GetButton("Fire")) // Return recoil using recoil recovery float
        {
            //print("Recovering from recoil");
        }

        // Figure out how to have visual gun recoil
        // 1: Use lerping and an animation curve to move gun between standard and fully recoiled position
        // 2: Just give it a spring joint and use Physics.AddForce or AddForceAtPosition to make it jump back
    }

    public void ADSHandler(GunOpticsStats optics)
    {
        if (optics == null)
        {
            return;
        }

        if (playerHolding.toggleAim == true)
        {
            if (Input.GetButtonDown("MouseRight"))
            {
                isAiming = !isAiming;
            }
        }
        else
        {
            if (Input.GetButton("MouseRight"))
            {
                isAiming = true;
            }
            else
            {
                isAiming = false;
            }
        }

        if (isAiming)
        {
            adsTimer += Time.deltaTime / optics.transitionTime;
        }
        else
        {
            adsTimer -= Time.deltaTime / optics.transitionTime;
        }
        adsTimer = Mathf.Clamp01(adsTimer);

        LerpADS(adsTimer);
    }

    void LerpADS(float timer)
    {
        if (optics == null)
        {
            return;
        }
        
        PlayerController pc = playerHolding.ph.pc;

        // Change FOV for zoom (update later to use variablevaluefloat)
        float defaultFOV = pc.fieldOfView.defaultValue;
        pc.playerCamera.fieldOfView = Mathf.Lerp(defaultFOV, defaultFOV / optics.magnification, timer);

        // Reduce sensitivity and movement speed, alter accuracy if specified
        sensitivityWhileAiming.SetIntensity(timer);
        speedWhileAiming.SetIntensity(timer);

        // Lerp weapon model between hip position and ADS position. Should I make this work with the MoveWeaponModel function?
        // Something is screwy here
        Vector3 relativeAimPosition = pc.head.position + (transform.position - optics.sightLine.position);
        weaponModel.transform.position = Vector3.Lerp(general.heldPosition.position, relativeAimPosition, timer);

        // Toggle overlay
        playerHolding.ph.hud.ADSTransition(timer, optics.scopeGraphic);
    }

    void CancelADS()
    {
        zoomTimer = 0;
        
        isAiming = false;
        adsTimer = 0;
        LerpADS(0);

    }










    public void AimHandler(GunOpticsStats os, Transform hipPosition, bool toggleAim)
    {
        if (toggleAim == true)
        {
            if (Input.GetButtonDown("MouseRight"))
            {
                isAiming = !isAiming;
            }
        }
        else
        {
            if (Input.GetButton("MouseRight"))
            {
                isAiming = true;
            }
            else
            {
                isAiming = false;
            }
        }

        if (isAiming) //Sets timer value to specify lerping of variables
        {
            zoomTimer += Time.deltaTime / os.transitionTime;
            LerpSights(os, zoomTimer, os.aimPosition);
        }
        else
        {
            zoomTimer -= Time.deltaTime / os.transitionTime;
            LerpSights(os, zoomTimer, hipPosition);
        }
        zoomTimer = Mathf.Clamp01(zoomTimer);
    }

    void LerpSights(GunOpticsStats os, float timer, Transform newWeaponPosition)
    {

        // Reduces FOV to zoom in camera
        zoomVariable = Mathf.Lerp(1, 1 / os.magnification, timer);
        playerHolding.ph.pc.playerCamera.fieldOfView = playerHolding.ph.pc.fieldOfView.defaultValue * zoomVariable;

        // Reduce sensitivity and movement speed
        sensitivityWhileAiming.SetIntensity(timer);
        speedWhileAiming.SetIntensity(timer);
        // Alter accuracy if specified

        // Move weapon model. THIS DOES NOT WORK FOR SOME REASON
        float moveTime = Vector3.Distance(weaponModel.transform.position, newWeaponPosition.position) * os.transitionTime;
        //MoveWeaponModel(newWeaponPosition.position, newWeaponPosition.rotation, moveTime);

        // Toggle overlay
        playerHolding.ph.hud.ADSTransition(timer, optics.scopeGraphic);
    }

    #endregion

    #region Reloading functions
    public void ReloadHandler(float reloadTime, float fireTimer, float roundsPerMinute, int roundsReloaded, Resource magazine, WeaponHandler playerHolding, AmmunitionType caliber)
    {
        int remainingAmmo = playerHolding.ph.a.GetStock(caliber) - magazine.current; // Checks how much spare ammunition the player has

        // If reload button is pressed and weapon's magazine is not full OR if magazine is empty and gun is finished firing, PLUS if ammunition remains and the player is not already reloading
        if (((Input.GetButtonDown("Reload") && magazine.current < magazine.max) || (magazine.current <= 0 && fireTimer >= 60 / roundsPerMinute)) && isReloading == false && remainingAmmo > 0)
        {
            reloadTimer = 0;
            isReloading = true;
            print("Reload sequence started");
        }
        if (isReloading == true)
        {
            reloadTimer += Time.deltaTime / reloadTime;

            // If magazine is full, there is no more ammunition, or reload is interrupted by another action
            if (isReloading == true && (magazine.current >= magazine.max || remainingAmmo <= 0 || (Input.GetButtonDown("Fire") && magazine.current > 0))) // Also include button options for melee attacking and any other functions that would cancel out the reload function
            {
                CancelReload();
            }

            if (reloadTimer >= 1) // If reload time has been reached, reload ammunition into magazine
            {
                print("Ammo reloaded");
                if (remainingAmmo < roundsReloaded) // If there is not enough ammunition to reload the usual amount
                {
                    magazine.current += remainingAmmo; // Reload all remaining ammunition
                }
                else
                {
                    magazine.current += roundsReloaded; // Reload standard amount of ammunition per reload cycle
                }
                magazine.current = Mathf.Clamp(magazine.current, 0, magazine.max); // Ensure magazine is not overloaded

                // If magazine is full, there is no more ammunition, or reload is interrupted by another action
                if (magazine.current >= magazine.max || remainingAmmo <= 0) // Also include button options for melee attacking and any other functions that would cancel out the reload function
                {
                    CancelReload();
                }
                else
                {
                    reloadTimer = 0; // Reset reload timer
                    print("Reload sequence continued");
                }
            }
        }
    }

    public void ReloadHandler(float reloadTime, float fireTimer, float roundsPerMinute, int roundsReloaded, Resource magazine)
    {
        // If reload button is pressed and weapon's magazine is not full OR if magazine is empty and gun is finished firing, PLUS if ammunition remains and the player is not already reloading
        if (((Input.GetButtonDown("Reload") && magazine.current < magazine.max) || (magazine.current <= 0 && fireTimer >= 60 / roundsPerMinute)) && isReloading == false)
        {
            reloadTimer = 0;
            isReloading = true;
            print("Reload sequence started");
        }
        if (isReloading == true)
        {
            reloadTimer += Time.deltaTime / reloadTime;

            // If magazine is full, there is no more ammunition, or reload is interrupted by another action
            if (isReloading == true && (magazine.current >= magazine.max || (Input.GetButtonDown("Fire") && magazine.current > 0))) // Also include button options for melee attacking and any other functions that would cancel out the reload function
            {
                CancelReload();
            }

            if (reloadTimer >= 1) // If reload time has been reached, reload ammunition into magazine
            {
                print("Ammo reloaded");
                magazine.current += roundsReloaded; // Reload standard amount of ammunition per reload cycle
                magazine.current = Mathf.Clamp(magazine.current, 0, magazine.max); // Ensure magazine is not overloaded

                // If magazine is full, there is no more ammunition, or reload is interrupted by another action
                if (magazine.current >= magazine.max) // Also include button options for melee attacking and any other functions that would cancel out the reload function
                {
                    CancelReload();
                }
                else
                {
                    reloadTimer = 0; // Reset reload timer
                    print("Reload sequence continued");
                }
            }
        }
    }

    public void CancelReload()
    {
        reloadTimer = 0;
        isReloading = false;
        //print("Reload sequence ended");
    }


    #endregion
}
