using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DamageType
{
    Shot,
    CriticalShot,
    Burned,
    KnockedOut,
    BlownUp,
    Gibbed
}

public static class Damage
{
    public static void ShootProjectile(Projectile projectile, float velocity, float gravityMultiplier, float diameter, float spread, float range, LayerMask rayDetection, GameObject origin, Transform aimOrigin, Transform muzzle, Vector3 direction)
    {
        RaycastHit targetFound;
        Vector3 destination = Quaternion.Euler(Random.Range(-spread, spread), Random.Range(-spread, spread), Random.Range(-spread, spread)) * direction;
        if (Physics.Raycast(aimOrigin.position, destination, out targetFound, range, rayDetection)) // To reduce the amount of superfluous variables, I re-used the 'target' Vector3 in the same function as it is now unneeded for its original purpose
        {
            destination = targetFound.point;
        }
        else
        {
            destination *= range;
        }

        GameObject launchedProjectile = Object.Instantiate(projectile.gameObject, muzzle.position, Quaternion.LookRotation(destination - muzzle.position, Vector3.up));
        Projectile p = launchedProjectile.GetComponent<Projectile>();
        p.velocity = velocity;
        p.gravityMultiplier = gravityMultiplier;
        p.diameter = diameter;
        p.targetDetection = rayDetection;
        p.origin = origin;

        KineticProjectile kp = p.GetComponent<KineticProjectile>();
        if (kp != null)
        {

        }

        ExplosiveProjectile ep = p.GetComponent<ExplosiveProjectile>();
        if (ep != null)
        {

        }

        // HAVE MORE STUFF HERE FOR DETERMINING TYPE OF PROJECTILE AND ASSIGNING APPROPRIATE VARIABLES. HOW DO I DO THIS?
    }



    public static void PointDamage(GameObject origin, GameObject attackedObject, int damage, float criticalMultiplier, DamageType normalCause, DamageType criticalCause)
    {
        DamageHitbox hitbox = attackedObject.GetComponent<DamageHitbox>(); // Checks collider gameObject for a damageHitbox script
        if (hitbox != null)
        {
            Debug.Log("Hit");

            if (hitbox.critical == true)
            {
                hitbox.Damage(Mathf.RoundToInt(damage * criticalMultiplier), origin, criticalCause);
            }
            else
            {
                hitbox.Damage(damage, origin, normalCause);
            }

            WeaponHandler wh = origin.GetComponent<WeaponHandler>(); // Checks for WeaponHandler script i.e. if the thing that shot the projectile was a player
            if (wh != null)
            {
                wh.ph.hud.PlayHitMarker(hitbox.critical);
            }
        }
    }

    public static void PointDamage(GameObject origin, GameObject attackedObject, int damage, DamageType cause, bool isCritical)
    {
        DamageHitbox hitbox = attackedObject.GetComponent<DamageHitbox>(); // Checks collider gameObject for a damageHitbox script
        if (hitbox != null)
        {
            hitbox.Damage(damage, origin, cause);

            WeaponHandler wh = origin.GetComponent<WeaponHandler>(); // Checks for WeaponHandler script i.e. if the thing that shot the projectile was a player
            if (wh != null)
            {
                wh.ph.hud.PlayHitMarker(isCritical);
            }
        }
    }

    public static void SimpleExplosion(Transform origin, LayerMask blastDetection, int damage, float knockback, float blastRadius, float explosionTime, AnimationCurve damageFalloff, AnimationCurve knockbackFalloff)
    {
        Collider[] affectedObjects = Physics.OverlapSphere(origin.position, blastRadius, blastDetection);
        foreach (Collider c in affectedObjects)
        {
            Vector3 targetDirection = c.transform.position - origin.position;
            RaycastHit isVulnerable;
            if (Physics.Raycast(origin.position, targetDirection, out isVulnerable, blastRadius, blastDetection))
            {
                if (isVulnerable.collider == c)
                {
                    float i = isVulnerable.distance / blastRadius;

                    /*
                    // DAMAGE CODE IS COMMENTED OUT BECAUSE I DON'T KNOW HOW TO PREVENT THE EXPLOSION FROM DAMAGING THE ENEMY MULTIPLE TIMES FOR DIFFERENT HITBOXES
                    DamageHitbox hitbox = isVulnerable.collider.GetComponent<DamageHitbox>(); // Checks collider gameObject for a damageHitbox script, and deals damage
                    if (hitbox != null)
                    {
                        float d = damage * damageFalloff.Evaluate(i);
                        hitbox.Damage(Mathf.RoundToInt(d), DamageType.BlownUp);
                    }
                    */

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

    /*
    public static IEnumerator Explosion(Transform origin, int damage, float directHitModifier, int knockback, float blastRadius, float explosionTime, AnimationCurve damageFalloff, AnimationCurve knockbackFalloff)
    {
        float i = 0;
        while(i < 1)
        {
            i += Time.deltaTime / explosionTime;
            float br = Mathf.Lerp(0, blastRadius, i);
            float d = damage * falloff.Evaluate(i);
            float f = explosionForce * falloff.Evaluate(i);

        }
        
    }
    */
    
}
