using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public struct Explosion
{
    [Header("Attack stats")]
    public int damage;
    public AnimationCurve damageFalloff;
    public float knockback;
    public AnimationCurve knockbackFalloff;
    public DamageType type;

    [Header("Detection")]
    public float blastRadius;
    //public float criticalRadius;
    //public float explosionTime;
    public LayerMask hitDetection;
    public bool selfDamage;
    public bool friendlyFire;

    [Header("Other functions")]
    public UnityEvent onExplosionStart;
    public UnityEvent onExplosionEnd;



    public void Detonate(Vector3 centre, Entity origin)
    {
        onExplosionStart.Invoke();

        Character attacker = origin as Character;
        List<Health> alreadyDamaged = new List<Health>();

        //RaycastHit[] affected = Physics.SphereCastAll(position, blastRadius, Vector3.forward, 0, hitDetection);
        Collider[] affected = Physics.OverlapSphere(centre, blastRadius, hitDetection);
        for (int i = 0; i < affected.Length; i++)
        {
            // Calculates the closest point and raycast towards it to check that it is not behind cover
            Vector3 closestPoint = GetColliderPointFromCentre(affected[i], centre);
            Ray checkRay = new Ray(centre, closestPoint - centre);
            bool lineOfSight = Physics.Raycast(checkRay, out RaycastHit hit, blastRadius, hitDetection) && hit.collider == affected[i];
            if (!lineOfSight) { continue; }
            

            // If raycast hits, collider is in the path of the explosion. Check if the object can be attacked by the user
            bool canAttack = attacker == null || attacker.CanDamage(hit.collider.GetComponent<Character>(), friendlyFire, selfDamage);
            if (!canAttack) { continue; }


            // If check is positive, attacker is not restricted from attacking the target (no faction or faction is hostile)
            float distanceZeroToOne = hit.distance / blastRadius;
            // If the collider is attached to an object that wasn't already damaged by the explosion
            DamageHitbox dh = hit.collider.GetComponent<DamageHitbox>();
            if (dh != null && !alreadyDamaged.Contains(dh.healthScript))
            {
                float multipliedDamage = damage * damageFalloff.Evaluate(distanceZeroToOne);
                Debug.Log(multipliedDamage);
                dh.Damage(Mathf.RoundToInt(multipliedDamage), origin, type);
                alreadyDamaged.Add(dh.healthScript);
            }
            // If the collider is affected by physics, knock it around
            Rigidbody rb = hit.collider.GetComponentInParent<Rigidbody>();
            if (rb != null)
            {
                float multipliedKnockback = knockback * knockbackFalloff.Evaluate(distanceZeroToOne);
                rb.AddForceAtPosition(checkRay.direction * multipliedKnockback, hit.point, ForceMode.Impulse);
            }
        }






        /*
        List<Health> alreadyDamaged = new List<Health>();
        List<Rigidbody> alreadyKnockedBack = new List<Rigidbody>();
        Character attacker = origin as Character;

        Collider[] affectedObjects = Physics.OverlapSphere(position, blastRadius, hitDetection);
        foreach (Collider c in affectedObjects)
        {
            Ray check = new Ray(position, c.transform.position - position);
            // If a check is successful between the collider and the blast origin
            if (Physics.Raycast(check, out RaycastHit hit, blastRadius, hitDetection) && hit.collider == c)
            {
                // If the collider can be attacked
                if (Character.CanDamage(attacker, Character.FromObject(hit.collider.gameObject), friendlyFire, selfDamage))
                {
                    float distanceFromOrigin = blastRadius / hit.distance;

                    // Check if entity can be damaged, and if it hasn't already
                    DamageHitbox dh = hit.collider.GetComponent<DamageHitbox>();
                    if (dh != null && !alreadyDamaged.Contains(dh.healthScript))
                    {
                        int damageToDeal = Mathf.RoundToInt(damage * damageFalloff.Evaluate(distanceFromOrigin));
                        dh.Damage(damageToDeal, origin, type);
                        alreadyDamaged.Add(dh.healthScript);
                    }

                    // Check if entity can be knocked back, and if it hasn't already
                    Rigidbody rb = hit.collider.GetComponentInParent<Rigidbody>();
                    if (rb != null && !alreadyKnockedBack.Contains(rb))
                    {
                        float knockbackToInflict = knockback * knockbackFalloff.Evaluate(distanceFromOrigin);
                        rb.AddForceAtPosition(check.direction * knockbackToInflict, hit.point, ForceMode.Impulse);
                    }
                }
                
                


                
                
                
                
                
                
            }
        }
        */

        onExplosionEnd.Invoke();
    }



    public static Vector3 GetColliderPointFromCentre(Collider c, Vector3 centre)
    {
        MeshCollider mc = c as MeshCollider;
        if (mc != null && mc.convex == false)
        {
            return c.bounds.ClosestPoint(centre);
        }
        else
        {
            //Debug.Log(c.name + " has proper bounds");
            return c.ClosestPoint(centre);
        }
    }


    
    public static Explosion Default
    {
        get
        {
            Explosion e = new Explosion
            {
                damage = 25,
                damageFalloff = AnimationCurve.Linear(0, 1, 1, 0),
                knockback = 100,
                knockbackFalloff = AnimationCurve.Linear(0, 1, 1, 0),
                type = DamageType.Explosive,
                blastRadius = 5,
                //criticalRadius = 0.2f,
                //explosionTime = 0.1f,
                hitDetection = ~0,
                onExplosionStart = null,
                onExplosionEnd = null,
            };
            return e;
        }
    }
}
