using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KineticProjectile : Projectile
{
    [Header("Damage")]
    public int damage;
    public float knockback;
    public float criticalMultiplier;

    public override void OnHit(RaycastHit rh)
    {
        GameObject o = rh.collider.gameObject;
        Damage.PointDamage(origin.gameObject, origin.faction, o, damage, criticalMultiplier, DamageType.Shot, DamageType.CriticalShot);
        Damage.Knockback(o, knockback, transform.forward);
        base.OnHit(rh);
    }
}
