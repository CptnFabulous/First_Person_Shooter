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
    Gibbed,

    Piercing, // e.g. gunshots and stabs
    Slashing, // e.g. shallow sword cuts
    Severing, // e.g. body part removal
    Bludgeoning, // e.g. blunt force attacks
    Impact, // e.g. slamming into a wall or floor
    Explosive,
    Burning,
    Freezing,
    Electric,
    Corrosive,
    Poison,
    Asphyxiation,
    DeletionByGame // e.g. falling out of the level or similar non-diegetic game process
}

public static class Damage
{
    /// <summary>
    /// Deprecated! Use GunGeneralStats.Shoot() instead.
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="count"></param>
    /// <param name="spread"></param>
    /// <param name="range"></param>
    /// <param name="origin"></param>
    /// <param name="aimOrigin"></param>
    /// <param name="forward"></param>
    /// <param name="up"></param>
    /// <param name="muzzle"></param>
    public static void ShootProjectile(Projectile prefab, int count, float spread, float range, Character origin, Vector3 aimOrigin, Vector3 forward, Vector3 up, Vector3 muzzle)
    {
        for (int i = 0; i < count; i++)
        {
            #region Calculate initial variables
            // Declare RaycastHit
            RaycastHit targetFound;
            // Declare direction in which to fire projectile
            Vector3 direction = new Vector3(Random.Range(-spread, spread), Random.Range(-spread, spread), Random.Range(-spread, spread));
            direction = Misc.AngledDirection(direction, forward, up);
            #endregion

            #region Launch raycast to determine where to shoot projectile
            if (Physics.Raycast(aimOrigin, direction, out targetFound, range, prefab.hitDetection)) // To reduce the amount of superfluous variables, I re-used the 'target' Vector3 in the same function as it is now unneeded for its original purpose
            {
                direction = targetFound.point;
            }
            else
            {
                direction = aimOrigin + direction.normalized * range;
            }
            #endregion

            #region Instantiate projectile
            Projectile p = prefab;

            p.origin = origin;

            // Checks that the position 'processedDirection' is actually further away than the muzzle and that the bullets will not travel in the complete wrong direction
            if (Vector3.Angle(forward, direction - muzzle) < 90)
            {
                Object.Instantiate(p, muzzle, Quaternion.LookRotation(direction - muzzle, up));
            }
            else
            {
                // Otherwise, the gun barrel is probably clipping into a wall. Directly spawn the projectiles at the appropriate hit points.
                Object.Instantiate(p, direction, Quaternion.LookRotation(direction - aimOrigin, up));
                p.OnHit(targetFound);
            }
            #endregion

            Debug.DrawLine(aimOrigin, direction, Color.magenta, 1f);
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

    public static void InstantExplosion(Character origin, Transform explosionOrigin, int damage, float knockback, float blastRadius, float explosionTime, AnimationCurve damageFalloff, AnimationCurve knockbackFalloff, LayerMask blastDetection, DamageType cause, bool isSevere)
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
                            //int d = (int) (damage * damageFalloff.Evaluate(i));
                            hitbox.Damage(d, origin, cause, isSevere);
                            alreadyDamaged.Add(hitbox.healthScript);
                        }
                    }

                    Knockback(isVulnerable.collider.gameObject, knockback * knockbackFalloff.Evaluate(i), targetDirection);
                }
            }
        }
    }
}
