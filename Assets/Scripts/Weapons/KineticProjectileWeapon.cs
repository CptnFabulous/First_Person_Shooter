using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KineticProjectileWeapon : ProjectileWeapon
{
    [Header("Damage")]
    public float criticalModifier = 2;

    [Header("Projectile")]
    public RaycastBullet projectile;
    public Transform weaponMuzzle;
    public float projectileDiameter = 0.1f;
    public float gravityMultiplier = 0.1f; // Determines gravity effect on projectile for bullet drop, set to zero to disable bullet drop
    public float projectileVelocity = 100;

    [Header("Cosmetic")]
    public GameObject muzzleFlash;
    [Range(0, 1)] public float muzzleFlashDuration;
    float muzzleFlashTimer;
    // public ParticleSystem shellEjection;
    // public GameObject shellPrefab;

    public override void Update()
    {
        base.Update(); // DOES ALL THE IMPORTANT STUFF IN UPDATE, STUFF EXCLUSIVE TO THIS SCRIPT IS PLACED BEFORE OR AFTER THIS.

        if (muzzleFlashTimer >= 60 / roundsPerMinute * muzzleFlashDuration)
        {
            muzzleFlash.SetActive(false);
        }
        else
        {
            muzzleFlashTimer += Time.deltaTime;
        }
    }

    public override void Shoot()
    {
        base.Shoot();

        // Do cosmetic stuff
        muzzleFlash.SetActive(true);
    }

    public override void LaunchProjectile()
    {
        base.LaunchProjectile();

        // GameObject bullet = Instantiate(projectile.gameObject, weaponMuzzle.transform.position, Quaternion.LookRotation( - weaponMuzzle.transform.position, Vector3.up));
        // How to reference bullet ray impact point
        projectile.diameter = projectileDiameter;
        projectile.gravityMultiplier = gravityMultiplier;
        projectile.velocity = projectileVelocity;
        projectile.damage = damage;
        projectile.criticalModifier = criticalModifier;
        projectile.rayDetection = rayDetection;
    }
}
