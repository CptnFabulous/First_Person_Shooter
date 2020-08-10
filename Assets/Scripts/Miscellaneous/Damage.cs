using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DamageType
{
    Shot,
    CriticalShot,
    Burned,
    Bludgeoned,
    FallDamage,
    BlownUp,
    Gibbed
}

public static class Damage
{
    public static void ShootProjectile(ProjectileStats projectile, float spread, float range, Character origin, Transform aimOrigin, Vector3 muzzle, Vector3 direction)
    {
        for (int i = 0; i < projectile.projectileCount; i++)
        {
            RaycastHit targetFound;
            Vector3 processedDirection = Quaternion.Euler(Random.Range(-spread, spread), Random.Range(-spread, spread), Random.Range(-spread, spread)) * direction;
            if (Physics.Raycast(aimOrigin.position, processedDirection, out targetFound, range, projectile.hitDetection)) // To reduce the amount of superfluous variables, I re-used the 'target' Vector3 in the same function as it is now unneeded for its original purpose
            {
                processedDirection = targetFound.point;
            }
            else
            {
                processedDirection = aimOrigin.position + processedDirection.normalized * range;
            }

            Projectile p = projectile.NewProjectileClass(origin);

            if (Vector3.Angle(direction, processedDirection - muzzle) < 90) // Checks that the position 'processedDirection' is actually further away than the muzzle and that the bullets will not travel in the complete wrong direction
            {
                Object.Instantiate(p.gameObject, muzzle, Quaternion.LookRotation(processedDirection - muzzle, aimOrigin.up));
            }
            else // Otherwise, the gun barrel is probably clipping into a wall. Directly spawn the projectiles at the appropriate hit points.
            {
                // Spawn the projectile directly at the location where it is supposed to hit, with the correct rotation, and activate its OnHit function.
                Object.Instantiate(p.gameObject, processedDirection, Quaternion.LookRotation(processedDirection - aimOrigin.position, aimOrigin.up));
                p.OnHit(targetFound);
            }
        }
    }

    public static void MeleeAttack()
    {

    }

    public static void PointDamage(GameObject origin, Faction originFaction, GameObject attackedObject, int damage, float criticalMultiplier, DamageType normalCause, DamageType criticalCause)
    {
        DamageHitbox hitbox = attackedObject.GetComponent<DamageHitbox>(); // Checks collider gameObject for a damageHitbox script
        if (hitbox != null)
        {
            Debug.Log("Hit");

            hitbox.Damage(damage, criticalMultiplier, origin, originFaction, normalCause, criticalCause);
        }
    }

    public static void PointDamage(GameObject origin, Faction originFaction, GameObject attackedObject, int damage, DamageType cause, bool isSevere)
    {
        DamageHitbox hitbox = attackedObject.GetComponent<DamageHitbox>(); // Checks collider gameObject for a damageHitbox script
        if (hitbox != null)
        {
            hitbox.Damage(damage, origin, originFaction, cause, isSevere);
        }
    }

    public static void Knockback(GameObject attackedObject, float force, Vector3 direction)
    {
        if (force != 0) // Checks if knockback needs to be applied
        {
            Rigidbody rb;
            DamageHitbox d = attackedObject.GetComponent<DamageHitbox>(); // Checks object hit for DamageHitbox script
            if (d != null) // If script is present, the object must be a DamageHitbox, so it checks its root object for a rigidbody component.
            {
                rb = d.GetRootObject().GetComponent<Rigidbody>();
            }
            else
            {
                rb = attackedObject.GetComponent<Rigidbody>(); // If object is not a hitbox, look for rigidbody script in object.
            }

            if (rb != null) // If a rigidbody is found, apply knockback force.
            {
                rb.AddForce(direction * force, ForceMode.Impulse);
            }
        }
        
    }

    public static void InstantExplosion(GameObject origin, Faction originFaction, Transform explosionOrigin, int damage, float knockback, float blastRadius, float explosionTime, AnimationCurve damageFalloff, AnimationCurve knockbackFalloff, LayerMask blastDetection, DamageType cause, bool isSevere)
    {
        List<Health> alreadyDamaged = new List<Health>();
        Collider[] affectedObjects = Physics.OverlapSphere(explosionOrigin.position, blastRadius, blastDetection);
        foreach (Collider c in affectedObjects)
        {
            Vector3 targetDirection = c.transform.position - explosionOrigin.position;
            RaycastHit isVulnerable;
            if (Physics.Raycast(explosionOrigin.position, targetDirection, out isVulnerable, blastRadius, blastDetection))
            {
                if (isVulnerable.collider == c)
                {
                    float i = isVulnerable.distance / blastRadius;
                    
                    DamageHitbox hitbox = c.GetComponent<DamageHitbox>(); // Checks collider gameObject for a damageHitbox script
                    if (hitbox != null)
                    {
                        bool undamagedCheck = false;
                        foreach (Health h in alreadyDamaged)
                        {
                            if (h == hitbox.healthScript)
                            {
                                undamagedCheck = true;
                            }
                        }

                        if (undamagedCheck == false)
                        {
                            int d = Mathf.RoundToInt(damage * damageFalloff.Evaluate(i));
                            hitbox.Damage(d, origin, originFaction, cause, isSevere);
                            Debug.Log("Dealt " + d + " damage to " + hitbox.name + " at " + hitbox.transform.position + ".");
                            alreadyDamaged.Add(hitbox.healthScript);
                        }
                    }

                    Knockback(isVulnerable.collider.gameObject, knockback * knockbackFalloff.Evaluate(i), targetDirection);
                }
            }
        }
    }
}
