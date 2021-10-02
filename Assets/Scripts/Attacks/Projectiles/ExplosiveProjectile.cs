using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveProjectile : Projectile
{
    [Header("Damage")]
    public Explosion explosionStats;
    public float directHitMultiplier;


    

    public override void OnHit(RaycastHit rh)
    {
        explosionStats.Detonate(transform.position, origin);

        //Damage.InstantExplosion(origin, transform, damage, knockback, blastRadius, explosionTime, damageFalloff, knockbackFalloff, hitDetection, DamageType.BlownUp, false);
        //Damage.PointDamage(origin, rh.collider.gameObject, Mathf.RoundToInt(damage * directHitMultiplier) - damage, DamageType.Gibbed, true); // Find way to ensure enemy is not damaged twice by direct hit and by splash damage
        base.OnHit(rh);
    }
}
