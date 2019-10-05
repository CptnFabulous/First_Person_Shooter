using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveProjectile : Projectile
{
    public int damage;
    public float directHitModifier;
    public float explosionForce;
    public float blastRadius;
    public float explosionTime;
    public AnimationCurve falloff;

    public override void OnHit()
    {


        base.OnHit();
    }


    /*
    public IEnumerator Explosion(int damage, float explosionForce, float blastRadius, float explosionTime, AnimationCurve falloff)
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

    public void SimpleExplosion(Transform origin, int damage, float directHitModifier, float blastRadius, float explosionTime, AnimationCurve falloff)
    {
        Collider[] affectedObjects = Physics.OverlapSphere(origin.position, blastRadius, rayDetection);
        foreach (Collider c in affectedObjects)
        {
            Vector3 targetDirection = c.transform.position - origin.position;
            RaycastHit isVulnerable;
            if (Physics.Raycast(origin.position, targetDirection, out isVulnerable, blastRadius, rayDetection))
            {
                if (isVulnerable.collider == c)
                {
                    float i = isVulnerable.distance / blastRadius;

                    DamageHitbox hitbox = isVulnerable.collider.GetComponent<DamageHitbox>(); // Checks collider gameObject for a damageHitbox script, and deals damage
                    if (hitbox != null)
                    {
                        float d = damage * falloff.Evaluate(i);
                        hitbox.Damage(Mathf.RoundToInt(d), DamageType.BlownUp);
                    }

                    Rigidbody rb = isVulnerable.collider.GetComponent<Rigidbody>(); // Checks collider gameObject for a rigidbody, and knocks rigidbody back accordingly
                    if (rb != null)
                    {
                        float f = explosionForce * falloff.Evaluate(i);
                        rb.AddForce(targetDirection.normalized * f, ForceMode.Impulse);
                    }
                }
            }
            
        }
    }
}
