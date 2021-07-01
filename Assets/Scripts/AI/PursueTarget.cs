using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Do I need to add code to have the agent stop after a certain distance.

public class PursueTarget : AIMovementBehaviour
{
    public float maxRange;

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Vector3.Distance(ai.transform.position, ai.currentTarget.transform.position) > maxRange)
        {
            ai.na.SetDestination(ai.currentTarget.transform.position);
        }
    } 
}
