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
            EndAvoidance();
        }
    }

    public NullableVector3 FindAvoidPosition() // Should I have separate versions for dodging vs. taking cover? I might need this based on whether the enemy is aggressive or skittish
    {
        Collider[] characterColliders = ai.GetComponentsInChildren<Collider>();

        NullableVector3 newSafeLocation = null;
        NavMeshPath referencePath = null;

        for (int i = 0; i < numberOfChecks; i++)
        {
            Vector3 randomPosition = ai.transform.position + Random.insideUnitSphere * maximumRange; // Samples a random position around the target, inside maximumRange.
            // Normalising the Random.insideUnitSphere ensures the magnitude (and therefore distance value) is always 1, and the distance is calculated correctly.

            NavMeshHit followCheck;
            // Checks if there is an actual point on the navmesh close to the randomly selected position
            if (NavMesh.SamplePosition(randomPosition, out followCheck, ai.na.height * 2, NavMesh.AllAreas))
            {
                if (attackToDodge.AtRisk(followCheck.position, characterColliders) == false) // Checks if the location is safe from the attack
                {
                    // Creates a new path for reference
                    NavMeshPath nmp = new NavMeshPath();

                    // If the agent can actually move to the location
                    if (ai.na.CalculatePath(followCheck.position, nmp))
                    {
                        // Check navmesh path cost between transform.position and the cover point currently being checked.
                        bool pathIsBetter = AI.NavMeshPathLength(nmp) < AI.NavMeshPathLength(referencePath); // Checks if the new cover position is easier to get to than the old one.


                        if (newSafeLocation == null || pathIsBetter == true) // If there is no path specified, or the new path is better than the old one
                        {
                            // If so, new cover position is established, and navmesh path is stored for next comparison
                            newSafeLocation = NullableVector3.New(followCheck.position);
                            referencePath = nmp;
                        }
                    }
                }
            }
        }

        // If the NPC cannot find a safe location, end the state machine behaviour. The NPC will be forced to tank the attack, as there is nowhere for it to dodge to.
        if (newSafeLocation == null)
        {
            EndAvoidance();
        }

        return newSafeLocation;
    }

    void EndAvoidance()
    {
        ai.attackToDodge = null;
        ai.stateMachine.SetBool("mustDodgeAttack", false); // Disables state machine bool for dodging attack, so the agent moves back to its normal routine
    }
}