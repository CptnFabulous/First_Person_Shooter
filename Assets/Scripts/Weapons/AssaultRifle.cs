using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssaultRifle : RangedWeapon
{
    [Header("Damage")]
    public float damage = 10;
    public float criticalModifier = 2;

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
    public bool toggleAim;
    public bool isAiming;

    [Header("Fire Rate")]
    [Tooltip("Cyclic fire rate, in rounds per minute.")]
    public float roundsPerMinute = 600;

    [Header("Ammunition")]
    [Tooltip("The type of ammunition used by the weapon.")]
    public AmmunitionType caliber;
    [Tooltip("How many units of ammunition are consumed per shot.")]
    public int ammoPerShot = 1;
    [Tooltip("The weapon's magazine.")]
    public Resource magazine = new Resource { max = 30, current = 30, critical = 10 };
    [Tooltip("Time taken to reload the weapon, in seconds.")]
    public float reloadTime = 2;
    [Tooltip("Amount of rounds reloaded at a time.")]
    public int roundsReloaded = 30;

    [Header("Projectile")]
    public Bullet projectile;
    public Transform weaponMuzzle;
    public float projectileDiameter = 0.1f;
    public float gravityMultiplier = 0.1f; // Determines gravity effect on projectile for bullet drop, set to zero to disable bullet drop
    public float projectileVelocity = 100;

    [Header("Cosmetic")]
    public ParticleSystem shellEjection;
    public GameObject muzzleFlash;
    [Range(0, 1)] public float muzzleFlashDuration;
    float muzzleFlashTimer;
    // public GameObject shellPrefab;

    // Update is called once per frame
    void Update()
    {
        
    }
}
