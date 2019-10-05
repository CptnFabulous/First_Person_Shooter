using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KineticProjectile : Projectile
{
    public int damage;
    public float criticalModifier;

    public override void OnHit()
    {
        DamageHitbox hitbox = projectileHit.collider.GetComponent<DamageHitbox>(); // Checks collider gameObject for a damageHitbox script
        if (hitbox != null)
        {
            if (hitbox.critical == true)
            {
                hitbox.Damage(Mathf.RoundToInt(damage * criticalModifier), DamageType.CriticalShot);
            }
            else
            {
                hitbox.Damage(damage, DamageType.Shot);
            }
        }

        base.OnHit();
    }
}
