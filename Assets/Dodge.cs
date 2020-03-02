using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dodge : AIMovementBehaviour
{
    public float dodgeDistance;

    Transform dodgeLocation;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        Transform attacker; // Somehow calculate how the enemy decides to dodge an attacker

    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ai.na.SetDestination(dodgeLocation.position);
    }
}
