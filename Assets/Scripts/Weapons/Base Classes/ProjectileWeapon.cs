﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileWeapon : Weapon
{
    [Header("Accuracy")]
    [Range(0, 180)] public float projectileSpread = 5;
    public float recoil = 10;
    public float recoilRecovery = 10;
    public float range = 1000;
    public LayerMask rayDetection;
    Ray targetRay;
    RaycastHit targetFound;

    [Header("Fire Rate")]
    public float roundsPerMinute = 600;
    public int projectileCount = 1;
    [Tooltip("Amount of shots that can be fired before needing to re-press the trigger. Set to 1 for semi-automatic, or more for burst-fire weapons. Set to zero to enable full-auto fire.")]
    public int burstCount;
    float fireTimer;
    float shotsInBurst;

    [Header("Ammunition")]
    public Ammunition ammoSource; // The ammunition supply belonging to the player shooting the gun
    public AmmunitionType caliber;
    [Tooltip("Sets weapon's magazine capacity. Set to zero to ignore magazine code, allowing the player to fire continuously without reloading.")]
    [Min(0)] public int magazineCapacity = 30;
    public int roundsInMagazine = 30;
    public float reloadTime = 2;
    bool isReloading;
    float reloadTimer;

    // Start is called before the first frame update
    void Start()
    {
        fireTimer = 60 / roundsPerMinute;
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (fireTimer < 60 / roundsPerMinute)
        {
            fireTimer += Time.deltaTime; // Counts up timer for next shot. This doesn't really need to be inside an if statement but whatever. It may benefit to have this be outside an if statement if for whatever reason you want to calculate how long it's been since the weapon was last fired.
        }

        // If fire button is pressed, previous shot has finished firing, maximum burst count has not been exceeded (or no burst function is present) and ammo remains in magazine (or if gun does not need reloading)
        if (Input.GetButton("MouseLeft") && fireTimer >= 60 / roundsPerMinute && (shotsInBurst < burstCount || burstCount <= 0) && ((roundsInMagazine > 0 && isReloading == false) || magazineCapacity <= 0))
        {
            Shoot();
        }
        else if (Input.GetButtonUp("MouseLeft") && burstCount > 0)
        {
            shotsInBurst = 0;
        }

        // If reload button is pressed and weapon's magazine is not full OR if magazine is empty and gun is finished firing
        if ((Input.GetButtonDown("Reload") && roundsInMagazine < magazineCapacity) || (roundsInMagazine <= 0 && fireTimer >= 60 / roundsPerMinute && isReloading == false))
        {
            reloadTimer = 0;
            isReloading = true;
        }
        if (isReloading == true)
        {
            Reload();
        }
    }

    void Reload() // Untested
    {
        reloadTimer += Time.deltaTime;
        if (reloadTimer >= reloadTime)
        {
            roundsInMagazine = magazineCapacity; // This simply sets the magzine capacity to the maximum, meaning the player has infinite ammo. Substitute appropriate code for subtracting ammo from reserve 
            isReloading = false;
        }
    }

    public virtual void Shoot()
    {
        for (int i = 0; i < projectileCount; i++)
        {
            LaunchProjectile();
        }

        fireTimer = 0;

        if (magazineCapacity > 0)
        {
            roundsInMagazine -= 1;
        }
        if (burstCount > 0)
        {
            shotsInBurst += 1;
        }

        // Put other cosmetic stuff here.
    }

    public virtual void LaunchProjectile()
    {
        Vector3 target = new Vector3();

        targetRay.origin = transform.position;
        targetRay.direction = Quaternion.Euler(Random.Range(-projectileSpread, projectileSpread), Random.Range(-projectileSpread, projectileSpread), Random.Range(-projectileSpread, projectileSpread)) * transform.forward;
        /*
        Vector3 raySpread = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1));
        raySpread.Normalize();
        targetRay.direction = Quaternion.Euler(raySpread.x * projectileSpread, raySpread.y * projectileSpread, raySpread.z * projectileSpread) * transform.forward;
        */

        if (Physics.Raycast(targetRay, out targetFound, range, rayDetection))
        {
            target = targetFound.point;
        }
        else
        {
            target = targetRay.direction * range;
        }

        //GameObject bullet = Instantiate(projectile.gameObject, weaponMuzzle.transform.position, Quaternion.LookRotation(target - weaponMuzzle.transform.position, Vector3.up));
    }
}
