using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DamageType
{
    Shot,
    CriticalShot,
    Burned,
    Bludgeoned,
    BlownUp,
    Gibbed
}

public static class Damage
{
    public static void ShootProjectile(ProjectileData projectile, float spread, float range, GameObject origin, Faction originFaction, Transform aimOrigin, Transform muzzle, Vector3 direction)
    {
        RaycastHit targetFound;
        Vector3 destination = Quaternion.Euler(Random.Range(-spread, spread), Random.Range(-spread, spread), Random.Range(-spread, spread)) * direction;
        if (Physics.Raycast(aimOrigin.position, destination, out targetFound, range, projectile.hitDetection)) // To reduce the amount of superfluous variables, I re-used the 'target' Vector3 in the same function as it is now unneeded for its original purpose
        {
            destination = targetFound.point;
        }
        else
        {
            destination *= range;
        }

        Object.Instantiate(projectile.NewProjectile(origin, originFaction), muzzle.position, Quaternion.LookRotation(destination - muzzle.position, Vector3.up));
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

                    Rigidbody rb = isVulnerable.collider.GetComponent<Rigidbody>(); // Checks collider gameObject for a rigidbody, and knocks rigidbody back accordingly
                    if (rb != null)
                    {
                        float f = knockback * knockbackFalloff.Evaluate(i);
                        rb.AddForce(targetDirection.normalized * f, ForceMode.Impulse);
                    }
                }
            }
        }
    }
}
