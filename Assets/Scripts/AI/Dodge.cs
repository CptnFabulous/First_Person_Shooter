using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dodge : AIMovementBehaviour
{
    public float dodgeRadius; // The minimum distance required to dodge
    public LayerMask terrainDetection;
    public int checkRaycastNumber = 8;

    public Transform attacker;

    Transform dodgeLocation;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        //Transform attacker; // Somehow calculate how the enemy decides what constitutes an attacker
        Vector3 attackerDirection = attacker.position - ai.transform.position;
        // Find locations perpendicular to the attacker


        Quaternion scanDirection = Quaternion.Euler(90, 0, 0);
        for (int i = 0; i < checkRaycastNumber; i++)
        {
            if (Physics.Raycast(ai.transform.position, scanDirection * attackerDirection, dodgeRadius, terrainDetection))
            {

            }
            i += 1;

            scanDirection = Quaternion.Euler(90, 0, scanDirection.z + (360 / checkRaycastNumber));
        }



    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ai.na.SetDestination(dodgeLocation.position);
    }
}
