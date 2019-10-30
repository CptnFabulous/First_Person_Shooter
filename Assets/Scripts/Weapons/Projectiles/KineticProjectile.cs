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

        if (knockback != 0)
        {
            Rigidbody rb = o.GetComponent<Rigidbody>();
            DamageHitbox d = o.GetComponent<DamageHitbox>();
            if (rb == null && d != null)
            {
                rb = d.GetRootObject().GetComponent<Rigidbody>();
            }

            if (rb != null)
            {
                rb.AddForce(transform.forward * knockback, ForceMode.Impulse);
            }
        }

        base.OnHit();
    }
}
