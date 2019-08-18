using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProjectileWeapon : Weapon
{
    [Header("Accuracy")]
    [Tooltip("Level of deviation in weapon accuracy from centre of reticle, in degrees.")]
    [Range(0, 180)] public float projectileSpread = 5;
    [Tooltip("Amount of recoil applied per shot.")]
    public float recoil = 10;
    [Tooltip("Speed at which camera returns to starting position.")]
    public float recoilRecovery = 10;
    [Tooltip("Maximum range for the gun's raycast check to determine where to launch projectiles. Decrease this value if the weapon is not meant to hit accurately past a certain point.")]
    public float range = 500;
    [Tooltip("What layers will the raycast check and launched projectiles register?")]
    public LayerMask rayDetection;
    Ray targetRay;
    RaycastHit targetFound;
    [HideInInspector] public Vector3 target;

    [Header("Fire Rate")]
    [Tooltip("Amount of projectiles launched per shot. Set to 1 for regular bullet-shooting weapons, increase for weapons such as shotguns.")]
    [Min(1)] public int projectileCount = 1;
    [Tooltip("Cyclic fire rate, in rounds per minute.")]
    public float roundsPerMinute = 600;
    [Tooltip("Amount of shots that can be fired before needing to re-press the trigger. Set to 1 for semi-automatic, or more for burst-fire weapons. Set to zero to enable full-auto fire.")]
    public int burstCount;
    float fireTimer = 9999999999;
    float shotsInBurst;

    [Header("Ammunition")]
    [Tooltip("The player's ammunition supply.")]
    public Ammunition ammoSupply; // The ammunition supply belonging to the player shooting the gun
    [Tooltip("The type of ammunition used by the weapon.")]
    public AmmunitionType caliber;
    [Tooltip("How many units of ammunition are consumed per shot.")]
    public int ammoPerShot = 1;
    [Tooltip("Sets weapon's magazine capacity. Set to zero to ignore magazine code, allowing the player to fire continuously without reloading.")]
    [Min(0)] public int magazineCapacity = 30;
    [Tooltip("Amount of ammunition currently in the weapon's magazine.")]
    public int roundsInMagazine = 30;
    [Tooltip("Time taken to reload the weapon, in seconds.")]
    public float reloadTime = 2;
    [Tooltip("Amount of rounds reloaded at a time. Set to the weapon's magazine capacity for weapons that reload from a single magazine, set to one or more for weapons that load small amounts of ammunition at a time such as fixed-magazine shotguns.")]
    public int roundsReloaded = 30;
    bool isReloading;
    float reloadTimer;


#if UNITY_EDITOR
    void Reset() { OnValidate(); }
    void OnValidate()
    {
        //magazineCapacity = Mathf.Clamp(magazineCapacity, 0, Mathf.Infinity);
        roundsInMagazine = Mathf.Clamp(roundsInMagazine, 0, magazineCapacity);
        roundsReloaded = Mathf.Clamp(roundsReloaded, 1, magazineCapacity);
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
        if (fireTimer < 60 / roundsPerMinute)
        {
            fireTimer += Time.deltaTime; // Counts up timer for next shot. This doesn't really need to be inside an if statement but whatever. It may benefit to have this be outside an if statement if for whatever reason you want to calculate how long it's been since the weapon was last fired.
        }

        #region Fire Controls
        // If fire button is pressed, previous shot has finished firing, maximum burst count has not been exceeded (or no burst function is present), ammunition is present and ammo remains in magazine (or if gun does not need reloading)
        if (Input.GetButton("MouseLeft") && fireTimer >= 60 / roundsPerMinute && (shotsInBurst < burstCount || burstCount <= 0) && ammoSupply.GetStock(caliber) > 0 && ((roundsInMagazine > 0 && isReloading == false) || magazineCapacity <= 0))
        {
            Shoot();
        }
        else if (Input.GetButtonUp("MouseLeft") && burstCount > 0)
        {
            shotsInBurst = 0;
        }
        #endregion

        #region Reloading
        // If reload button is pressed and weapon's magazine is not full OR if magazine is empty and gun is finished firing
        if (((Input.GetButtonDown("Reload") && roundsInMagazine < magazineCapacity) || (roundsInMagazine <= 0 && fireTimer >= 60 / roundsPerMinute && isReloading == false)) && ammoSupply.GetStock(caliber) > 0)
        {
            ExecuteReload();
        }
        if (isReloading == true)
        {
            ReloadSequence();
        }
        #endregion
    }

    /*
    public virtual void Reload()
    {
        reloadTimer += Time.deltaTime;
        if (reloadTimer >= reloadTime)
        {
            if (ammoSupply.GetStock(caliber) < magazineCapacity)
            {
                roundsInMagazine = ammoSupply.GetStock(caliber);
            }
            else
            {
                roundsInMagazine = magazineCapacity;
            }
        }

        if (roundsInMagazine >= magazineCapacity || (Input.GetButtonDown("MouseLeft") && roundsInMagazine > 0)) // Also include button options for melee attacking and any other functions that would cancel out the reload function
        {
            reloadTimer = 0;
            isReloading = false;
        }
    }
    */
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
        print(reloadTimer);
        if (reloadTimer >= reloadTime) // If reload time has passed, reload gun
        {
            print("Gun reloaded");
            int remainingAmmo = ammoSupply.GetStock(caliber) - roundsInMagazine; // Checks how much spare ammunition the player has
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
        if (roundsInMagazine >= magazineCapacity || ammoSupply.GetStock(caliber) - roundsInMagazine <= 0 || (Input.GetButtonDown("MouseLeft") && roundsInMagazine > 0)) // Also include button options for melee attacking and any other functions that would cancel out the reload function
        {
            // Cancel reload
            reloadTimer = 0;
            isReloading = false;
        }
    }

    public virtual void Shoot()
    {
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
        ammoSupply.Spend(caliber, ammoPerShot); // Spends appropriate ammunition type
        fireTimer = 0; // Reset fire timer to count up to next shot
        // Cosmetic effects are done in another derived class, for different cosmetic effects.
    }

    public virtual void LaunchProjectile()
    {
        targetRay.origin = transform.position;
        targetRay.direction = Quaternion.Euler(Random.Range(-projectileSpread, projectileSpread), Random.Range(-projectileSpread, projectileSpread), Random.Range(-projectileSpread, projectileSpread)) * transform.forward;
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
}