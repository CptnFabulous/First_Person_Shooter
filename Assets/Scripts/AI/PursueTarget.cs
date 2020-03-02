using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PursueTarget : AIMovementBehaviour
{
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ai.na.SetDestination(ai.target.transform.position);
    }
}
