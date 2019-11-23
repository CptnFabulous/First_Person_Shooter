using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Soldier : NPC
{
    [Header("General stats")]
    public float moveSpeed;

    [Header("Targeting")]
    public GameObject head;
    public GameObject target;

    public float targetRange = 25;
    public float pursueRange = 50; // Max distance NPC will pursue target before running out of patience
    //public float patience = 5; // Amount of time NPC will p
    public LayerMask viewDetection;

    RaycastHit lookingAt;

    [Header("Attacks")]
    public ProjectileBurst attack;

    [Header("Animations")]
    Animator a;


    public override void Awake()
    {
        base.Awake();
        a = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null)
        {
            Character t = AcquireTarget();
            if (t != null)
            {
                target = t.gameObject;
            }
        }

        if (target != null)
        {
            if (Vector3.Distance(transform.position, target.transform.position) > attack.range || LineOfSight(target, pursueRange) == false)
            {
                Seek(target, pursueRange);
            }
            else
            {
                StandStill();
            }


            //attack.TargetEnemy(target, gameObject, na, moveSpeed, ch.faction, head.transform, lookingAt, audioSource);

            Health h = target.GetComponent<Health>();
            if (h != null && h.health.current <= 0)
            {
                target = null;
            }

            if (Vector3.Distance(transform.position, target.transform.position) > pursueRange && attack.isAttacking == false)
            {
                target = null;
            }
        }
        else
        {
            // Perform idle behaviour
            StandStill();
        }

        if (attack.isAttacking)
        {
            a.SetBool("IsShooting", true);
        }
        else if (Vector3.Distance(transform.position, na.destination) > 0)
        {
            a.SetBool("IsShooting", false);
            a.SetBool("IsMoving", true);
        }
        else
        {
            a.SetBool("IsShooting", false);
            a.SetBool("IsMoving", false);
        }
    }

    Character AcquireTarget()
    {
        Collider[] thingsInEnvironment = Physics.OverlapSphere(head.transform.position, targetRange);
        foreach (Collider c in thingsInEnvironment)
        {
            if (LineOfSight(c.gameObject, targetRange))
            {
                Character targetCharacter = c.transform.root.GetComponent<Character>();
                if (targetCharacter != null && ch.faction.Affiliation(targetCharacter.faction) == FactionState.Hostile)
                {
                    return targetCharacter;
                }
            }

            /*
            RaycastHit lineOfSight;
            if (Physics.Raycast(head.transform.position, c.transform.position - head.transform.position, out lineOfSight, pursueRange, viewDetection) && lineOfSight.collider == c)
            {
                Character targetCharacter = c.transform.root.GetComponent<Character>();
                if (targetCharacter != null && ch.faction.Affiliation(targetCharacter.faction) == FactionState.Hostile)
                {
                    return targetCharacter;
                }
            }
            */
        }
        return null;
    }

    bool LineOfSight(GameObject target, float range)
    {
        RaycastHit lineOfSight;
        if (Physics.Raycast(head.transform.position, target.transform.position - head.transform.position, out lineOfSight, range, viewDetection))
        {
            if (lineOfSight.collider.gameObject == target)
            {
                return true;
            }
        }
        return false;
    }

}
