using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAttackingState : StateMachineBehaviour
{
    [HideInInspector] public AICombatant wielder;
    public int attackIndex;
    public AIMovementBehaviour currentMovementBehaviour;

    AIAttack currentAttack;


    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        wielder = animator.GetComponent<AICombatant>();
        currentAttack = wielder.attacks[attackIndex];
        currentAttack.currentMovementBehaviour = currentMovementBehaviour;
        currentAttack.StateStart();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        currentAttack.AttackUpdate();
    }
}
