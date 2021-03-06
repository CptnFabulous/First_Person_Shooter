﻿using System.Collections;
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
    [HideInInspector] public Combatant c;
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
                //c.projectileAttacks[attackIndex].Update(); // Run attack from AI's attack array based on index
                break;
            case NPCAttackType.Projectile:
                if (c.projectileAttacks.Length > 0) // If the enemy has any attacks
                {
                    c.projectileAttacks[attackIndex].Update(); // Run attack from AI's attack array based on index
                }
                break;
            case NPCAttackType.Throwable:
                //c.projectileAttacks[attackIndex].Update(); // Run attack from AI's attack array based on index
                break;
            default:

                break;
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        

        base.OnStateExit(animator, stateInfo, layerIndex);


    }


    void Update()
    {
        switch (type)
        {
            case NPCAttackType.Melee:
                //c.projectileAttacks[attackIndex].Update(); // Run attack from AI's attack array based on index
                break;
            case NPCAttackType.Projectile:
                if (c.projectileAttacks.Length > 0) // If the enemy has any attacks
                {
                    c.projectileAttacks[attackIndex].Update(); // Run attack from AI's attack array based on index
                }
                break;
            case NPCAttackType.Throwable:
                //c.projectileAttacks[attackIndex].TargetEnemy(); // Run attack from AI's attack array based on index
                break;
            default:

                break;
        }
    }

    void EndAttack()
    {
        switch (type)
        {
            case NPCAttackType.Melee:
                //c.projectileAttacks[attackIndex].EndAttack(); // Run attack from AI's attack array based on index
                break;
            case NPCAttackType.Projectile:
                if (c.projectileAttacks.Length > 0) // If the enemy has any attacks
                {
                    c.projectileAttacks[attackIndex].EndAttack(); // Run attack from AI's attack array based on index
                }
                break;
            case NPCAttackType.Throwable:
                //c.projectileAttacks[attackIndex].EndAttack(); // Run attack from AI's attack array based on index
                break;
            default:

                break;
        }
    }

}
