using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// LINE OF SIGHT CHECK TO ENSURE COVER HAS NOT BEEN COMPROMISED: Since the line of sight check is not checking layers that the attacker is on, it is not detecting the attacker and is assuming that the attacker cannot attack them. This needs to be fixed!

public class TakeCover : AIMovementBehaviour
{
    [Tooltip("How far away will the enemy look for cover points? If this distance is too large, it can cause lag when calculating paths")]
    public float coverCheckRadius = 10;
    public int numberOfChecks = 15;
    public LayerMask coverCriteria;

    Transform attacker;

    NullableVector3 currentCover = null;


    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        attacker = ai.target.transform; // Placeholder, replace with a better way to detect an attacker

        currentCover = FindCover(attacker, ai.na, coverCheckRadius, numberOfChecks, coverCriteria);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (currentCover != null)
        {
            Debug.Log("Enemy is taking cover");
            
            if (AI.LineOfSight(currentCover.position, attacker, coverCriteria))
            {
                Debug.Log("Cover is compromised");
                // Reset and find a new cover point
                currentCover = null;
                //currentCover = FindCover(attacker, ai.na, coverCheckRadius, numberOfChecks, coverCriteria);
            }
        }

        if (currentCover == null)
        {
            currentCover = FindCover(attacker, ai.na, coverCheckRadius, numberOfChecks, coverCriteria);
        }

        if (currentCover != null)
        {
            ai.na.SetDestination(currentCover.position);
        }
    }

    public NullableVector3 FindCover(Transform attacker, NavMeshAgent na, float coverCheckRadius, int numberOfChecks, LayerMask coverCriteria)
    {
        NullableVector3 newCover = null;
        NavMeshPath newCoverPath = null;

        for (int i = 0; i < numberOfChecks; i++)
        {
            // Obtains a random position within a certain vicinity of the agent
            Vector3 randomPosition = ai.transform.position + Random.insideUnitSphere * coverCheckRadius;
            NavMeshHit coverCheck;
            // Checks if there is an actual point on the navmesh close to the randomly selected position
            if (NavMesh.SamplePosition(randomPosition, out coverCheck, na.height * 2, NavMesh.AllAreas))
            {
                
                if (AI.LineOfSight(coverCheck.position, attacker, coverCriteria) == false) // If line of sight is not established
                {
                    // Ensures that the agent can actually move to the cover position.
                    NavMeshPath nmp = new NavMeshPath();
                    if (na.CalculatePath(coverCheck.position, nmp))
                    {
                        // Checks if the new cover position is easier to get to than the old one.
                        if (newCover == null || AI.NavMeshPathLength(nmp) < AI.NavMeshPathLength(newCoverPath)) // Use OR statement, and check navmesh path cost between transform.position and the cover point currently being checked.
                        {
                            // If so, new cover position is established, and navmesh path is stored for next comparison
                            newCover = NullableVector3.New(coverCheck.position);
                            newCoverPath = nmp;
                        }
                    }
                }
            }
        }

        return newCover;
    }
}