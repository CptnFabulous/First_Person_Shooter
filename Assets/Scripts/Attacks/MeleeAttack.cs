using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEditor;
using UnityEngine;
//using System.Linq;

[System.Serializable]
public class MeleeAttack
{
    public int damage;
    public float range;
    public float angle;
    public float attackTime;
    public LayerMask hitDetection;


    bool isAttacking;


    


    public IEnumerator PlayerSnapAttack(PlayerHandler origin, Vector3 attackOrigin, Vector3 direction)
    {
        DamageHitbox target = FindEnemyHitbox(origin, attackOrigin, direction);

        if (target == null)
        {
            //Play swing animation
        }


        // Sets up loop
        isAttacking = true;
        float timer = 0;


        // Temporarily disables player controller
        origin.pc.canMove = false;




        Vector3 oldLookDirection = origin.pc.head.transform.forward;
        Vector3 oldPosition = origin.transform.position;

        while (timer <= 1)
        {
            // Count up timer
            timer += Time.deltaTime / attackTime;

            // If the isAttacking bool has been remotely disabled, and end attack prematurely
            if (isAttacking == false)
            {
                EndAttack();
                yield break;
            }

            // Lerp player rotation so they are looking at the enemy
            Vector3 newLookDirection = target.transform.position - origin.transform.position;
            origin.pc.LookAt(Vector3.Lerp(oldLookDirection, newLookDirection, timer));

            // Lerp player position closer to enemy

            yield return new WaitForEndOfFrame();
        }

        // Execute attack (play animation and deal damage)
        target.Damage(damage, origin, DamageType.Bludgeoned, false);


        EndAttack();
    }

   

    void EndAttack()
    {
        
        // Re-enable player controller
        
        
        
        isAttacking = false;
    }





    DamageHitbox FindEnemyHitbox(Character origin, Vector3 attackOrigin, Vector3 direction)
    {
        // Run a vision cone to check for entities inside the attack angle and range
        RaycastHit[] hits = AIFunction.VisionCone(attackOrigin, direction, origin.transform.up, angle, range, hitDetection, hitDetection);

        // Create a 'target' variable to determine who the 
        DamageHitbox target = null;
        float bestRange = range * 2;

        // Determine the closest hitbox that is an enemy of the attacker
        foreach (RaycastHit rh in hits)
        {
            // Check for damage hitbox script
            DamageHitbox closestHitbox = rh.collider.GetComponent<DamageHitbox>();
            if (closestHitbox != null)
            {
                // Check damage hitbox for character script (if the object detected is part of an entity who can be damaged)
                Character ch = Character.FromObject(closestHitbox.gameObject);
                if (ch != null && origin.HostileTowards(ch))
                {
                    // Check if the collider is inside the angle and range to find the hitbox closest to the origin
                    float r = AIFunction.ComplexColliderDistance(rh.collider, attackOrigin);
                    float a = AIFunction.ComplexColliderAngle(rh.collider, attackOrigin, direction);
                    if (r < bestRange && a < angle)
                    {
                        target = closestHitbox;
                        bestRange = r;
                    }
                }
            }
        }

        return target;
    }

    public void Cancel()
    {
        isAttacking = false;
    }

}
