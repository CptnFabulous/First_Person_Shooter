using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveProjectile : Projectile
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

    [Header("Cosmetics")]
    public ParticleSystem impactEffect;

    public override void OnHit()
    {
        InstantiateOnImpact(impactEffect.gameObject, false);
        Damage.InstantExplosion(origin.gameObject, origin.faction, transform, damage, knockback, blastRadius, explosionTime, damageFalloff, knockbackFalloff, hitDetection, DamageType.BlownUp, false);
        Damage.PointDamage(origin.gameObject, origin.faction, projectileHit.collider.gameObject, Mathf.RoundToInt(damage * directHitMultiplier) - damage, DamageType.Gibbed, true); // Find way to ensure enemy is not damaged twice by direct hit and by splash damage
        base.OnHit();
    }
}
