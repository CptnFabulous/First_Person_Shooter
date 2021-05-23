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
    public Sprite weaponSelectorIcon;

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

    #endregion

    #region Universal variables
    [HideInInspector] public WeaponHandler playerHolding;

    // Switching modes
    [HideInInspector] public bool isSwitchingWeapon;
    [HideInInspector] public bool isSwitchingFireMode;

    IEnumerator switching;
    IEnumerator reloading;
    IEnumerator animatingWeaponModel;


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
            Debug.LogError("No firing modes present for " + name + "!");
        }
    }

    private void OnEnable()
    {
        // Check all magazines, and set to zero if the player does not have enough ammo (or there is no player assigned)
        foreach(GunFiringMode f in firingModes)
        {
            if (f.magazine != null)
            {
                if (playerHolding == null)
                {
                    f.magazine.data.current = 0;
                }
                else if (playerHolding.ph.a.GetStock(f.general.ammoType) < f.magazine.data.max)
                {
                    f.magazine.data.current = playerHolding.ph.a.GetStock(f.general.ammoType);
                }
            }
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
        // Ensure things don't happen if the game is paused.
        if (Time.timeScale == 0)
        {
            return;
        }
        
        
        attackMessageLimitTimer += Time.deltaTime;

        // If the player is not switching weapons or fire modes, and is therefore able to operate the weapon
        if (isSwitchingWeapon == false && isSwitchingFireMode == false && playerHolding.weaponSelector.MenuIsActive() == false)
        {
            #region Firing mode controls
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");
            if (scrollInput != 0 && firingModes.Length > 1) // Switch firing modes with the scroll wheel
            {
                int i = firingModeIndex;
                i -= (int)new Vector2(scrollInput, 0).normalized.x;
                i = (int)Misc.InverseClamp(i, 0, firingModes.Length - 1);
                StartCoroutine(SwitchMode(i));
            }
            #endregion

            #region Fire controls
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
                && (magazine == null || (magazine.data.current >= general.ammoPerShot && reloading == null)))
            {
                CancelReloadSequence();

                #region Adjust fire control variables
                fireControls.fireTimer = 0; // Reset fire timer to count up to next shot
                fireControls.burstCounter += 1;
                #endregion

                #region Consume ammo
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
            #endregion

            #region ADS and reload controls
            ADSHandler();
            ReloadHandler();
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
            CancelADSInstantly();
        }

        if (newMode.optics != null)
        {
            sensitivityWhileAiming.percentageValue = 100 / newMode.optics.magnification;
            speedWhileAiming.percentageValue = newMode.optics.moveSpeedReductionPercentage;
        }

        if (magazine != null)
        {
            // If a reload sequence is in progress, cancel it
            CancelReloadSequence();

            if (magazine == newMode.magazine && general.ammoType != newMode.general.ammoType)
            {
                magazine.data.current = 0;
            }
        }

        yield return new WaitForSeconds(newMode.switchSpeed);

        firingModeIndex = index;
        AssignFiringModes(firingModeIndex);

        isSwitchingFireMode = false;
    }

    public IEnumerator Draw()
    {
        isSwitchingWeapon = true;

        // Reassigns firing modes
        AssignFiringModes(firingModeIndex);

        gameObject.SetActive(true);
        // Draw weapon animation
        weaponModel.transform.SetPositionAndRotation(holsterPosition.position, holsterPosition.rotation);

        // Animate weapon model moving from holstered to drawn position
        Transform newMoveTransform = general.heldPosition;
        ChangeWeaponModelPosition(newMoveTransform.localPosition, newMoveTransform.localRotation, switchSpeed);

        // Waits until animation has completed
        yield return new WaitForSeconds(switchSpeed);

        // End sequence
        isSwitchingWeapon = false;
    }

    public IEnumerator Holster()
    {
        isSwitchingWeapon = true;

        // Cancel ADS animations
        if (optics != null)
        {
            CancelADSInstantly();
        }

        // Cancel reload sequence
        if (magazine != null)
        {
            CancelReloadSequence();
        }

        // Animate weapon model moving from drawn to holstered position
        weaponModel.transform.SetPositionAndRotation(general.heldPosition.position, general.heldPosition.rotation);
        ChangeWeaponModelPosition(holsterPosition.localPosition, holsterPosition.localRotation, switchSpeed);

        // Waits until animation has completed
        yield return new WaitForSeconds(switchSpeed);

        // Disables weapon object and ends sequence
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

    #region ADS functions
    public void ADSHandler()
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

        // Figure out sensitivity while aiming and print it. I presume that something with the mechanics is setting the sensitivity to nothing.
        Debug.Log("Player camera sensitivity = " + playerHolding.ph.pc.sensitivityModifier.Calculate());
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
        Transform wmt = weaponModel;
        
        Vector3 relativePosition = Misc.PositionWhereChildIsAtSamePositionAsAnotherTransform(wmt.position, optics.sightLine.position, pc.head.position);
        Quaternion relativeRotation = Misc.RotationWhereChildIsAtSameRotationAsAnotherTransform(wmt.rotation, optics.sightLine.rotation, pc.head.rotation);
        wmt.position = Vector3.Lerp(general.heldPosition.position, relativePosition, timer);
        wmt.rotation = Quaternion.Lerp(general.heldPosition.rotation, relativeRotation, timer);

        // Toggle overlay
        playerHolding.ph.hud.ADSTransition(timer, optics.scopeGraphic);
    }

    void CancelADSInstantly()
    {
        zoomTimer = 0;
        
        isAiming = false;
        adsTimer = 0;
        LerpADS(0);
    }

    IEnumerator CancelADS()
    {
        float timer = 1;
        while (timer > 0)
        {
            timer -= Time.deltaTime / optics.transitionTime;

            LerpADS(timer);

            yield return new WaitForEndOfFrame();
        }


    }
    


    IEnumerator EnableOrDisableADS(bool enabled)
    {
        float timer = 0;

        while (timer < 1)
        {
            timer += Time.deltaTime / optics.transitionTime;

            float t = timer;
            if (enabled == false)
            {
                t = 1 / timer;
            }

            LerpADS(t);

            yield return new WaitForEndOfFrame();
        }
    }


    #endregion

    #region Reloading functions
    void ReloadHandler()
    {
        // End function prematurely if there is no magazine to feed from
        if (magazine == null)
        {
            return;
        }
        
        // Checks if either the player has pressed the reload button, or the magazine is empty
        bool manualReload = Input.GetButtonDown("Reload") && magazine.data.current < magazine.data.max;
        bool automaticReload = magazine.data.current <= 0;

        // Checks for manual or automatic reload, and if the player is not already reloading
        if ((manualReload || automaticReload) && reloading == null && RemainingAmmo() > 0)
        {
            // Starts the current reload sequence
            reloading = ReloadSequence();
            StartCoroutine(reloading);
        }

        // Put this into the main firing code
        if (Input.GetButtonDown("Fire") && magazine.data.current > 0)
        {
            CancelReloadSequence();
        }
    }

    IEnumerator ReloadSequence()
    {
        // Wait for firing sequence to end
        yield return new WaitUntil(() => fireControls.fireTimer >= 60 / fireControls.roundsPerMinute);

        // Start reload sequence, e.g. play animations

        while (magazine.data.current < magazine.data.max || RemainingAmmo() <= 0)
        {
            yield return new WaitForSeconds(magazine.reloadTime);
            // Add ammunition to the magazine based off roundsReloadedPerCycle. Unless there is not enough ammo for a full cycle, in which case load all remaining ammo.
            magazine.data.current += Mathf.Min(magazine.roundsReloadedPerCycle, RemainingAmmo());
            magazine.data.current = Mathf.Clamp(magazine.data.current, 0, magazine.data.max); // Ensure magazine is not overloaded
        }

        // End reload sequence
        CancelReloadSequence();
    }

    void CancelReloadSequence()
    {
        if (reloading != null)
        {
            StopCoroutine(reloading);
            reloading = null;
        }
    }

    int RemainingAmmo()
    {
        return playerHolding.ph.a.GetStock(general.ammoType) - magazine.data.current;
    }
    #endregion

    #region Cosmetics
    void ChangeWeaponModelPosition(Vector3 newLocalPosition, Quaternion newLocalRotation, float time, AnimationCurve curve = null, bool interruptsOtherAnimations = true)
    {
        if (animatingWeaponModel != null)
        {
            if (interruptsOtherAnimations == true)
            {
                CancelMoveWeaponModel();
            }
            else
            {
                return;
            }
        }

        animatingWeaponModel = MoveWeaponModelSequence(newLocalPosition, newLocalRotation, time, curve);
        StartCoroutine(animatingWeaponModel);
    }

    IEnumerator MoveWeaponModelSequence(Vector3 newLocalPosition, Quaternion newLocalRotation, float time, AnimationCurve curve = null)
    {
        if (curve == null)
        {
            curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        }
        Vector3 oldPosition = weaponModel.localPosition;
        Quaternion oldRotation = weaponModel.localRotation;
        float timer = 0;

        while (timer < 1)
        {
            float percentage = curve.Evaluate(timer);

            weaponModel.localPosition = Vector3.Lerp(oldPosition, newLocalPosition, percentage);
            weaponModel.localRotation = Quaternion.Lerp(oldRotation, newLocalRotation, percentage);

            timer += Time.deltaTime / time;
            yield return new WaitForEndOfFrame();
        }

        float final = curve.Evaluate(1);
        weaponModel.localPosition = Vector3.Lerp(oldPosition, newLocalPosition, final);
        weaponModel.localRotation = Quaternion.Lerp(oldRotation, newLocalRotation, final);
    }

    void CancelMoveWeaponModel()
    {
        if (animatingWeaponModel != null)
        {
            StopCoroutine(animatingWeaponModel);
            animatingWeaponModel = null;
        }
    }
    #endregion

}