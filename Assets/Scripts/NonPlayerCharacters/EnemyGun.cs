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
    
    public float standardMoveSpeed;

    [Header("Targeting")]
    public GameObject head;
    public GameObject target;
    float pursueRange = 50;
    public float aimSpeed;
    public float targetThreshold = 0.1f;
    RaycastHit lookingAt;

    [Header("Attacks")]
    public ProjectileBurst attack;

    // Update is called once per frame
    void Update()
    {

        if (target != null)
        {
            Seek(target, pursueRange);

            attack.TargetEnemy(target, gameObject, head, targetThreshold, aimSpeed, lookingAt);
            na.enabled = !attack.isAttacking;
        }
    }

    void FindTarget()
    {

    }
}
