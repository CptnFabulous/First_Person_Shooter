using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idle : AIMovementBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        ai.na.isStopped = true;
    }
}
