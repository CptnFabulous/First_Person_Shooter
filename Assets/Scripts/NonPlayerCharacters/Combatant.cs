using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combatant : NPC
{
    public float casualMoveSpeed;
    public float pursueMoveSpeed;
    public float attackMoveSpeed;

    [Header("Targeting")]
    public GameObject head;
    public GameObject target;

    public float targetRange = 25;
    public float pursueRange = 50; // Max distance NPC will pursue target before running out of patience
    //public float patience = 5; // Amount of time NPC will p
    public LayerMask viewDetecion;

    RaycastHit lookingAt;

    // Update is called once per frame
    void Update()
    {
        
        if (target == null)
        {
            target = AcquireTarget().gameObject;
        }
        
        if (target != null)
        {
            Seek(target, pursueRange);

            

            if (Vector3.Distance(transform.position, target.transform.position) > pursueRange)
            {
                target = null;
            }
        }
        else
        {
            // Perform idle behaviour
        }
    }

    Character AcquireTarget()
    {
        Collider[] thingsInEnvironment = Physics.OverlapSphere(head.transform.position, targetRange);
        foreach (Collider c in thingsInEnvironment)
        {
            RaycastHit lineOfSight;
            if (Physics.Raycast(head.transform.position, c.transform.position - head.transform.position, out lineOfSight, pursueRange, viewDetecion) && lineOfSight.collider == c)
            {
                Character targetCharacter = c.GetComponent<Character>();
                if (targetCharacter != null && ch.faction.Affiliation(targetCharacter.faction) == FactionState.Hostile)
                {
                    return targetCharacter;
                }
            }
        }
        return null;
    }
}
