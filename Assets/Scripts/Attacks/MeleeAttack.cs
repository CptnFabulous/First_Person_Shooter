using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using System.Linq;

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
        // // Run a vision cone to check for entities inside the attack angle and range
        RaycastHit[] hits = AIFunction.VisionCone(attackOrigin, direction, origin.transform.up, angle, range, hitDetection, hitDetection);

        List<RaycastHit> h;
        //h.Sort()

        DamageHitbox target;
        //float angle;
        //float range;

        foreach(RaycastHit rh in hits)
        {
            // Determine the closest hitbox that is an enemy of the attacker

            // Check for damage hitbox script
            DamageHitbox dh = rh.collider.GetComponent<DamageHitbox>();
            if (dh != null)
            {
                // Check damage hitbox for character script (if the object detected is part of an entity who can be damaged)
                Character ch = Character.FromObject(dh.gameObject);
                if (ch != null && origin.HostileTowards(ch))
                {
                    
                    
                    // Check if the collider is inside the angle and range
                    if (AIFunction.ComplexColliderDistance(rh.collider, attackOrigin) < range && AIFunction.ComplexColliderAngle(rh.collider, attackOrigin, direction) < angle)
                    {
                        // Check angles and ranges to find the hitbox closest to the origin
                    }
                }
            }
            //if ()
        }
        // Figure out a way to sort hitboxes based on distance and relevance
        

        
        // Quickly lerp the attacker's position to a space close to the target
        // Quickly lerp the attacker's look direction to face the target
        // Deal damage



        //DamageHitbox dh;
    }


}
