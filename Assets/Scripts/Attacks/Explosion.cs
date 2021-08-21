using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Explosion : MonoBehaviour
{
    public int damage = 25;
    public float knockback = 100;
    public float blastRadius = 5;
    public float criticalRadius = 0.2f;
    public float explosionTime = 0.1f;
    public AnimationCurve damageFalloff = AnimationCurve.Linear(0, 1, 1, 0);
    public AnimationCurve knockbackFalloff = AnimationCurve.Linear(0, 1, 1, 0);
    public LayerMask hitDetection = ~0;

    public UnityEvent onExplosionStart;
    public UnityEvent onExplosionEnd;


    public void Explode()
    {
        Explode(transform.position, GetComponent<Entity>());
    }


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


    void ExplodeInstantly(Vector3 position, Entity origin)
    {
        onExplosionStart.Invoke();
        
        List<Health> alreadyDamaged = new List<Health>();
        List<Rigidbody> alreadyKnockedBack = new List<Rigidbody>();
        Collider[] affectedObjects = Physics.OverlapSphere(position, blastRadius, hitDetection);
        foreach (Collider c in affectedObjects)
        {
            Vector3 direction = c.transform.position - position;
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
                            hitbox.Damage(d, null, DamageType.BlownUp, distancePercentage <= criticalRadius);
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
}
