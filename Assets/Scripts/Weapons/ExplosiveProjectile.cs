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

    public override void OnHit()
    {
        DamageHitbox hitbox = projectileHit.collider.GetComponent<DamageHitbox>(); // Checks collider gameObject for a damageHitbox script
        if (hitbox != null)
        {
            hitbox.Damage(Mathf.RoundToInt(damage * directHitMultiplier), DamageType.Gibbed);
        }

        Damage.SimpleExplosion(transform, targetDetection, damage, knockback, blastRadius, explosionTime, damageFalloff, knockbackFalloff);
        base.OnHit();
    }
}
