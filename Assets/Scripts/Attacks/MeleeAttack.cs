using System.Collections;
using System.Collections.Generic;
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
    
    /*
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    */

    public void ExecuteSingle(Character origin, Vector3 attackOrigin, Vector3 direction)
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

        // If there is no target
        if (target == null)
        {
            // Perform animation and 'miss'
        }

        //MonoBehaviour.StartCoroutine(PerformAttack(origin, target));

        //StartCoroutine

        // PERFORM MELEE ATTACK
        
        // Quickly lerp the attacker's position to a space close to the target
        // Quickly lerp the attacker's look direction to face the target
        // Deal damage


    }

    
    IEnumerator PerformPlayerAttack(PlayerHandler origin, DamageHitbox dh)
    {
        float timer = 0;
        // THE FULL METHOD I WANT
        // Calculate a point out from the player, in the direction of the enemy hitbox.
        // Lerp the player towards that position
        // Lerp the player's rotation to face the enemy
        // Do both of these things over the span of attackTime
        // Deal damage
        // Reposition player/enemy so they do not clip into terrain

        // THE SIMPLER METHOD I WILL START OFF WITH
        // Lerp the player's rotation to face the enemy, over the span of attackTime
        // Deal damage


        // STUFF I MAY NEED TO DO IN ORDER TO ACCOMPLISH THIS
        // Make player movement/camera controls into accessible functions that can be triggered from other scripts
        // Do the same thing to AI characters

        //origin.pc.canMove = false;


        while (timer < 1)
        {
            timer += Time.deltaTime / attackTime;
            yield return new WaitForEndOfFrame();
        }


    }
    

}
