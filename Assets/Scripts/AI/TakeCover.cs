using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TakeCover : AIMovementBehaviour
{
    public float coverCheckRadius = 10;
    public int numberOfChecks = 15;
    public LayerMask coverCriteria;

    Transform attacker;
    Transform currentCover;


    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        currentCover = FindCover(attacker, ai.na, coverCheckRadius, numberOfChecks, coverCriteria);

        ai.na.isStopped = false;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Physics.Raycast(currentCover.position, attacker.transform.position - currentCover.position, Vector3.Distance(currentCover.position, attacker.transform.position) + 0.01f, coverCriteria))
        {
            // Run FindCover again
        }
    }

    public Transform FindCover(Transform attacker, NavMeshAgent na, float coverCheckRadius, int numberOfChecks, LayerMask coverCriteria)
    {
        Transform currentCover = null;

        for (int i = 0; i < numberOfChecks; i++)
        {
            Vector3 randomPosition = Random.insideUnitSphere * coverCheckRadius;
            NavMeshHit coverCheck;
            if (NavMesh.SamplePosition(randomPosition, out coverCheck, na.height * 2, NavMesh.AllAreas))
            {
                if (Physics.Raycast(coverCheck.position, attacker.transform.position - coverCheck.position, Vector3.Distance(coverCheck.position, attacker.transform.position) + 0.01f, coverCriteria))
                {
                    

                    if (currentCover == null) // Use OR statement, and check navmesh path cost between transform.position and the cover point currently being checked.
                    {
                        currentCover.position = coverCheck.position;
                    }
                }
            }
        }

        return currentCover;
    }
}
