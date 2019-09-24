using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RangedWeapon : MonoBehaviour
{
    
    public WeaponHandler playerHolding;

    #region Firing functions

    #region Firing controls
    public void FullAutoFireControls(float fireTimer, float roundsPerMinute)
    {
        fireTimer += Time.deltaTime; // Counts up timer for next shot.
        // If fire button is pressed, previous shot has finished firing, maximum burst count has not been exceeded (or no burst function is present), ammunition is present and ammo remains in magazine (or if gun does not need reloading)
        if (Input.GetButton("MouseLeft") && fireTimer >= 60 / roundsPerMinute)
        {
            //Shoot();
            fireTimer = 0; // Reset fire timer to count up to next shot
        }
    }

    public void SemiAutoFireControls(float fireTimer, float roundsPerMinute)
    {
        fireTimer += Time.deltaTime; // Counts up timer for next shot.
        // If fire button is pressed, previous shot has finished firing, maximum burst count has not been exceeded (or no burst function is present), ammunition is present and ammo remains in magazine (or if gun does not need reloading)
        if (Input.GetButtonDown("MouseLeft") && fireTimer >= 60 / roundsPerMinute)
        {
            //Shoot();
            fireTimer = 0; // Reset fire timer to count up to next shot
        }
    }

    public void BurstFireControls(float fireTimer, float roundsPerMinute, int burstCount, int shotsInBurst)
    {
        fireTimer += Time.deltaTime; // Counts up timer for next shot.
        // If fire button is pressed, previous shot has finished firing, maximum burst count has not been exceeded (or no burst function is present), ammunition is present and ammo remains in magazine (or if gun does not need reloading)
        if (Input.GetButton("MouseLeft") && fireTimer >= 60 / roundsPerMinute && shotsInBurst < burstCount)
        {
            //Shoot();
            fireTimer = 0; // Reset fire timer to count up to next shot
            shotsInBurst += 1;
        }
        else if (Input.GetButtonUp("MouseLeft"))
        {
            shotsInBurst = 0;
        }
    }
    #endregion




    public virtual void Shoot(AmmunitionType ammoType, int ammoPerShot, Resource magazine, int projectileCount, float recoil, float recoilToApply, bool isReloading)
    {
        if (playerHolding.ph.isActive == true && playerHolding.ph.a.GetStock(ammoType) >= ammoPerShot && ((magazine.current >= ammoPerShot && isReloading == false) || magazine.max <= 0))
        {
            // Modifies player's aim based on accuracy stat. This Vector3 is currently unused.
            Quaternion ar = Quaternion.Euler(Random.Range(-playerHolding.standingAccuracy, playerHolding.standingAccuracy), Random.Range(-playerHolding.standingAccuracy, playerHolding.standingAccuracy), Random.Range(-playerHolding.standingAccuracy, playerHolding.standingAccuracy));
            Vector3 aimDirection = ar * transform.forward;

            if (magazine.max > 0) // If weapon has reloadable magazine
            {
                magazine.current -= ammoPerShot; // Subtract ammunition from weapon magazine
            }
            playerHolding.ph.a.Spend(ammoType, ammoPerShot); // Spends appropriate ammunition type
            recoilToApply += recoil; // Adds recoil to total amount needed to be applied to player

            for (int i = 0; i < projectileCount; i++) // Shoots an amount of projectiles based on the projectileCount variable.
            {
                //LaunchProjectile(aimDirection);
            }
        }
    }

    public void LaunchProjectile(Vector3 aimDirection, RaycastHit targetFound, LayerMask rayDetection, float projectileSpread, float projectileRange)
    {
        Vector3 target = Quaternion.Euler(Random.Range(-projectileSpread, projectileSpread), Random.Range(-projectileSpread, projectileSpread), Random.Range(-projectileSpread, projectileSpread)) * aimDirection;
        Ray targetRay = new Ray(transform.position, target);
        if (Physics.Raycast(targetRay, out targetFound, projectileRange, rayDetection)) // To reduce the amount of superfluous variables, I re-used the 'target' Vector3 in the same function as it is now unneeded for its original purpose
        {
            target = targetFound.point;
        }
        else
        {
            target = targetRay.direction * projectileRange;
        }
        // Instantiating of projectile is done in another derived class, so different kinds of projectiles can be instantiated
    }


    #region Launching projectiles
    public void LaunchKineticProjectile(Bullet bullet, Transform origin, Vector3 destination, int damage, float criticalModifier, float velocity, float gravityMultiplier, float diameter, LayerMask projectileDetection)
    {
        Instantiate(bullet.gameObject, origin.position, Quaternion.LookRotation(destination - origin.position, Vector3.up));
        bullet.damage = damage;
        bullet.criticalModifier = criticalModifier;
        bullet.velocity = velocity;
        bullet.gravityMultiplier = gravityMultiplier;
        bullet.diameter = diameter;
        bullet.rayDetection = projectileDetection;
    }
    /*
    public void LaunchExplosiveProjectile(Explosive explosive, Transform origin, Vector3 destination, int damage, float blastRadius, float velocity, float gravityMultiplier, float diameter, LayerMask projectileDetection)
    {
        // Do stuff here
    }
    */
    #endregion


    public void RecoilHandler(float recoilToApply, float recoilApplyRate, WeaponHandler playerHolding)
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
        else if (!Input.GetButton("MouseLeft")) // Return recoil using recoil recovery float
        {
            //print("Recovering from recoil");
        }
    }
    #endregion




    #region ADS functions
    public void AimHandler(float magnification, float moveSpeedReduction, float zoomTime, float zoomTimer, GameObject weaponModel, Transform hipPosition, Transform aimPosition,  bool isAiming, bool toggleAim)
    {
        HoldOrToggleAim(isAiming, toggleAim);
        if (isAiming)
        {
            LerpSights(magnification, moveSpeedReduction, zoomTime, zoomTimer, weaponModel, hipPosition, aimPosition);
        }
        else
        {
            LerpSights(magnification, moveSpeedReduction, -zoomTime, zoomTimer, weaponModel, hipPosition, aimPosition);
        }
    }

    public void HoldOrToggleAim(bool isAiming, bool toggleAim)
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

    public void LerpSights(float magnification, float moveSpeedReduction, float timeAndDirection, float zoomTimer, GameObject weaponModel, Transform hipPosition, Transform aimPosition)
    {
        //Sets timer value to specify lerping of variables
        zoomTimer += Time.deltaTime / timeAndDirection;
        zoomTimer = Mathf.Clamp01(zoomTimer);

        // Reduces FOV to zoom in camera
        float zoomVariable = Mathf.Lerp(1, 1 / magnification, zoomTimer);
        playerHolding.ph.pc.playerCamera.fieldOfView = playerHolding.ph.pc.fieldOfView * zoomVariable;

        // Moves weapon position via lerping. Should I change this to an animation?
        Vector3 currentWeaponPosition = Vector3.Lerp(hipPosition.position, aimPosition.position, zoomTimer);
        Quaternion currentWeaponRotation = Quaternion.Lerp(hipPosition.rotation, aimPosition.rotation, zoomTimer);
        weaponModel.transform.SetPositionAndRotation(currentWeaponPosition, currentWeaponRotation);

        // Reduce sensitivity
        float newSensitivity = Mathf.Lerp(0, -1 + (1 / magnification), zoomTimer);
        playerHolding.ph.pc.sensitivityModifier.ApplyEffect("Aiming down sights", newSensitivity, Time.deltaTime);

        // Reduce movement speed
        float newSpeed = Mathf.Lerp(0, moveSpeedReduction, zoomTimer);
        playerHolding.ph.pc.speedModifier.ApplyEffect("Aiming down sights", newSpeed, Time.deltaTime);

        // Alter accuracy if specified
        
    }

    #endregion


    #region Reloading functions
    public void ReloadHandler(bool reloadInput, float reloadTime, bool isReloading, float reloadTimer, int roundsReloaded, Resource magazine, WeaponHandler playerHolding, AmmunitionType caliber)
    {
        // If reload button is pressed and weapon's magazine is not full OR if magazine is empty and gun is finished firing
        if (((reloadInput == true && magazine.current < magazine.max) || (magazine.current <= 0 &&/* fireTimer >= 60 / roundsPerMinute &&*/ isReloading == false)) && playerHolding.ph.a.GetStock(caliber) > 0)
        {
            ExecuteReload(reloadTimer, isReloading);
        }
        if (isReloading == true)
        {
            reloadTimer += Time.deltaTime;
        }

        if (reloadTimer >= reloadTime) // If reload time has been reached, reload ammunition into magazine
        {
            print("Gun reloaded");
            int remainingAmmo = playerHolding.ph.a.GetStock(caliber) - magazine.current; // Checks how much spare ammunition the player has
            if (remainingAmmo < roundsReloaded) // If there is not enough ammunition to reload the usual amount
            {
                magazine.current += remainingAmmo; // Reload all remaining ammunition
            }
            else
            {
                magazine.current += roundsReloaded; // Reload standard amount of ammunition per reload cycle
            }
            magazine.current = Mathf.Clamp(magazine.current, 0, magazine.max); // Ensure magazine is not overloaded

            ExecuteReload(reloadTimer, isReloading); // Start reload again. This function will cycle continuously for weapons that reload ammunition in 'batches' rather than all at once, e.g. manually loading firearms

            // If magazine is full, there is no more ammunition, or reload is interrupted by another action
            if (magazine.current >= magazine.max || playerHolding.ph.a.GetStock(caliber) - magazine.current <= 0 || (Input.GetButtonDown("MouseLeft") && magazine.current > 0)) // Also include button options for melee attacking and any other functions that would cancel out the reload function
            {
                // Cancel reload
                reloadTimer = 0;
                isReloading = false;
            }

            //ReloadMagazine(reloadTimer, roundsReloaded, magazine, playerHolding, caliber, isReloading);
        }
    }

    public void ExecuteReload(float reloadTimer, bool isReloading)
    {
        reloadTimer = 0;
        isReloading = true;
        print("Reload function started");
        // Do other reloading stuff here, like playing reloading animation
    }
    
    /*
    public void ReloadMagazine(float reloadTimer, int roundsReloaded, Resource magazine, WeaponHandler playerHolding, AmmunitionType caliber, bool isReloading)
    {
        print("Gun reloaded");
        int remainingAmmo = playerHolding.ph.a.GetStock(caliber) - magazine.current; // Checks how much spare ammunition the player has
        if (remainingAmmo < roundsReloaded) // If there is not enough ammunition to reload the usual amount
        {
            magazine.current += remainingAmmo; // Reload all remaining ammunition
        }
        else
        {
            magazine.current += roundsReloaded; // Reload standard amount of ammunition per reload cycle
        }
        magazine.current = Mathf.Clamp(magazine.current, 0, magazine.max); // Ensure magazine is not overloaded

        ExecuteReload(reloadTimer, isReloading); // Start reload again. This function will cycle continuously for weapons that reload ammunition in 'batches' rather than all at once, e.g. manually loading firearms

        // If magazine is full, there is no more ammunition, or reload is interrupted by another action
        if (magazine.current >= magazine.max || playerHolding.ph.a.GetStock(caliber) - magazine.current <= 0 || (Input.GetButtonDown("MouseLeft") && magazine.current > 0)) // Also include button options for melee attacking and any other functions that would cancel out the reload function
        {
            // Cancel reload
            reloadTimer = 0;
            isReloading = false;
        }
    }
    */
    #endregion

}
