using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
using UnityEngine;
using UnityEngine.AI;

public class AvoidAttack : AIMovementBehaviour
{
    NullableVector3 safeLocation;

    public float destinationThreshold = 0.1f;

    public float minCheckRadius = 2;
    public float maxCheckRadius;
    public float maxMoveDistance;
    public int numberOfChecks = 15;
    public LayerMask coverCriteria;



    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        Debug.Log(ai.name + " is entering dodge state");

        safeLocation = FindAvoidPosition();
        // If the NPC cannot find a safe location, end the state machine behaviour. The NPC will be forced to tank the attack, as there is nowhere for it to dodge to.
        if (safeLocation == null)
        {
            EndDodge();
        }
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Setting destination for " + ai.name + " from EngageTarget behaviour");

        // If a destination was found but has not been reached yet, go towards the destination
        if (safeLocation != null && ai.na.remainingDistance > destinationThreshold)
        {
            ai.na.SetDestination(safeLocation.position);
        }
        else
        {
            // Otherwise, end the dodge state
            EndDodge();
        }
    }

    public NullableVector3 FindAvoidPosition() // Should I have separate versions for dodging vs. taking cover? I might need this based on whether the enemy is aggressive or skittish
    {
        NullableVector3 newSafeLocation = null;
        float maxPathDistance = maxMoveDistance;

        for (int i = 0; i < numberOfChecks; i++)
        {
            // Samples a random position around the target, normalises it, and randomises the magnitude to a point in betwen the min and max radii.
            // If I simply multiply by the max check radius, the position may be too close.
            Vector3 randomPosition = ai.transform.position + Random.insideUnitSphere.normalized * Random.Range(minCheckRadius, maxCheckRadius);
            // Normalising the Random.insideUnitSphere ensures the magnitude (and therefore distance value) is always 1, and the distance is calculated correctly.

            NavMeshHit followCheck;
            // Checks if there is an actual point on the navmesh close to the randomly selected position
            if (NavMesh.SamplePosition(randomPosition, out followCheck, ai.na.height * 2, NavMesh.AllAreas))
            {
                // Checks if the location is safe from the attack
                if (ai.attackToDodge.IsPositionSafe(followCheck.position, ai.characterData.HealthData.hitboxes) == false)
                {
                    // Creates a new path for reference
                    NavMeshPath nmp = new NavMeshPath();
                    // If the agent can actually move to the location
                    if (ai.na.CalculatePath(followCheck.position, nmp) && nmp.status == NavMeshPathStatus.PathComplete)
                    {
                        float distance = AIFunction.NavMeshPathLength(nmp); // Check navmesh path cost between transform.position and the cover point currently being checked.

                        if (distance < maxPathDistance) // If the NPC is willing to travel that distance to the dodge zone, or if this distance is shorter than that of the previous route.
                        {
                            // If so, new cover position is established, and navmesh path is stored for next comparison
                            newSafeLocation = new NullableVector3(followCheck.position);
                            maxPathDistance = distance;
                        }
                    }
                }
            }
        }

        return newSafeLocation;
    }


    










    void EndDodge()
    {
        ai.attackToDodge = null;
        ai.aiStateMachine.SetBool("mustDodgeAttack", false); // Disables state machine bool for dodging attack, so the agent moves back to its normal routine
    }


    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
    }
}