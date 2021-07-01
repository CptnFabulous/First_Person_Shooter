using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAttackingState : StateMachineBehaviour
{
    [HideInInspector] public AICombatant wielder;
    public int attackIndex;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        wielder = animator.GetComponent<AICombatant>();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        wielder.attacks[attackIndex].AttackUpdate();
    }
}
