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

    public Transform muzzle;

    public Projectile projectile;
    public int projectileCount;
    public int damage;
    public float velocity;
    public float diameter;
    public float gravityMultiplier;
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

    [Header("Firing modes")]
    public FiringMode[] firingModes;

    [Header("Universal variables")]
    public int firingModeIndex;
    public GameObject weaponModel;
    public AudioSource weaponSoundSource;
    public Transform holsterPosition;

    [HideInInspector] public FireControlStats fireControls;
    [HideInInspector] public AccuracyStats accuracy;
    [HideInInspector] public ProjectileStats projectile;
    [HideInInspector] public OpticsStats optics;
    [HideInInspector] public RecoilStats recoil;
    [HideInInspector] public AmmunitionStats ammunition;
    [HideInInspector] public MagazineStats magazine;


    #region Universal variables (variables that can be used multiple times for different firing modes)
    // Firing weapon
    [HideInInspector] public RaycastHit targetFound;
    [HideInInspector] public Vector3 aimDirection;
    [HideInInspector] public Vector3 target;
    // Applying recoil
    [HideInInspector] public float recoilToApply;
    [HideInInspector] public Vector2 aimOrigin;
    [HideInInspector] public Vector2 aimCurrent;
    // Aiming down sights
    [HideInInspector] public bool isAiming;
    [HideInInspector] public float zoomVariable;
    [HideInInspector] public float zoomTimer;
    // Reloading weapon
    [HideInInspector] public bool isReloading;
    [HideInInspector] public float reloadTimer;
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
    OpticsStats GetStats(OpticsStats[] o, FiringMode f)
    {
        if (f.opticsMode <= -1 || o.Length <= 0)
        {
            return null;
        }
        return o[f.opticsMode];
    }

    RecoilStats GetStats(RecoilStats[] r, FiringMode f)
    {
        if (f.recoilMode <= -1 || r.Length <= 0)
        {
            return null;
        }
        return r[f.recoilMode];
    }

    AmmunitionStats GetStats(AmmunitionStats[] a, FiringMode f)
    {
        if (f.ammunitionMode <= -1 || a.Length <= 0)
        {
            return null;
        }
        return a[f.ammunitionMode];
    }

    MagazineStats GetStats(MagazineStats[] m, FiringMode f)
    {
        if (f.magazineMode <= -1 || m.Length <= 0)
        {
            return null;
        }
        return m[f.magazineMode];
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
        optics = GetStats(opticsModes, firingModes[firingModeIndex]);
        recoil = GetStats(recoilModes, firingModes[firingModeIndex]);
        ammunition = GetStats(ammunitionModes, firingModes[firingModeIndex]);
        magazine = GetStats(magazineModes, firingModes[firingModeIndex]);

        #endregion

        foreach (FiringMode f in firingModes)
        {
            fireControlModes[f.fireControlMode].fireTimer += Time.deltaTime;
        }

        // If player is active
        // If player is pressing fire button
        // If fireTimer has finished
        // If burst count has not exceeded the limit OR burst count is set to zero
        // If ammo is available OR supply is null
        // If magazine is not empty OR null
        if (playerHolding.ph.isActive == true && Input.GetButton("Fire") && fireControls.fireTimer >= 60 / fireControls.roundsPerMinute && (fireControls.burstCounter < fireControls.maxBurst || fireControls.maxBurst <= 0) && (ammunition == null || (playerHolding.ph.a.GetStock(ammunition.ammoType) >= ammunition.ammoPerShot)) && (magazine == null || (magazine.magazine.current >= 1/*ammoPerShot*/ && isReloading == false)))
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
            MuzzleFlashEffect m = firingModes[firingModeIndex].muzzleFlash;
            if (m != null)
            {
                //m.Play(60 / fireControls.roundsPerMinute * firingModes[firingModeIndex].muzzleFlashRelativeDuration);
                m.Restart(60 / fireControls.roundsPerMinute * firingModes[firingModeIndex].muzzleFlashRelativeDuration);
            }

            AudioClip a = firingModes[firingModeIndex].firingNoise;
            if (a != null)
            {
                weaponSoundSource.clip = a;
                weaponSoundSource.Play();
                //AudioSource.PlayClipAtPoint(a, weaponModel.transform.position);
            }

            ParticleSystem s = firingModes[firingModeIndex].shellEjection;
            if (s != null)
            {
                s.Play();
            }
            #endregion



            // Calculate direction to shoot in
            Quaternion ar = Quaternion.Euler(Random.Range(-playerHolding.standingAccuracy, playerHolding.standingAccuracy), Random.Range(-playerHolding.standingAccuracy, playerHolding.standingAccuracy), Random.Range(-playerHolding.standingAccuracy, playerHolding.standingAccuracy));
            Vector3 aimDirection = ar * transform.forward;

            for (int i = 0; i < projectile.projectileCount; i++) // Shoots an amount of projectiles based on the projectileCount variable.
            {
                LaunchProjectile(aimDirection, targetFound, accuracy.rayDetection, accuracy.projectileSpread, accuracy.range, projectile.muzzle, projectile.projectile, projectile.velocity, projectile.gravityMultiplier, projectile.diameter);
            }
        }
        else if (!Input.GetButton("Fire"))
        {
            fireControls.burstCounter = 0;
        }
        
        if (optics != null)
        {
            AimHandler(optics.magnification, optics.moveSpeedReduction, optics.transitionTime, firingModes[firingModeIndex].heldPosition, optics.aimPosition, playerHolding.toggleAim);
        }
        

        if (recoil != null)
        {
            RecoilHandler(recoil.recoilApplyRate, playerHolding);
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

    private void LateUpdate()
    {
        MoveWeaponHandler();
    }

    public void SwitchWeaponMode(int modeIndex)
    {
        firingModeIndex = modeIndex;
    }


    #region Weapon functions

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

    void UpdateWeaponTransform(Transform newTransform, float moveSpeed)
    {
        newWeaponTransform = newTransform;

        if (newWeaponTransform != previousNewWeaponTransform)
        {
            oldWeaponTransform = weaponModel.transform;
            moveWeaponTime = Vector3.Distance(weaponModel.transform.position, newWeaponTransform.position) / moveSpeed; // Calculates time taken for the weapon to move the appropriate distance at the desired speed
            moveWeaponTimer = 0;
            previousNewWeaponTransform = newWeaponTransform;
        }
    }

    void MoveWeaponHandler()
    {
        moveWeaponTimer += Time.deltaTime / moveWeaponTime;
        moveWeaponTimer = Mathf.Clamp01(moveWeaponTimer);
        // Moves weapon position via lerping. Should I change this to an animation?
        Vector3 currentWeaponPosition = Vector3.Lerp(oldWeaponTransform.position, newWeaponTransform.position, moveWeaponTimer);
        Quaternion currentWeaponRotation = Quaternion.Lerp(oldWeaponTransform.rotation, newWeaponTransform.rotation, moveWeaponTimer);
        weaponModel.transform.SetPositionAndRotation(currentWeaponPosition, currentWeaponRotation);
    }
    #endregion

    #region Firing functions
    public void LaunchProjectile(Vector3 direction, RaycastHit target, LayerMask rayDetection, float spread, float range, Transform muzzle, Projectile projectile, float velocity, float gravityMultiplier, float diameter)
    {
        Vector3 destination = Quaternion.Euler(Random.Range(-spread, spread), Random.Range(-spread, spread), Random.Range(-spread, spread)) * direction;
        //Ray targetRay = new Ray(transform.position, destination);
        if (Physics.Raycast(transform.position, destination, out target, range, rayDetection)) // To reduce the amount of superfluous variables, I re-used the 'target' Vector3 in the same function as it is now unneeded for its original purpose
        {
            destination = target.point;
        }
        else
        {
            destination *= range;
        }

        

        Instantiate(projectile.gameObject, muzzle.position, Quaternion.LookRotation(destination - muzzle.position, Vector3.up));
        projectile.velocity = velocity;
        projectile.gravityMultiplier = gravityMultiplier;
        projectile.diameter = diameter;
        projectile.targetDetection = rayDetection;
        projectile.origin = playerHolding.gameObject;
        // HAVE MORE STUFF HERE FOR DETERMINING TYPE OF PROJECTILE AND ASSIGNING APPROPRIATE VARIABLES. HOW DO I DO THIS?
    }

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
    }
    #endregion

    #region ADS functions
    public void AimHandler(float magnification, float moveSpeedReduction, float zoomTime, Transform hipPosition, Transform aimPosition, bool toggleAim)
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
            UpdateWeaponTransform(aimPosition, Vector3.Distance(weaponModel.transform.position, aimPosition.position) / zoomTime);
            zoomTimer += Time.deltaTime / zoomTime;
        }
        else
        {
            UpdateWeaponTransform(hipPosition, Vector3.Distance(weaponModel.transform.position, hipPosition.position) / zoomTime);
            zoomTimer -= Time.deltaTime / zoomTime;
        }
        zoomTimer = Mathf.Clamp01(zoomTimer);
        LerpSights(magnification, moveSpeedReduction, zoomTimer, hipPosition, aimPosition);
    }

    void LerpSights(float magnification, float moveSpeedReduction, float timer, Transform hipPosition, Transform aimPosition)
    {
        // Reduces FOV to zoom in camera
        zoomVariable = Mathf.Lerp(1, 1 / magnification, timer);
        playerHolding.ph.pc.playerCamera.fieldOfView = playerHolding.ph.pc.fieldOfView * zoomVariable;

        /* // Moves weapon position via lerping. This is obsolete
        Vector3 currentWeaponPosition = Vector3.Lerp(hipPosition.position, aimPosition.position, timer);
        Quaternion currentWeaponRotation = Quaternion.Lerp(hipPosition.rotation, aimPosition.rotation, timer);
        weaponModel.transform.SetPositionAndRotation(currentWeaponPosition, currentWeaponRotation);
        */

        // Reduce sensitivity
        float newSensitivity = Mathf.Lerp(0, -1 + (1 / magnification), timer);
        playerHolding.ph.pc.sensitivityModifier.ApplyEffect("Aiming down sights", newSensitivity, 0);

        // Reduce movement speed
        float newSpeed = Mathf.Lerp(0, moveSpeedReduction, timer);
        playerHolding.ph.pc.speedModifier.ApplyEffect("Aiming down sights", newSpeed, 0);

        // Alter accuracy if specified
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

            //ExecuteReload(reloadTimer, isReloading);
        }
        if (isReloading == true)
        {
            reloadTimer += Time.deltaTime;

            // If magazine is full, there is no more ammunition, or reload is interrupted by another action
            if (isReloading == true && (magazine.current >= magazine.max || remainingAmmo <= 0 || (Input.GetButtonDown("Fire") && magazine.current > 0))) // Also include button options for melee attacking and any other functions that would cancel out the reload function
            {
                reloadTimer = 0;
                isReloading = false;
                print("Reload sequence finished");
            }

            if (reloadTimer >= reloadTime) // If reload time has been reached, reload ammunition into magazine
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
                reloadTimer = 0; // Reset reload timer

                // If magazine is full, there is no more ammunition, or reload is interrupted by another action
                if (magazine.current >= magazine.max || remainingAmmo <= 0) // Also include button options for melee attacking and any other functions that would cancel out the reload function
                {
                    isReloading = false;
                    print("Reload sequence finished");
                }
                else
                {
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

            //ExecuteReload(reloadTimer, isReloading);
        }
        if (isReloading == true)
        {
            reloadTimer += Time.deltaTime;

            // If magazine is full, there is no more ammunition, or reload is interrupted by another action
            if (isReloading == true && (magazine.current >= magazine.max || (Input.GetButtonDown("Fire") && magazine.current > 0))) // Also include button options for melee attacking and any other functions that would cancel out the reload function
            {
                reloadTimer = 0;
                isReloading = false;
                print("Reload sequence finished");
            }

            if (reloadTimer >= reloadTime) // If reload time has been reached, reload ammunition into magazine
            {
                print("Ammo reloaded");
                magazine.current += roundsReloaded; // Reload standard amount of ammunition per reload cycle
                magazine.current = Mathf.Clamp(magazine.current, 0, magazine.max); // Ensure magazine is not overloaded
                reloadTimer = 0; // Reset reload timer

                // If magazine is full, there is no more ammunition, or reload is interrupted by another action
                if (magazine.current >= magazine.max) // Also include button options for melee attacking and any other functions that would cancel out the reload function
                {
                    isReloading = false;
                    print("Reload sequence finished");
                }
                else
                {
                    print("Reload sequence continued");
                }
            }
        }
    }
    #endregion

    #endregion
}
