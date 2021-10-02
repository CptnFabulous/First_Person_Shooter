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



    /*
    public void Explode(Vector3 position, Entity origin)
    {
        // If explosion happens instantly
        if (explosionTime <= 0)
        {
            // Use a simpler and more performant function for the explosion
            ExplodeInstantly(position, origin);
            return;
        }
        // replace with ExplodeOverTimer
        ExplodeInstantly(position, origin);
    }
    */

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
            Vector3 closestPoint = affected[i].bounds.ClosestPoint(centre);
            if (affected[i] as MeshCollider == null || (affected[i] as MeshCollider).convex == true)
            {
                Debug.Log(affected[i].name + " has proper bounds");
                closestPoint = affected[i].ClosestPoint(centre);
            }
            Ray checkRay = new Ray(centre, closestPoint - centre);
            bool lineOfSight = Physics.Raycast(checkRay, out RaycastHit hit, blastRadius, hitDetection) && hit.collider == affected[i];
            if (!lineOfSight) { continue; }

            //Debug.Log(hit.collider.name + " is in line of sight");

            // If raycast hits, collider is in the path of the explosion. Check if the object can be attacked by the user
            bool canAttack = attacker == null || attacker.CanDamage(hit.collider.GetComponent<Character>(), friendlyFire, selfDamage);
            if (!canAttack) { continue; }

            //Debug.Log(hit.collider.name + " can be attacked");

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

    /*
    IEnumerator ExplodeOverTime(Vector3 position, Entity origin)
    {
        // This is declared beforehand because creating a new one multiple times eats up processing power
        WaitForEndOfFrame loop = new WaitForEndOfFrame();

        List<Collider> hitOnPreviousFrames = new List<Collider>();
        List<Health> alreadyDamaged = new List<Health>();
        List<Rigidbody> alreadyKnockedBack = new List<Rigidbody>();

        float timer = 0;
        while (timer <= 1)
        {
            // Divides time elapsed fraction by the blast radius to get the current size of the explosion
            float currentRadius = timer / blastRadius;
            Collider[] thingsHit = Physics.OverlapSphere(position, currentRadius, hitDetection);
            for(int i = 0; i < thingsHit.Length; i++)
            {
                // If collider was not previously hit
                if (!hitOnPreviousFrames.Contains(thingsHit[i]))
                {
                    // Add it to list so it does not get hit a second time
                    hitOnPreviousFrames.Add(thingsHit[i]);

                    Vector3 direction = thingsHit[i].transform.position - position;
                    // Launch raycast to make sure object isn't behind cover
                    RaycastHit explosionHit;
                    if (Physics.Raycast(position, direction, out explosionHit, blastRadius, hitDetection))
                    {
                        // If the raycast hits the collider successfully
                        if (explosionHit.collider == c)
                        {
                            // Calculate a float from zero to one representing how close to the blast radius the collider is
                            float distancePercentage = explosionHit.distance / blastRadius;

                            // Check for hitbox
                            DamageHitbox hitbox = c.GetComponent<DamageHitbox>();
                            if (hitbox != null) // If one exists
                            {
                                // Check if this hitbox's health script already appears in the list
                                Health health = alreadyDamaged.Find(h => hitbox.healthScript == h);
                                if (health == null) // If not
                                {
                                    // Deal damage
                                    int d = Mathf.RoundToInt(damage * damageFalloff.Evaluate(distancePercentage));
                                    hitbox.Damage(d, origin, DamageType.BlownUp, distancePercentage <= criticalRadius);
                                    alreadyDamaged.Add(hitbox.healthScript);
                                }
                            }

                            // Check for the parent rigidbody it is closest to
                            Rigidbody rb = c.GetComponentInParent<Rigidbody>();
                            while (rb != null && rb.isKinematic == true)
                            {
                                // If rigidbody found is kinematic, check ITS parents for one that isn't
                                // rb's parent is specified so GetComponentInParent doesn't check itself and return the previous value
                                rb = rb.transform.parent.GetComponentInParent<Rigidbody>();
                                // Keep checking until one is found, or there are no more
                            }
                            if (rb != null) // If a non-kinematic rigidbody is found
                            {
                                // Check to see if it matches one already in the list
                                Rigidbody alreadyHit = alreadyKnockedBack.Find(r => rb == r);
                                if (alreadyHit == null) // If not, the rigidbody has not been affected by the explosion yet
                                {
                                    // Knock back
                                    float decayedForce = knockback * knockbackFalloff.Evaluate(distancePercentage);
                                    rb.AddForceAtPosition(direction * decayedForce, explosionHit.point, ForceMode.Impulse);
                                }
                            }
                        }
                    }
                }
            }




            // Check explosion stuff
            timer += explosionTime / Time.deltaTime;
            yield return loop;
        }
    }
    */

    /*
    public static bool Has<T>(RaycastHit hit, out T thingFound)
    {
        thingFound = hit.collider.GetComponentInParent<T>();
        return thingFound != null;
    }
    */
    
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
