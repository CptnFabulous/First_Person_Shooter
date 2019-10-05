using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProjectileWeapon : Weapon
{
    #region Public variables
    [Header("Accuracy")]
    [Tooltip("Level of deviation in weapon accuracy from centre of reticle, in degrees.")]
    [Range(0, 180)] public float projectileSpread = 5;
    [Tooltip("Maximum range for the gun's raycast check to determine where to launch projectiles. Decrease this value if the weapon is not meant to hit accurately past a certain point.")]
    public float range = 500;
    [Tooltip("What layers will the raycast check and launched projectiles register?")]
    public LayerMask rayDetection;

    [Header("Recoil")]
    [Tooltip("Amount of recoil applied per shot.")]
    public float recoil = 10;
    [Tooltip("Rate at which recoil is applied to aim.")]
    public float recoilApplyRate = 100;
    [Tooltip("Speed at which camera returns to starting position.")]
    public float recoilRecovery = 10;

    [Header("Optics")]
    public float magnification = 4;
    public float zoomTime = 0.25f;
    [Range(-1, 0)] public float moveSpeedReduction = -0.5f;
    public Transform hipPosition;
    public Transform aimPosition;

    [Header("Fire Rate")]
    [Tooltip("Amount of projectiles launched per shot. Set to 1 for regular bullet-shooting weapons, increase for weapons such as shotguns.")]
    [Min(1)] public int projectileCount = 1;
    [Tooltip("Cyclic fire rate, in rounds per minute.")]
    public float roundsPerMinute = 600;
    [Tooltip("Amount of shots that can be fired before needing to re-press the trigger. Set to 1 for semi-automatic, or more for burst-fire weapons. Set to zero to enable full-auto fire.")]
    public int burstCount;

    [Header("Ammunition")]
    [Tooltip("The type of ammunition used by the weapon.")]
    public AmmunitionType caliber;
    [Tooltip("How many units of ammunition are consumed per shot.")]
    public int ammoPerShot = 1;
    [Tooltip("Sets weapon's magazine capacity. Set to zero to ignore magazine code and fire continuously without reloading.")]
    [Min(0)] public int magazineCapacity = 30;
    [Tooltip("Amount of ammunition currently in the weapon's magazine.")]
    public int roundsInMagazine = 30;
    [Tooltip("Time taken to reload the weapon, in seconds.")]
    public float reloadTime = 2;
    [Tooltip("Amount of rounds reloaded at a time.")]
    public int roundsReloaded = 30;
    #endregion

    #region Private variables
    // Firing weapon
    RaycastHit targetFound;
    Vector3 aimDirection;
    [HideInInspector] public Vector3 target;

    // Applying recoil
    float recoilToApply;
    Vector2 aimOrigin;
    Vector2 aimCurrent;


    // Aiming down sights
    public bool toggleAim;
    public bool isAiming;
    [HideInInspector]
    public float zoomVariable;
    float zoomTimer;

    // Fire rate
    float fireTimer = float.MaxValue;
    float shotsInBurst;

    // Reloading weapon
    bool isReloading;
    float reloadTimer;
    #endregion

#if UNITY_EDITOR
    void Reset() { OnValidate(); }
    void OnValidate()
    {
        magazineCapacity = Mathf.Clamp(magazineCapacity, 0, int.MaxValue);
        roundsInMagazine = Mathf.Clamp(roundsInMagazine, 0, magazineCapacity);
        roundsReloaded = Mathf.Clamp(roundsReloaded, 1, magazineCapacity);
        moveSpeedReduction = Mathf.Clamp(moveSpeedReduction, -1, 0);
        //ammoPerShot = Mathf.Clamp(ammoPerShot, 1, magazineCapacity);

        if (isAiming == true)
        {
            weaponModel.transform.SetPositionAndRotation(aimPosition.position, aimPosition.rotation);
        }
        else
        {
            weaponModel.transform.SetPositionAndRotation(hipPosition.position, hipPosition.rotation);
        }

    }
#endif

    // Start is called before the first frame update
    void Start()
    {
        type = WeaponType.projectile;
    }

    // Update is called once per frame
    public virtual void Update()
    {
        #region ADS controls
        HoldOrToggleAim();
        if (isAiming)
        {
            LerpSights(zoomTime);
        }
        else
        {
            LerpSights(-zoomTime);
        }
        // LERP CODE IS SCREWY AND DOESN'T ROUND PROPERLY
        #endregion

        #region Fire Controls
        fireTimer += Time.deltaTime; // Counts up timer for next shot.
        // If fire button is pressed, previous shot has finished firing, maximum burst count has not been exceeded (or no burst function is present), ammunition is present and ammo remains in magazine (or if gun does not need reloading)


        

        if (playerHolding.ph.isActive == true && Input.GetButton("Fire") && fireTimer >= 60 / roundsPerMinute && (shotsInBurst < burstCount || burstCount <= 0) && playerHolding.ph.a.GetStock(caliber) >= ammoPerShot && ((roundsInMagazine >= ammoPerShot && isReloading == false) || magazineCapacity <= 0))
        {
            /*
            if (Input.GetButtonDown("Fire"))
            {
                aimOrigin = new Vector2(playerHolding.pc.transform.rotation.y, playerHolding.pc.head.transform.localRotation.x);
                print(aimOrigin);
            }
            */
            Shoot();
        }
        else if (Input.GetButtonUp("Fire") && burstCount > 0)
        {
            shotsInBurst = 0;
        }
        #endregion

        #region Reload controls
        // If reload button is pressed and weapon's magazine is not full OR if magazine is empty and gun is finished firing
        if (((Input.GetButtonDown("Reload") && roundsInMagazine < magazineCapacity) || (roundsInMagazine <= 0 && fireTimer >= 60 / roundsPerMinute && isReloading == false)) && playerHolding.ph.a.GetStock(caliber) > 0)
        {
            ExecuteReload();
        }
        if (isReloading == true)
        {
            ReloadSequence();
        }
        #endregion

        Recoil();
    }

    #region ADS functions
    void HoldOrToggleAim()
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
    }

    void LerpSights(float t)
    {
        //Sets timer value to specify lerping of variables
        zoomTimer += Time.deltaTime / t;
        zoomTimer = Mathf.Clamp01(zoomTimer);

        // Reduces FOV to zoom in camera
        zoomVariable = Mathf.Lerp(1, 1 / magnification, zoomTimer);
        playerHolding.ph.pc.playerCamera.fieldOfView = playerHolding.ph.pc.fieldOfView * zoomVariable;
        //zoomVariable = Mathf.Lerp(1, magnification, zoomTimer);
        //playerHolding.pc.playerCamera.fieldOfView = playerHolding.pc.fieldOfView / zoomVariable;

        // Moves weapon position via lerping. Should I change this to an animation?
        Vector3 currentWeaponPosition = Vector3.Lerp(hipPosition.position, aimPosition.position, zoomTimer);
        Quaternion currentWeaponRotation = Quaternion.Lerp(hipPosition.rotation, aimPosition.rotation, zoomTimer);
        weaponModel.transform.SetPositionAndRotation(currentWeaponPosition, currentWeaponRotation);
        
        // Reduce sensitivity
        float newSensitivity = Mathf.Lerp(0, -1 + (1 / magnification), zoomTimer);
        //ModifyStat.ApplyEffect(playerHolding.ph.pc.sensitivityModifier, "Aiming down sights", newSensitivity, Time.deltaTime);
        playerHolding.ph.pc.sensitivityModifier.ApplyEffect("Aiming down sights", newSensitivity, Time.deltaTime);

        // Reduce movement speed
        float newSpeed = Mathf.Lerp(0, moveSpeedReduction, zoomTimer);
        playerHolding.ph.pc.speedModifier.ApplyEffect("Aiming down sights", newSpeed, Time.deltaTime);

        // Alter accuracy if specified
    }
    #endregion

    #region Firing functions
    public virtual void Shoot()
    {
        // Modifies player's aim based on accuracy stat. This Vector3 is currently unused.
        Quaternion ar = Quaternion.Euler(Random.Range(-playerHolding.standingAccuracy, playerHolding.standingAccuracy), Random.Range(-playerHolding.standingAccuracy, playerHolding.standingAccuracy), Random.Range(-playerHolding.standingAccuracy, playerHolding.standingAccuracy));
        aimDirection = ar * transform.forward;
        for (int i = 0; i < projectileCount; i++) // Shoots an amount of projectiles based on the projectileCount variable.
        {
            LaunchProjectile();
        }
        if (burstCount > 0) // If the weapon fires in bursts
        {
            shotsInBurst += 1; // Adds number to burst count
        }
        if (magazineCapacity > 0) // If weapon has reloadable magazine
        {
            roundsInMagazine -= ammoPerShot; // Subtract ammunition from weapon magazine
        }
        playerHolding.ph.a.Spend(caliber, ammoPerShot); // Spends appropriate ammunition type
        fireTimer = 0; // Reset fire timer to count up to next shot
        recoilToApply += recoil; // Adds recoil to total amount needed to be applied to player

        // Cosmetic effects are done in another derived class, for different cosmetic effects.
    }

    public virtual void LaunchProjectile()
    {
        target = Quaternion.Euler(Random.Range(-projectileSpread, projectileSpread), Random.Range(-projectileSpread, projectileSpread), Random.Range(-projectileSpread, projectileSpread)) * aimDirection;
        Ray targetRay = new Ray(transform.position, target);

        //targetRay.direction = Quaternion.Euler(Random.Range(-projectileSpread, projectileSpread), Random.Range(-projectileSpread, projectileSpread), Random.Range(-projectileSpread, projectileSpread)) * transform.forward;

        if (Physics.Raycast(targetRay, out targetFound, range, rayDetection))
        {
            target = targetFound.point;
        }
        else
        {
            target = targetRay.direction * range;
        }
        // Instantiating of projectile is done in another derived class, so different kinds of projectiles can be instantiated
    }

    public void Recoil()
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

    #region Reload functions
    public virtual void ExecuteReload()
    {
        reloadTimer = 0;
        isReloading = true;
        print("Reload function started");
        // Do other reloading stuff here, like playing reloading animation
    }

    public virtual void ReloadSequence()
    {
        reloadTimer += Time.deltaTime; // Timer counts up to ensure correct reload time has passed
        //print(reloadTimer);
        if (reloadTimer >= reloadTime) // If reload time has passed, reload gun
        {
            print("Gun reloaded");
            int remainingAmmo = playerHolding.ph.a.GetStock(caliber) - roundsInMagazine; // Checks how much spare ammunition the player has
            if (remainingAmmo < roundsReloaded) // If there is not enough ammunition to reload the usual amount
            {
                roundsInMagazine += remainingAmmo; // Reload all remaining ammunition
            }
            else
            {
                roundsInMagazine += roundsReloaded; // Reload standard amount of ammunition per reload cycle
            }
            roundsInMagazine = Mathf.Clamp(roundsInMagazine, 0, magazineCapacity); // Ensure magazine is not overloaded

            ExecuteReload(); // Start reload again. This function will cycle continuously for weapons that reload ammunition in 'batches' rather than all at once, e.g. manually loading firearms
        }

        // If magazine is full, there is no more ammunition, or reload is interrupted by another action
        if (roundsInMagazine >= magazineCapacity || playerHolding.ph.a.GetStock(caliber) - roundsInMagazine <= 0 || (Input.GetButtonDown("Fire") && roundsInMagazine > 0)) // Also include button options for melee attacking and any other functions that would cancel out the reload function
        {
            // Cancel reload
            reloadTimer = 0;
            isReloading = false;
        }
    }
    #endregion
}