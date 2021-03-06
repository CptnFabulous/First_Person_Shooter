﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EngageTarget : AIMovementBehaviour
{
    Transform targetLocation;
    NullableVector3 currentDestination;

    public float minimumRange = 10;
    public float maximumRange = 20;
    public int numberOfChecks = 15;
    public LayerMask coverCriteria = ~0;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        targetLocation = ai.currentTarget.transform;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        #region Check validity of destination, and return null if no longer valid
        if (currentDestination != null)
        {
           // Checks if the agent's position is still ideal or not.
            float distance = Vector3.Distance(currentDestination.position, targetLocation.position); // Obtains distance between agent and target
            if (AIFunction.LineOfSight(currentDestination.position, targetLocation, ai.transform, coverCriteria) == false || distance < minimumRange || distance > maximumRange) // Checks if agent can no longer see or attack the target from the position, if target is too close to the position, or if target is too far away from the position
            {
                // If not, position is nulled so a new position can be found.
                currentDestination = null;
            }
        }
        #endregion

        #region Find new destination if there is none
        // If there is no position assigned, search for one.
        if (currentDestination == null)
        {
            currentDestination = FindFollowPosition(targetLocation, minimumRange, maximumRange, numberOfChecks);
        }
        #endregion

        #region Travel to destination
        // If a valid position is found the agent must travel to it.
        if (currentDestination != null)
        {
            ai.na.SetDestination(currentDestination.position);
        }
        #endregion
    }

    public NullableVector3 FindFollowPosition(Transform target, float minimumRange, float maximumRange, int numberOfChecks)
    {
        NullableVector3 newFollowPosition = null;
        NavMeshPath followPath = null;

        for (int i = 0; i < numberOfChecks; i++)
        {
            Vector3 randomPosition = target.position + Random.insideUnitSphere.normalized * Random.Range(minimumRange, maximumRange); // Samples a random position around the target, outside minimumRange and inside maximumRange.
            // Normalising the Random.insideUnitSphere ensures the magnitude (and therefore distance value) is always 1, and the distance is calculated correctly.

            NavMeshHit followCheck;
            // Checks if there is an actual point on the navmesh close to the randomly selected position
            if (NavMesh.SamplePosition(randomPosition, out followCheck, ai.na.height * 2, NavMesh.AllAreas))
            {
                if (AIFunction.LineOfSight(followCheck.position, target, coverCriteria)) // Checks if line of sight is established between the new position and target. The agent is still pursuing and attacking the target, but they are just staying cautious.
                {
                    // Ensures that the agent can actually move to the cover position.
                    NavMeshPath nmp = new NavMeshPath();
                    if (ai.na.CalculatePath(followCheck.position, nmp))
                    {
                        // Checks if the new cover position is easier to get to than the old one.
                        if (newFollowPosition == null || AIFunction.NavMeshPathLength(nmp) < AIFunction.NavMeshPathLength(followPath)) // Use OR statement, and check navmesh path cost between transform.position and the cover point currently being checked.
                        {
                            // If so, new cover position is established, and navmesh path is stored for next comparison
                            newFollowPosition = NullableVector3.New(followCheck.position);
                            followPath = nmp;
                        }
                    }
                }
            }
        }

        return newFollowPosition;
    }







    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {


        base.OnStateExit(animator, stateInfo, layerIndex);
    }
}
