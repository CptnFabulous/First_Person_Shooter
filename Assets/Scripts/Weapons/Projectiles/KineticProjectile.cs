using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KineticProjectile : Projectile
{
    [Header("Damage")]
    public int damage;
    public float knockback;
    public float criticalMultiplier;

    public override void OnHit()
    {
        GameObject o = projectileHit.collider.gameObject;
        Damage.PointDamage(origin, originFaction, o, damage, criticalMultiplier, DamageType.Shot, DamageType.CriticalShot);
        Damage.Knockback(o, knockback, transform.forward);
        base.OnHit();
    }
}
