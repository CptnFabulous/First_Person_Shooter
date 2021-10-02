using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : Projectile
{
    [Header("Damage")]
    public int damage;
    public float knockback;
    public float criticalMultiplier;
    public DamageType type = DamageType.Piercing;
    public bool allowSelfDamage;
    public bool allowFriendlyFire;

    public override void OnHit(RaycastHit rh)
    {
        if (origin.CanDamage(Character.FromObject(rh.collider.gameObject), allowFriendlyFire, allowSelfDamage))
        {
            // If the target has health, damage it
            DamageHitbox hitbox = rh.collider.GetComponent<DamageHitbox>();
            if (hitbox != null)
            {
                // Calculate damage
                int damageToDeal = damage;
                if (hitbox.critical)
                {
                    damageToDeal = Mathf.CeilToInt(damageToDeal * criticalMultiplier);
                }
                hitbox.Damage(damageToDeal, origin, type);
                // Play damage effects
            }

            // If the target has physics, knock it around
            Rigidbody rb = rh.collider.GetComponentInParent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForceAtPosition(transform.forward * knockback, rh.point, ForceMode.Impulse);
                // Play knockback effects
            }

            base.OnHit(rh);
        }

        // Otherwise, ignore and resume normal operation
    }
}