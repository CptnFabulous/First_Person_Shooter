using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Kinetic Projectile", menuName = "ScriptableObjects/Projectiles/Kinetic Projectile", order = 1)]
public class KineticProjectileData : ProjectileData
{
    [Header("Damage")]
    public int damage;
    public float knockback;
    public float criticalMultiplier;

    public override GameObject NewProjectile(GameObject origin, Faction originFaction)
    {
        GameObject launchedProjectile = prefab.gameObject;
        KineticProjectile p = launchedProjectile.GetComponent<KineticProjectile>();

        p.velocity = velocity;
        p.diameter = diameter;
        p.gravityMultiplier = gravityMultiplier;
        p.hitDetection = hitDetection;
        p.origin = origin;
        p.originFaction = originFaction;

        p.damage = damage;
        p.knockback = knockback;
        p.criticalMultiplier = criticalMultiplier;

        return launchedProjectile;
    }
}
