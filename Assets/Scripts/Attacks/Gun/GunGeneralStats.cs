using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GunGeneralStats : MonoBehaviour
{
    [Header("Projectile")]
    public Projectile projectilePrefab;
    public Transform muzzle;
    public int projectileCount = 1;

    [Header("Accuracy")]
    [Range(0, 180)] public float projectileSpread;
    public float range;

    [Header("Recoil")]
    public float recoil;
    public float recoilApplyRate;
    public float recoilRecovery;

    //[Header("Spread")]
    //public float spreadMultiplier;
    //public float timeToReachMaxSpread;
    //public AnimationCurve spreadRampSpeed;

    [Header("Ammunition")]
    public bool consumesAmmo = true;
    public AmmunitionType ammoType;
    public int ammoPerShot = 1;

    [Header("Cosmetics")]
    public Transform heldPosition;
    public UnityEvent effectsOnFire;
}
