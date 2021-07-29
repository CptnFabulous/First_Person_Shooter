using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EngageTarget : AIMovementBehaviour
{
    Transform targetLocation;
    NullableVector3 currentDestination;

    [Header("Detection ranges")]
    public float minimumMoveRange = 15;
    public float maximumMoveRange = 45;
    public float minimumDestinationRange = 20;
    public float maximumDestinationRange = 35;

    [Header("Additional detection stats")]
    public int numberOfChecks = 15;
    public LayerMask coverCriteria = ~0;

    Collider[] collidersToIgnoreWhenPerformingLineOfSightChecks;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        // Sets the location of the current target
        targetLocation = ai.currentTarget.transform;

        collidersToIgnoreWhenPerformingLineOfSightChecks = ai.GetComponentsInChildren<Collider>();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        #region Check validity of destination, and return null if no longer valid
        if (currentDestination != null)
        {
            float distance = Vector3.Distance(currentDestination.position, targetLocation.position); // Obtains distance between agent and target

            //bool lineOfSightLost = AIFunction.LineOfSight(currentDestination.position, targetLocation, ai.transform, coverCriteria) == false;
            

            // Checks if the enemy loses line of sight, is too close or too far
            bool lineOfSightLost = AIFunction.SimpleLineOfSightCheck(targetLocation.position, currentDestination.position, coverCriteria, collidersToIgnoreWhenPerformingLineOfSightChecks);
            bool tooClose = distance < minimumMoveRange;
            bool tooFar = distance > maximumMoveRange;
            if (lineOfSightLost || tooClose || tooFar) // Checks if agent can no longer see or attack the target from the position, if target is too close to the position, or if target is too far away from the position
            {
                Debug.Log("Destination is now nulll");
                // If one of these are true, the AI cannot engage with the current target. The position is nulled so a new position can be found.
                currentDestination = null;
            }
        }
        #endregion

        #region Find new destination if there is none
        // If there is no position assigned, search for one.
        if (currentDestination == null)
        {
            Debug.Log("Finding destination normally");
            currentDestination = FindFollowPosition(targetLocation, minimumDestinationRange, maximumDestinationRange, numberOfChecks);
        }
        #endregion

        #region Travel to destination
        // If a valid position is found the agent must travel to it.
        if (currentDestination != null)
        {
            //Debug.Log("Assigning destination");
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
                            newFollowPosition = new NullableVector3(followCheck.position);
                            followPath = nmp;
                        }
                    }
                }
            }
        }

        Debug.Log("Assigning new position, " + newFollowPosition.position + ", on frame " + Time.frameCount);

        return newFollowPosition;
    }







    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {


        base.OnStateExit(animator, stateInfo, layerIndex);
    }
}
