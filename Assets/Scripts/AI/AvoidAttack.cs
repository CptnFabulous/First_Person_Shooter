using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
using UnityEngine;
using UnityEngine.AI;

public class AvoidAttack : AIMovementBehaviour
{
    AttackMessage attackToDodge;
    Transform targetLocation;
    NullableVector3 safeLocation;

    public float destinationThreshold = 0.1f;

    public float maximumRange = 10;
    public int numberOfChecks = 15;
    public LayerMask coverCriteria = ~0;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        safeLocation = FindAvoidPosition();
        attackToDodge = ai.attackToDodge;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ai.na.SetDestination(safeLocation.position);
        if (Vector3.Distance(ai.transform.position, safeLocation.position) <= destinationThreshold) // If enemy has reached the location, i.e. finished dodging
        {
            ai.attackToDodge = null;
            ai.stateMachine.SetBool("mustDodgeAttack", false);
        }
    }

    public NullableVector3 FindAvoidPosition() // Should I have separate versions for dodging vs. taking cover? I might need this based on whether the enemy is aggressive or skittish
    {
        NullableVector3 newSafeLocation = null;
        NavMeshPath followPath = null;

        for (int i = 0; i < numberOfChecks; i++)
        {
            Vector3 randomPosition = ai.transform.position + Random.insideUnitSphere * maximumRange; // Samples a random position around the target, inside maximumRange.
            // Normalising the Random.insideUnitSphere ensures the magnitude (and therefore distance value) is always 1, and the distance is calculated correctly.

            NavMeshHit followCheck;
            // Checks if there is an actual point on the navmesh close to the randomly selected position
            if (NavMesh.SamplePosition(randomPosition, out followCheck, ai.na.height * 2, NavMesh.AllAreas))
            {
                if (attackToDodge.AtRisk(followCheck.position) == false) // Checks if the location is safe from the attack
                {
                    // Ensures that the agent can actually move to the cover position.
                    NavMeshPath nmp = new NavMeshPath();
                    if (ai.na.CalculatePath(followCheck.position, nmp))
                    {
                        // Checks if the new cover position is easier to get to than the old one.
                        if (newSafeLocation == null || AI.NavMeshPathLength(nmp) < AI.NavMeshPathLength(followPath)) // Use OR statement, and check navmesh path cost between transform.position and the cover point currently being checked.
                        {
                            // If so, new cover position is established, and navmesh path is stored for next comparison
                            newSafeLocation = NullableVector3.New(followCheck.position);
                            followPath = nmp;
                        }
                    }
                }
            }
        }

        return newSafeLocation;
    }
}