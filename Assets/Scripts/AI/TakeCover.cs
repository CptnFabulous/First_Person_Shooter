using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



public class TakeCover : AIMovementBehaviour
{
    public float coverCheckRadius = 10; // Make sure this number is small, otherwise it can cause lag when calculating large paths
    public int numberOfChecks = 15;
    public LayerMask coverCriteria;

    Transform attacker;

    Vector3 currentCover;


    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        attacker = ai.target.transform; // Placeholder, replace with a better way to detect an attacker

        currentCover = FindCover(attacker, ai.na, coverCheckRadius, numberOfChecks, coverCriteria);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Debug.Log("Update");
        Debug.Log(currentCover);

        if (currentCover != Vector3.zero)
        {
            // Launches a raycast between the cover position and the attacker
            RaycastHit lineOfSightCheck;
            if (Physics.Raycast(currentCover, attacker.transform.position - currentCover, out lineOfSightCheck, Vector3.Distance(currentCover, attacker.transform.position) + 0.01f, coverCriteria))
            {
                // Checks if line of sight is established between the attacker and the cover position. If so, the cover position has been compromised.
                if (lineOfSightCheck.collider.transform == attacker)
                {
                    // Reset and find a new cover point
                    currentCover = Vector3.zero;
                    currentCover = FindCover(attacker, ai.na, coverCheckRadius, numberOfChecks, coverCriteria);
                }
            }
        }

        if (currentCover != null)
        {
            ai.na.SetDestination(currentCover);
        }
    }

    public Vector3 FindCover(Transform attacker, NavMeshAgent na, float coverCheckRadius, int numberOfChecks, LayerMask coverCriteria)
    {
        Vector3 newCover = Vector3.zero;
        NavMeshPath newCoverPath = null;

        for (int i = 0; i < numberOfChecks; i++)
        {
            // Obtains a random position within a certain vicinity of the agent
            Vector3 randomPosition = ai.transform.position + Random.insideUnitSphere * coverCheckRadius;
            NavMeshHit coverCheck;
            // Checks if there is an actual point on the navmesh close to the randomly selected position
            if (NavMesh.SamplePosition(randomPosition, out coverCheck, na.height * 2, NavMesh.AllAreas))
            {
                // Launches a raycast between the cover position and the attacker
                RaycastHit lineOfSightCheck;
                if (Physics.Raycast(coverCheck.position, attacker.transform.position - coverCheck.position, out lineOfSightCheck, Vector3.Distance(coverCheck.position, attacker.transform.position) + 0.01f, coverCriteria))
                {
                    // Checks if line of sight is established between the attacker and the cover position. If not, the agent can take cover there.
                    if (lineOfSightCheck.collider.transform != attacker)
                    {
                        // Ensures that the agent can actually move to the cover position.
                        NavMeshPath nmp = new NavMeshPath();
                        if (na.CalculatePath(coverCheck.position, nmp))
                        {
                            // Checks if the new cover position is easier to get to than the old one.
                            if (newCover == Vector3.zero || AI.NavMeshPathLength(nmp) < AI.NavMeshPathLength(newCoverPath)) // Use OR statement, and check navmesh path cost between transform.position and the cover point currently being checked.
                            {
                                // If so, new cover position is established, and navmesh path is stored for next comparison
                                newCover = coverCheck.position;
                                newCoverPath = nmp;
                            }
                        }
                    }
                }
            }
        }

        return newCover;
    }
}