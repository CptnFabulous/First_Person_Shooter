using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EngageTarget : AIMovementBehaviour
{
    NullableVector3 currentDestination;

    [Header("Detection ranges")]
    public float minimumMoveRange = 15;
    public float maximumMoveRange = 45;
    public float minimumDestinationRange = 20;
    public float maximumDestinationRange = 35;

    [Header("Additional detection stats")]
    public int numberOfChecks = 15;
    public LayerMask coverCriteria = ~0;


    Vector3 TargetLocation
    {
        get
        {
            return ai.currentTarget.transform.position;
        }
    }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        #region Check validity of destination, and return null if no longer valid
        if (currentDestination != null)
        {
            float distance = Vector3.Distance(currentDestination.position, TargetLocation); // Obtains distance between agent and target

            Debug.DrawLine(TargetLocation, currentDestination.position, new Color(1, 0.5f, 0));

            // Checks if the AI can no longer see the target from their desired position, or if they are too close or too far
            bool lineOfSightLost = !AIFunction.LineOfSightCheckWithExceptions(TargetLocation, currentDestination.position, coverCriteria, ai.characterData.health.hitboxes, ai.currentTarget.health.hitboxes);
            bool tooClose = distance < minimumMoveRange;
            bool tooFar = distance > maximumMoveRange;
            if (lineOfSightLost || tooClose || tooFar)
            {
                // The AI cannot engage with the current target, position is nulled.
                currentDestination = null;
            }
            //Debug.Log(ai.name + " pathfinding status: " + lineOfSightLost + ", " + tooClose + ", " + tooFar);
        }
        #endregion

        #region Find new destination if there is none
        // If there is no position assigned, search for one.
        if (currentDestination == null)
        {
            //Debug.Log("Finding destination normally");
            currentDestination = FindFollowPosition(TargetLocation, minimumDestinationRange, maximumDestinationRange, numberOfChecks);

            //Debug.Log("Current destination = " + currentDestination);
            
        }
        #endregion

        #region Travel to destination
        // If a valid position is found the agent must travel to it.
        if (currentDestination != null)
        {
            Debug.DrawLine(ai.transform.position, currentDestination.position, Color.magenta);
            ai.na.SetDestination(currentDestination.position);
            Debug.DrawLine(currentDestination.position, ai.na.destination, new Color(1, 0.75f, 0), 1);
        }
        #endregion
    }

    public NullableVector3 FindFollowPosition(Vector3 targetPosition, float minimumRange, float maximumRange, int numberOfChecks)
    {
        NullableVector3 newFollowPosition = null;
        float currentPathLength = float.MaxValue;
        /*
        int noValidPosition = 0;
        int noLineOfSight = 0;
        int noValidPath = 0;
        int incompletePath = 0;
        int inefficient = 0;
        */
        for (int i = 0; i < numberOfChecks; i++)
        {
            // Samples a random position around the target, outside minimumRange and inside maximumRange.
            // Normalising the Random.insideUnitSphere magnitude then multiplying it again by another random value allows me to ensure that the distance of the point is random but still within certain distance requirements.
            Vector3 randomPosition = targetPosition + Random.insideUnitSphere.normalized * Random.Range(minimumRange, maximumRange);

            NavMeshHit followCheck;
            // Checks if there is an actual point on the navmesh close to the randomly selected position
            if (NavMesh.SamplePosition(randomPosition, out followCheck, ai.na.height * 2, NavMesh.AllAreas))
            {
                Debug.DrawLine(randomPosition, followCheck.position, Colours.darkGreen);
                
                // Checks if line of sight is established between the new position and target. The agent is still pursuing and attacking the target, but they are just staying cautious.
                if (AIFunction.LineOfSightCheckWithExceptions(targetPosition, followCheck.position, coverCriteria, ai.characterData.health.hitboxes, ai.currentTarget.health.hitboxes))
                {
                    // Ensures that a path can be sampled to the destination.
                    NavMeshPath nmp = new NavMeshPath();
                    if (ai.na.CalculatePath(followCheck.position, nmp))
                    {
                        // Checks to make sure a whole path can be found.
                        // This if statement could probably be added to the previous one with the && operator
                        if (nmp.status == NavMeshPathStatus.PathComplete)
                        {
                            // Checks if the new cover position is a shorter route to get to than the old one.
                            // Use OR statement, and check navmesh path cost between transform.position and the cover point currently being checked.
                            float length = AIFunction.NavMeshPathLength(nmp);
                            if (newFollowPosition == null || length < currentPathLength)
                            {
                                // If so, new cover position is established, and navmesh path is stored for next comparison
                                newFollowPosition = new NullableVector3(followCheck.position);
                                currentPathLength = length;
                            }
                            /*
                            else
                            {
                                inefficient++;
                            }
                            */
                        }
                        /*
                        else
                        {
                            incompletePath++;
                        }
                        */
                    }
                    /*
                    else
                    {
                        noValidPath++;
                    }
                    */
                }
                /*
                else
                {
                    noLineOfSight++;
                }
                */
            }
            /*
            else
            {
                noValidPosition++;
            }
            */
        }

        //Debug.Log("Path result - " + (newFollowPosition != null).ToString() + ", " + noValidPosition + " sampling errors, " + noLineOfSight + " line of sight fails, " + noValidPath + " pathing fails, " + incompletePath + " incomplete paths, and " + inefficient + " inefficient paths.");

        return newFollowPosition;
    }


    




    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {


        base.OnStateExit(animator, stateInfo, layerIndex);
    }
}
