using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class NPCMeleeAttack : NPCAttack
{
    [Header("Melee attack stats")]
    public int damage = 10;
    public Vector2 attackAngles;

    NullableVector3 oldNavMeshTarget;
    NullableVector3 attackTargetLocation;


    public override void TelegraphAttack()
    {
        base.TelegraphAttack();
        // Perform telegraph functions, and transmit attack message
        oldNavMeshTarget.position = c.na.destination;
    }

    public override void ExecuteAttack()
    {
        base.ExecuteAttack();
        // Perform melee attack
    }
}