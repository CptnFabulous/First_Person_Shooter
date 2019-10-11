using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KineticProjectile : Projectile
{
    [Header("Damage")]
    public int damage;
    public float criticalMultiplier;

    public override void OnHit()
    {
        Damage.PointDamage(originCharacter, projectileHit.collider.gameObject, damage, criticalMultiplier, DamageType.Shot, DamageType.CriticalShot);
        base.OnHit();
    }
}
