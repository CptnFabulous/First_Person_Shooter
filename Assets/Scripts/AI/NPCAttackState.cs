using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NPCAttackType
{
    Melee,
    Projectile,
    Throwable
}

public class NPCAttackState : StateMachineBehaviour
{
    public Combatant c;
    public NPCAttackType type;
    public int attackIndex;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        c = animator.GetComponent<Combatant>();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        switch (type)
        {
            case NPCAttackType.Melee:
                //c.projectileAttacks[attackIndex].TargetEnemy(); Run attack from AI's attack array based on index
                break;
            case NPCAttackType.Projectile:
                //c.projectileAttacks[attackIndex].TargetEnemy(); Run attack from AI's attack array based on index
                break;
            case NPCAttackType.Throwable:
                //c.projectileAttacks[attackIndex].TargetEnemy(); Run attack from AI's attack array based on index
                break;
            default:

                break;
        }
    }

}
