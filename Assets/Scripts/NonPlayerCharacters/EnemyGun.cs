/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof (NavMeshAgent))]
public class EnemyGun : MonoBehaviour
{
    NavMeshAgent na;

    public float standardMoveSpeed;
    //public float attackingMoveSpeed;

    [Header("Targeting")]
    public GameObject head;
    public GameObject target;
    public Transform a;
    public float aimSpeed;
    public float targetThreshold = 0.1f;
    RaycastHit lookingAt;

    [Header("Attacks")]
    public ProjectileBurst attack;

    private void Awake()
    {
        na = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {

        if (target != null)
        {
            na.destination = target.transform.position;

            attack.TargetEnemy(target, gameObject, head, a, targetThreshold, aimSpeed, lookingAt);
            na.enabled = !attack.isAttacking;
        }
    }
}
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyGun : NPC
{
    [Header("Targeting")]
    public GameObject head;
    public GameObject target;

    public float targetRange = 25;
    public float pursueRange = 50; // Max distance NPC will pursue target before running out of patience
    //public float patience = 5; // Amount of time NPC will p
    public LayerMask viewDetecion;

    RaycastHit lookingAt;

    [Header("Attacks")]
    public ProjectileBurst attack;

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
            Seek(target, pursueRange);

            attack.TargetEnemy(target, gameObject, ch.faction, head.transform, lookingAt);
            na.enabled = !attack.isAttacking;

            if (target.GetComponent<Health>().health.current <= 0)
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
    }

    Character AcquireTarget()
    {
        Collider[] thingsInEnvironment = Physics.OverlapSphere(head.transform.position, targetRange);
        foreach(Collider c in thingsInEnvironment)
        {
            RaycastHit lineOfSight;
            if (Physics.Raycast(head.transform.position, c.transform.position - head.transform.position, out lineOfSight, pursueRange, viewDetecion) && lineOfSight.collider == c)
            {
                Character targetCharacter = c.transform.root.GetComponent<Character>();
                if (targetCharacter != null && ch.faction.Affiliation(targetCharacter.faction) == FactionState.Hostile)
                {
                    return targetCharacter;
                }
            }
        }
        return null;
    }
}
