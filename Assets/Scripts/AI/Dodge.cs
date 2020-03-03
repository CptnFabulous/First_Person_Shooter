using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dodge : AIMovementBehaviour
{
    public float dodgeDistance;
    public Transform attacker;

    Transform dodgeLocation;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        //Transform attacker; // Somehow calculate how the enemy decides what constitutes an attacker
        Vector3 attackerDirection = attacker.position - ai.transform.position;
        // Find locations perpendicular to the attacker

    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ai.na.SetDestination(dodgeLocation.position);
    }
}
