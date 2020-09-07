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
        //Debug.Log(ai.target.transform);
        targetLocation = ai.target.transform;
        currentDestination = NullableVector3.New(ai.transform.position);
        //currentDestination = FindFollowPosition(targetLocation, minimumRange, maximumRange, numberOfChecks);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (currentDestination != null)
        {
           // For some reason the agent will constantly update its position, even if the target position is still suitable

            float distance = Vector3.Distance(currentDestination.position, targetLocation.position); // Obtains distance between agent and target
            if (AI.LineOfSight(currentDestination.position, targetLocation, ai.transform, coverCriteria) == false || distance < minimumRange || distance > maximumRange) // Checks if agent can no longer see or attack the target from the position, if target is too close to the position, or if target is too far away from the position
            {
                currentDestination = null;
            }
        }

        if (currentDestination == null)
        {
            currentDestination = FindFollowPosition(targetLocation, minimumRange, maximumRange, numberOfChecks);
        }

        if (currentDestination != null)
        {
            ai.na.SetDestination(currentDestination.position);
        }
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
                if (AI.LineOfSight(followCheck.position, target, coverCriteria)) // Checks if line of sight is established between the new position and target. The agent is still pursuing and attacking the target, but they are just staying cautious.
                {
                    // Ensures that the agent can actually move to the cover position.
                    NavMeshPath nmp = new NavMeshPath();
                    if (ai.na.CalculatePath(followCheck.position, nmp))
                    {
                        // Checks if the new cover position is easier to get to than the old one.
                        if (newFollowPosition == null || AI.NavMeshPathLength(nmp) < AI.NavMeshPathLength(followPath)) // Use OR statement, and check navmesh path cost between transform.position and the cover point currently being checked.
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
}
