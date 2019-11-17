using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Stat classes
[System.Serializable]
public class FireControlStats
{
#if UNITY_EDITOR
    public string name;
#endif
    public float roundsPerMinute;
    public int maxBurst;
    [HideInInspector] public float fireTimer;
    [HideInInspector] public float burstCounter;
}

[System.Serializable]
public class AccuracyStats
{
#if UNITY_EDITOR
    public string name;
#endif
    [Range(0, 180)] public float projectileSpread;
    public float range;
    public LayerMask rayDetection;
}

[System.Serializable]
public class RecoilStats
{
#if UNITY_EDITOR
    public string name;
#endif
    public float recoil;
    public float recoilApplyRate;
    public float recoilRecovery;
}

[System.Serializable]
public class OpticsStats
{
#if UNITY_EDITOR
    public string name;
#endif
    public float magnification;
    public float transitionTime;
    [Range(-1, 0)] public float moveSpeedReduction;
    public Transform aimPosition;
    public Sprite scopeGraphic;
}

[System.Serializable]
public class AmmunitionStats
{
#if UNITY_EDITOR
    public string name;
#endif
    public bool consumesAmmo;
    public AmmunitionType ammoType;
    public int ammoPerShot = 1;
}

[System.Serializable]
public class MagazineStats
{
#if UNITY_EDITOR
    public string name;
#endif
    public Resource magazine;
    public int roundsReloaded;
    public float reloadTime;
}

[System.Serializable]
public class ProjectileStats
{
#if UNITY_EDITOR
    public string name;
#endif
    public ProjectileData projectile;
    public int projectileCount;
    public Transform muzzle;
}
#endregion

[System.Serializable]
public class FiringMode
{
    [Header("Stat profile references")]
    public string name;
    public int fireControlMode;
    public int accuracyMode;
    public int projectileMode;
    public int recoilMode;
    public int opticsMode;
    public int ammunitionMode;
    public int magazineMode;

    [Header("Other")]
    public float switchSpeed;

    [Header("Cosmetics")]
    public Sprite hudIcon;
    public Transform heldPosition;
    public AudioClip firingNoise;
    public MuzzleFlashEffect muzzleFlash;
    public float muzzleFlashRelativeDuration;
    public ParticleSystem shellEjection;
}


public class RangedWeapon : MonoBehaviour
{
    public WeaponHandler playerHolding;

    [Header("Fire controls")]
    public FireControlStats[] fireControlModes;
    [Header("Accuracy stats")]
    public AccuracyStats[] accuracyModes;
    [Header("Recoil stats")]
    public RecoilStats[] recoilModes;
    [Header("Optics")]
    public OpticsStats[] opticsModes;
    [Header("Ammunition types")]
    public AmmunitionStats[] ammunitionModes;
    [Header("Magazines")]
    public MagazineStats[] magazineModes;
    [Header("Projectiles")]
    public ProjectileStats[] projectileModes;

    [HideInInspector] public FireControlStats fireControls;

    [HideInInspector] public AccuracyStats accuracy;

    [HideInInspector] public RaycastHit targetFound;
    [HideInInspector] public Vector3 aimDirection;
    [HideInInspector] public Vector3 target;

    [HideInInspector] public ProjectileStats projectile;

    [HideInInspector] public OpticsStats optics;
    [HideInInspector] public bool isAiming;
    [HideInInspector] public float zoomVariable;
    [HideInInspector] public float zoomTimer;

    [HideInInspector] public RecoilStats recoil;
    [HideInInspector] public float recoilToApply;
    [HideInInspector] public Vector2 aimOrigin;
    [HideInInspector] public Vector2 aimCurrent;

    [HideInInspector] public AmmunitionStats ammunition;

    [HideInInspector] public MagazineStats magazine;
    [HideInInspector] public bool isReloading;
    [HideInInspector] public float reloadTimer;

    [Header("Firing modes")]
    public FiringMode[] firingModes;

    [Header("Universal variables")]
    public float switchSpeed;
    public GameObject weaponModel;
    public AudioSource weaponSoundSource;
    public Transform holsterPosition;
    public int firingModeIndex;

    #region Universal variables (variables that can be used multiple times for different firing modes)
    // Switching modes
    [HideInInspector] public bool isSwitchingWeapon;
    [HideInInspector] public bool isSwitchingFireMode;
    
    // Moving weapon model
    Transform newWeaponTransform;
    Transform oldWeaponTransform;
    Transform previousNewWeaponTransform;
    float moveWeaponTime;
    float moveWeaponTimer;
    #endregion

    private void Reset()
    {
        OnValidate();
    }

    private void OnValidate()
    {
        if (fireControlModes.Length <= 0)
        {
            fireControlModes = new FireControlStats[1];
        }

        if (accuracyModes.Length <= 0)
        {
            accuracyModes = new AccuracyStats[1];
        }

        if (projectileModes.Length <= 0)
        {
            projectileModes = new ProjectileStats[1];
        }

        if (firingModes.Length <= 0)
        {
            firingModes = new FiringMode[1];
        }

        foreach (FiringMode fm in firingModes)
        {
            fm.fireControlMode = Mathf.Clamp(fm.fireControlMode, 0, fireControlModes.Length - 1);
            fm.accuracyMode = Mathf.Clamp(fm.accuracyMode, 0, accuracyModes.Length - 1);
            fm.projectileMode = Mathf.Clamp(fm.projectileMode, 0, projectileModes.Length - 1);
            fm.recoilMode = Mathf.Clamp(fm.recoilMode, -1, recoilModes.Length - 1);
            fm.opticsMode = Mathf.Clamp(fm.opticsMode, -1, opticsModes.Length - 1);
            fm.ammunitionMode = Mathf.Clamp(fm.ammunitionMode, -1, ammunitionModes.Length - 1);
            fm.magazineMode = Mathf.Clamp(fm.magazineMode, -1, magazineModes.Length - 1);
        }

        firingModeIndex = Mathf.Clamp(firingModeIndex, 0, firingModes.Length - 1);
    }

    #region Get optional weapon stats
    public OpticsStats GetOpticsStats(int index)
    {
        if (firingModes[index].opticsMode <= -1 || opticsModes.Length <= 0)
        {
            return null;
        }
        return opticsModes[firingModes[index].opticsMode];
    }

    public RecoilStats GetRecoilStats(int index)
    {
        if (firingModes[index].recoilMode <= -1 || recoilModes.Length <= 0)
        {
            return null;
        }
        return recoilModes[firingModes[index].recoilMode];
    }

    public AmmunitionStats GetAmmunitionStats(int index)
    {
        if (firingModes[index].ammunitionMode <= -1 || ammunitionModes.Length <= 0)
        {
            return null;
        }
        return ammunitionModes[firingModes[index].ammunitionMode];
    }

    public MagazineStats GetMagazineStats(int index)
    {
        if (firingModes[index].magazineMode <= -1 || magazineModes.Length <= 0)
        {
            return null;
        }
        return magazineModes[firingModes[index].magazineMode];
    }
    #endregion
    
    private void Start()
    {
        ResetWeaponMoveVariables();
    }
    
    void Update()
    {
        #region Makes easy references to the appropriate sets of stats so I don't have to type "xModes[firingModes[firingModeIndex].xMode]" every single bloody time I need to reference a set of stats
        fireControls = fireControlModes[firingModes[firingModeIndex].fireControlMode];
        accuracy = accuracyModes[firingModes[firingModeIndex].accuracyMode];
        projectile = projectileModes[firingModes[firingModeIndex].projectileMode];
        optics = GetOpticsStats(firingModeIndex);
        recoil = GetRecoilStats(firingModeIndex);
        ammunition = GetAmmunitionStats(firingModeIndex);
        magazine = GetMagazineStats(firingModeIndex);
        #endregion

        // Checks following criteria to determine if the player should be able to control their weapon:
        // If the player is not currently switching weapon or firing mode
        // If the player's weapon selector is not active
        if (isSwitchingWeapon == false && isSwitchingFireMode == false && playerHolding.weaponSelector.MenuIsActive() == false)
        {
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

                if (i > firingModes.Length - 1)
                {
                    i = 0;
                }
                else if (i < 0)
                {
                    i = firingModes.Length - 1;
                }

                SwitchWeaponMode(i);
            }

            fireControls.fireTimer += Time.deltaTime;

            // If player is active
            // If player is pressing fire button
            // If fireTimer has finished
            // If burst count has not exceeded the limit OR burst count is set to zero
            // If ammo is available OR supply is null
            // If magazine is not empty OR null
            if (playerHolding.ph.CurrentState() == PlayerState.Active && /*playerHolding.ph.isActive == true && */Input.GetButton("Fire") && fireControls.fireTimer >= 60 / fireControls.roundsPerMinute && (fireControls.burstCounter < fireControls.maxBurst || fireControls.maxBurst <= 0) && (ammunition == null || (playerHolding.ph.a.GetStock(ammunition.ammoType) >= ammunition.ammoPerShot)) && (magazine == null || (magazine.magazine.current >= 1/*ammoPerShot*/ && isReloading == false)))
            {
                // Adjust fire control variables
                fireControls.fireTimer = 0; // Reset fire timer to count up to next shot
                fireControls.burstCounter += 1;

                #region Alter ammunition, magazine and recoil variables if present
                // Consume ammo if supply is present
                if (ammunition != null)
                {
                    playerHolding.ph.a.Spend(ammunition.ammoType, ammunition.ammoPerShot);
                }

                // Deplete magazine if present
                if (magazine != null)
                {
                    if (ammunition != null) // If ammunition supply is present, consume appropriate amount of 
                    {
                        magazine.magazine.current -= ammunition.ammoPerShot;
                    }
                    else
                    {
                        magazine.magazine.current -= 1;
                    }
                }

                // Apply recoil if recoil stat exists
                if (recoil != null)
                {
                    recoilToApply += recoil.recoil;
                }
                #endregion

                #region Trigger cosmetic effects

                /*
                firingModes[firingModeIndex].muzzleFlash.Play();
                weaponSoundSource.PlayOneShot(firingModes[firingModeIndex].firingNoise);
                firingModes[firingModeIndex].shellEjection.Play();
                */

                MuzzleFlashEffect m = firingModes[firingModeIndex].muzzleFlash;
                if (m != null)
                {
                    m.Play();
                }

                AudioClip a = firingModes[firingModeIndex].firingNoise;
                if (a != null)
                {
                    weaponSoundSource.PlayOneShot(a);
                }

                ParticleSystem s = firingModes[firingModeIndex].shellEjection;
                if (s != null)
                {
                    s.Play();
                }
                #endregion

                // Calculate direction to shoot in
                //Quaternion ar = Quaternion.Euler(Random.Range(-playerHolding.standingAccuracy, playerHolding.standingAccuracy), Random.Range(-playerHolding.standingAccuracy, playerHolding.standingAccuracy), Random.Range(-playerHolding.standingAccuracy, playerHolding.standingAccuracy));
                //Vector3 aimDirection = ar * transform.forward;

                Vector3 aimDirection = transform.forward;
                if (optics == null || isAiming == false)
                {
                    aimDirection = Quaternion.Euler(Random.Range(-playerHolding.standingAccuracy, playerHolding.standingAccuracy), Random.Range(-playerHolding.standingAccuracy, playerHolding.standingAccuracy), Random.Range(-playerHolding.standingAccuracy, playerHolding.standingAccuracy)) * aimDirection;
                }

                for (int i = 0; i < projectile.projectileCount; i++) // Shoots an amount of projectiles based on the projectileCount variable.
                {
                    Damage.ShootProjectile(projectile.projectile, accuracy.projectileSpread, accuracy.range, playerHolding.gameObject, playerHolding.ph.faction, transform, projectile.muzzle, aimDirection);
                }
            }
            else if (!Input.GetButton("Fire"))
            {
                fireControls.burstCounter = 0;
            }

            if (optics != null)
            {
                AimHandler(optics, firingModes[firingModeIndex].heldPosition, playerHolding.toggleAim);
            }

            if (magazine != null)
            {
                if (ammunition != null)
                {
                    ReloadHandler(magazine.reloadTime, fireControls.fireTimer, fireControls.roundsPerMinute, magazine.roundsReloaded, magazine.magazine, playerHolding, ammunition.ammoType);
                }
                else
                {
                    ReloadHandler(magazine.reloadTime, fireControls.fireTimer, fireControls.roundsPerMinute, magazine.roundsReloaded, magazine.magazine);
                }
            }
        }

        if (recoil != null)
        {
            RecoilHandler(recoil.recoilApplyRate, playerHolding);
        }
    }
    
    private void LateUpdate()
    {
        MoveWeaponHandler();
    }

    #region Weapon functions

    #region Switching functions
    public void SwitchWeaponMode(int index)
    {
        // Check if the weapon being switched to has different optics, and cancel out if so.
        OpticsStats newOptics = GetOpticsStats(index);
        if (optics != null && (newOptics == null || newOptics != optics))
        {
            isAiming = false;
            zoomTimer = 0;
            LerpSights(optics, 0, firingModes[firingModeIndex].heldPosition);
        }

        if (magazine != null)
        {
            reloadTimer = 0;
            isReloading = false;
            print("Reload sequence cancelled");
        }

        firingModeIndex = index;
    }

    public IEnumerator Draw()
    {
        isSwitchingWeapon = true;

        gameObject.SetActive(true);
        // Draw weapon animation
        weaponModel.transform.SetPositionAndRotation(holsterPosition.position, holsterPosition.rotation);
        UpdateWeaponTransform(firingModes[firingModeIndex].heldPosition, Vector3.Distance(holsterPosition.position, firingModes[firingModeIndex].heldPosition.position) / switchSpeed, true);

        yield return new WaitForSeconds(switchSpeed);
        isSwitchingWeapon = false;
    }

    public IEnumerator Holster()
    {
        isSwitchingWeapon = true;

        #region Cancel running weapon functions
        if (optics != null) // DOES NOT WORK PROPERLY
        {
            isAiming = false;
            zoomTimer = 0;
            LerpSights(optics, 0, firingModes[firingModeIndex].heldPosition);
        }

        if (magazine != null)
        {
            CancelReload();
        }
        #endregion

        // Begin holster animation
        weaponModel.transform.SetPositionAndRotation(firingModes[firingModeIndex].heldPosition.position, firingModes[firingModeIndex].heldPosition.rotation);
        UpdateWeaponTransform(holsterPosition, Vector3.Distance(firingModes[firingModeIndex].heldPosition.position, holsterPosition.position) / switchSpeed, true);

        yield return new WaitForSeconds(switchSpeed);

        gameObject.SetActive(false);
        print("Weapon holstered");
        isSwitchingWeapon = false;
    }
    
    #endregion

    #region Move weapon model

    void ResetWeaponMoveVariables()
    {
        if (oldWeaponTransform == null)
        {
            oldWeaponTransform = transform;
        }
        if (newWeaponTransform == null)
        {
            newWeaponTransform = firingModes[firingModeIndex].heldPosition;
        }
        if (previousNewWeaponTransform == null)
        {
            previousNewWeaponTransform = newWeaponTransform;
        }
        if (moveWeaponTime <= 0)
        {
            moveWeaponTime = 1;
            moveWeaponTimer = 1;
        }
    }

    void UpdateWeaponTransform(Transform newTransform, float moveSpeed, bool oneShot)
    {
        newWeaponTransform = newTransform;

        if (newWeaponTransform != previousNewWeaponTransform || oneShot == true)
        {
            oldWeaponTransform = weaponModel.transform;
            moveWeaponTime = Vector3.Distance(weaponModel.transform.position, newWeaponTransform.position) / moveSpeed; // Calculates time taken for the weapon to move the appropriate distance at the desired speed
            moveWeaponTimer = 0;
            previousNewWeaponTransform = newWeaponTransform;
        }
    }

    void MoveWeaponHandler()
    {
        if (Time.timeScale != 0) // Checks if time is moving in either direction, otherwise model will move even when not paused
        {
            moveWeaponTimer += Time.deltaTime / moveWeaponTime;
            moveWeaponTimer = Mathf.Clamp01(moveWeaponTimer);
            // Moves weapon position via lerping. Should I change this to an animation?
            Vector3 currentWeaponPosition = Vector3.Lerp(oldWeaponTransform.position, newWeaponTransform.position, moveWeaponTimer);
            Quaternion currentWeaponRotation = Quaternion.Lerp(oldWeaponTransform.rotation, newWeaponTransform.rotation, moveWeaponTimer);
            weaponModel.transform.SetPositionAndRotation(currentWeaponPosition, currentWeaponRotation);
        }
        
    }
    #endregion
    
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
    #endregion

    #region ADS functions

    public void AimHandler(OpticsStats os, Transform hipPosition, bool toggleAim)
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

    void LerpSights(OpticsStats os, float timer, Transform newWeaponPosition)
    {
        
        // Reduces FOV to zoom in camera
        zoomVariable = Mathf.Lerp(1, 1 / os.magnification, timer);
        playerHolding.ph.pc.playerCamera.fieldOfView = playerHolding.ph.pc.fieldOfView * zoomVariable;
        
        // Reduce sensitivity
        float newSensitivity = Mathf.Lerp(0, -1 + (1 / os.magnification), timer);
        playerHolding.ph.pc.sensitivityModifier.ApplyEffect("Aiming down sights", newSensitivity, Time.deltaTime);

        
        // Reduce movement speed
        float newSpeed = Mathf.Lerp(0, os.moveSpeedReduction, timer);
        playerHolding.ph.pc.speedModifier.ApplyEffect("Aiming down sights", newSpeed, Time.deltaTime);

        // Alter accuracy if specified
        

        // Move weapon model
        UpdateWeaponTransform(newWeaponPosition, Vector3.Distance(weaponModel.transform.position, newWeaponPosition.position) / os.transitionTime, false);

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
        print("Reload sequence ended");
    }
    #endregion

    #endregion
}
