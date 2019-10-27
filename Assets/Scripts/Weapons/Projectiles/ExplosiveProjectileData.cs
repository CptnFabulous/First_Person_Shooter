using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Explosive Projectile", menuName = "ScriptableObjects/Projectiles/Explosive Projectile", order = 1)]
public class ExplosiveProjectileData : ProjectileData
{
    [Header("Damage")]
    public int damage;
    public float directHitMultiplier;

    [Header("Explosion stats")]
    public float blastRadius;
    public float explosionTime;
    public float knockback;
    public AnimationCurve damageFalloff;
    public AnimationCurve knockbackFalloff;

    public override GameObject NewProjectile(GameObject origin, Faction originFaction)
    {
        GameObject launchedProjectile = prefab.gameObject;
        ExplosiveProjectile p = launchedProjectile.GetComponent<ExplosiveProjectile>();

        p.velocity = velocity;
        p.diameter = diameter;
        p.gravityMultiplier = gravityMultiplier;
        p.hitDetection = hitDetection;
        p.origin = origin;
        p.originFaction = originFaction;

        p.damage = damage;
        p.directHitMultiplier = directHitMultiplier;
        p.blastRadius = blastRadius;
        p.explosionTime = explosionTime;
        p.knockback = knockback;
        p.damageFalloff = damageFalloff;
        p.knockbackFalloff = knockbackFalloff;

        return launchedProjectile;
    }
}
