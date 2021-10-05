using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class GunFiringMode
{
    public string name = "New Firing Mode";
    public string description = "A distinct mode of operation for this gun.";

    [Header("Stats")]
    public GunGeneralStats general;
    public GunFireControlStats fireControls;
    public GunMagazineStats magazine;
    public GunOpticsStats optics;

    [Header("Other")]
    public Sprite hudIcon;
    public float switchSpeed;

    Gun attachedGun;
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
    public Renderer weaponVisual;
    public Transform weaponVisualTransform;
    //public AudioSource weaponSoundSource;
    public Sprite weaponSelectorIcon;

    [Header("Other")]
    public float attackMessageLimitDelay = 1;

    #region Additional variables for weapon function
    public GunFiringMode currentMode
    {
        get
        {
            return firingModes[firingModeIndex];
        }
    }
    GunGeneralStats general
    {
        get
        {
            return currentMode.general;
        }
    }
    GunFireControlStats fireControls
    {
        get
        {
            return currentMode.fireControls;
        }
    }
    GunMagazineStats magazine
    {
        get
        {
            return currentMode.magazine;
        }
    }
    GunOpticsStats optics
    {
        get
        {
            return currentMode.optics;
        }
    }

    // Additional optics stats
    [HideInInspector] public bool isAiming;
    [HideInInspector] public float zoomVariable;
    [HideInInspector] public PercentageModifier sensitivityWhileAiming = new PercentageModifier(0, true, false);
    [HideInInspector] public PercentageModifier speedWhileAiming = new PercentageModifier();
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


    public IEnumerator currentAction;


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
                else if (playerHolding.handler.ammo.GetStock(f.general.ammoType) < f.magazine.data.max)
                {
                    f.magazine.data.current = playerHolding.handler.ammo.GetStock(f.general.ammoType);
                }
                else
                {
                    f.magazine.data.current = f.magazine.data.max;
                }
            }
        }
    }

    private void Start()
    {
        playerHolding = GetComponentInParent<WeaponHandler>();

        playerHolding.handler.movement.sensitivityModifier.Add(sensitivityWhileAiming, this);
        playerHolding.handler.movement.movementSpeed.Add(speedWhileAiming, this);

        OnEnable();
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
        if (isSwitchingWeapon == false && isSwitchingFireMode == false && playerHolding.weaponSelector.MenuIsActive == false)
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
            if (playerHolding.handler.PlayerState() == GameState.Active
                && Input.GetButton("Fire")
                && fireControls.fireTimer >= 60 / fireControls.roundsPerMinute
                && (fireControls.burstCounter < fireControls.maxBurst || fireControls.maxBurst <= 0)
                && (general.consumesAmmo == false || (playerHolding.handler.ammo.GetStock(general.ammoType) >= general.ammoPerShot))
                && (magazine == null || (magazine.data.current >= general.ammoPerShot && reloading == null)))
            {
                CancelReloadSequence();

                fireControls.fireTimer = 0; // Reset fire timer to count up to next shot
                fireControls.burstCounter += 1;

                if (general.consumesAmmo == true)
                {
                    playerHolding.handler.ammo.Spend(general.ammoType, general.ammoPerShot);
                }

                if (magazine != null)
                {
                    magazine.data.current -= general.ammoPerShot;
                }

                recoilToApply += general.recoil;

                #region Calculate and fire shot
                Transform head = playerHolding.handler.movement.head;
                Vector3 aimDirection = head.forward;
                if (optics == null || isAiming == false)
                {
                    float accuracy = playerHolding.standingAccuracy.Calculate();
                    aimDirection = Quaternion.Euler(Random.Range(-accuracy, accuracy), Random.Range(-accuracy, accuracy), Random.Range(-accuracy, accuracy)) * aimDirection;
                }

                general.Shoot(playerHolding.handler, head.position, aimDirection, head.up);
                #endregion

                #region Send attack message if the timer is right
                if (attackMessageLimitTimer >= attackMessageLimitDelay) // Sends attack message
                {
                    AttackMessage am = AttackMessage.Ranged(playerHolding.handler, transform.position, transform.forward, general.range, general.projectilePrefab.diameter, playerHolding.standingAccuracy.Calculate() + general.projectileSpread, general.projectilePrefab.velocity, general.projectilePrefab.hitDetection);
                    EventJunction.Transmit(am);
                    

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

            /*
            if (optics != null)
            {
                optics.ADSHandler(this, currentMode);
            }
            */
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

        isSwitchingFireMode = false;
    }
    public IEnumerator Draw()
    {
        isSwitchingWeapon = true;


        gameObject.SetActive(true);
        // Draw weapon animation
        weaponVisualTransform.transform.SetPositionAndRotation(holsterPosition.position, holsterPosition.rotation);

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
        weaponVisualTransform.transform.SetPositionAndRotation(general.heldPosition.position, general.heldPosition.rotation);
        ChangeWeaponModelPosition(holsterPosition.localPosition, holsterPosition.localRotation, switchSpeed);

        // Waits until animation has completed
        yield return new WaitForSeconds(switchSpeed);

        // Disables weapon object and ends sequence
        gameObject.SetActive(false);
        isSwitchingWeapon = false;
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
            playerHolding.handler.movement.LookAngle(r * rd); // Add recoil
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




    public static readonly AnimationCurve cosmeticRecoilCurve = new AnimationCurve
    {
        keys = new Keyframe[]
        {
            new Keyframe(0, 0, 0, 90),
            new Keyframe(0.25f, 1, 0, 0),
            new Keyframe(1, 0, 0, 0),
        }
    };

    
    public void CosmeticRecoil(Transform recoilOrientation)
    {
        Debug.Log("Cosmetic recoil activated");
        StartCoroutine(MoveWeaponCosmetically(general.heldPosition, recoilOrientation, cosmeticRecoilCurve));
    }

    public IEnumerator MoveWeaponCosmetically(Transform first, Transform second, AnimationCurve curve)
    {
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime / (60 / fireControls.roundsPerMinute * 2);
            timer = Mathf.Clamp(timer, 0f, 1f);
            Debug.Log(timer);

            float value = curve.Evaluate(timer);
            weaponVisualTransform.position = Vector3.Lerp(first.position, second.position, value);
            weaponVisualTransform.rotation = Quaternion.Lerp(first.rotation, second.rotation, value);

            yield return null;
        }
    }


    #region ADS functions
    public void ADSHandler()
    {
        if (optics == null)
        {
            return;
        }

        if (playerHolding.toggleAim == true && Input.GetButtonDown("MouseRight"))
        {
            isAiming = !isAiming;
        }
        else
        {
            isAiming = Input.GetButton("MouseRight");
        }


        float timeToAdd = -Time.deltaTime / optics.transitionTime;
        if (isAiming)
        {
            timeToAdd = -timeToAdd;
        }
        adsTimer += timeToAdd;
        adsTimer = Mathf.Clamp01(adsTimer);

        LerpADS(adsTimer);

        if (isAiming)
        {
            /*
            // Produces two varying noise values for the two axes (these will be between zero and one).
            float swayX = Mathf.PerlinNoise(Time.time * aimSwaySpeed, 0);
            float swayY = Mathf.PerlinNoise(0, Time.time * aimSwaySpeed);
            Vector2 sway = new Vector2(swayX, swayY); // Combine into a Vector2
            sway = (sway - Vector2.one * 0.5f) * 2; // Subtract 0.5 then multiply by 2 so the values are between -1 and 1
            sway = sway.normalized * gun.playerHolding.standingAccuracy.Calculate(); // Normalise and multiply by player's current accuracy
            
            Quaternion swayedAimDirection = Quaternion.Euler(sway.y, sway.x, 0);
            gun.playerHolding.handler.movement.aimDirectionTransform.localRotation = swayedAimDirection;
            */
        }
        else
        {
            //gun.playerHolding.handler.movement.aimDirectionTransform.localRotation = Quaternion.identity;
        }

        // Figure out sensitivity while aiming and print it. I presume that something with the mechanics is setting the sensitivity to nothing.
        // Debug.Log("Player camera sensitivity = " + playerHolding.ph.movement.sensitivityModifier.Calculate());
    }
    void LerpADS(float timer)
    {
        if (optics == null)
        {
            return;
        }
        
        PlayerController pc = playerHolding.handler.movement;

        // Change FOV for zoom (update later to use variablevaluefloat)
        float defaultFOV = pc.fieldOfView.defaultValue;
        pc.playerCamera.fieldOfView = Mathf.Lerp(defaultFOV, defaultFOV / optics.magnification, timer);

        // Reduce sensitivity and movement speed, alter accuracy if specified
        sensitivityWhileAiming.SetIntensity(timer);
        speedWhileAiming.SetIntensity(timer);



        // Lerp weapon model between hip position and ADS position. Should I make this work with the MoveWeaponModel function?
        Transform wmt = weaponVisualTransform;
        
        Vector3 relativePosition = Misc.PositionWhereChildIsAtSamePositionAsAnotherTransform(wmt.position, optics.sightLine.position, pc.head.position);
        Quaternion relativeRotation = Misc.RotationWhereChildIsAtSameRotationAsAnotherTransform(wmt.rotation, optics.sightLine.rotation, pc.head.rotation);
        wmt.position = Vector3.Lerp(general.heldPosition.position, relativePosition, timer);
        wmt.rotation = Quaternion.Lerp(general.heldPosition.rotation, relativeRotation, timer);

        // Toggle overlay
        playerHolding.handler.hud.ADSTransition(timer, optics.scopeGraphic);

        weaponVisual.enabled = (timer < optics.whenToDisableWeaponVisual);
    }
    void CancelADSInstantly()
    {
        adsTimer = 0;
        LerpADS(0);
        isAiming = false;
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

        magazine.onReloadStart.Invoke();

        while (magazine.data.current < magazine.data.max || RemainingAmmo() <= 0)
        {
            yield return new WaitForSeconds(magazine.reloadTime);
            // Add ammunition to the magazine based off roundsReloadedPerCycle. Unless there is not enough ammo for a full cycle, in which case load all remaining ammo.
            magazine.data.current += Mathf.Min(magazine.roundsReloadedPerCycle, RemainingAmmo());
            magazine.data.current = Mathf.Clamp(magazine.data.current, 0, magazine.data.max); // Ensure magazine is not overloaded

            magazine.onRoundsReloaded.Invoke();
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

            magazine.onReloadEnd.Invoke();
        }
    }

    int RemainingAmmo()
    {
        return playerHolding.handler.ammo.GetStock(general.ammoType) - magazine.data.current;
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
        Vector3 oldPosition = weaponVisualTransform.localPosition;
        Quaternion oldRotation = weaponVisualTransform.localRotation;
        float timer = 0;

        while (timer < 1)
        {
            float percentage = curve.Evaluate(timer);

            weaponVisualTransform.localPosition = Vector3.Lerp(oldPosition, newLocalPosition, percentage);
            weaponVisualTransform.localRotation = Quaternion.Lerp(oldRotation, newLocalRotation, percentage);

            timer += Time.deltaTime / time;
            yield return new WaitForEndOfFrame();
        }

        float final = curve.Evaluate(1);
        weaponVisualTransform.localPosition = Vector3.Lerp(oldPosition, newLocalPosition, final);
        weaponVisualTransform.localRotation = Quaternion.Lerp(oldRotation, newLocalRotation, final);
    }

    void CancelMoveWeaponModel()
    {
        if (animatingWeaponModel != null)
        {
            StopCoroutine(animatingWeaponModel);
            animatingWeaponModel = null;
        }
    }


    Vector3 position;
    Vector3 rotation;
    private void LateUpdate()
    {
        
    }

    #endregion


    public void PickUp(PlayerHandler ph)
    {
        playerHolding = ph.weapons;
        transform.SetParent(playerHolding.defaultHoldingPosition);
        GetComponent<Collider>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = false;
        //weaponVisualTransform.SetPositionAndRotation()
        enabled = true;
    }

    public void Drop()
    {
        transform.SetParent(null);
        weaponVisualTransform.localPosition = Vector3.zero;
        weaponVisualTransform.localRotation = Quaternion.identity;
        GetComponent<Collider>().enabled = true;
        GetComponent<Rigidbody>().isKinematic = false;
        playerHolding = null;
        enabled = false;
    }


}