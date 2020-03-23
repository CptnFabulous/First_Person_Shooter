using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAttackBehaviour : StateMachineBehaviour
{
    [HideInInspector] public AI ai;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ai = animator.GetComponent<AI>();
    }
}
