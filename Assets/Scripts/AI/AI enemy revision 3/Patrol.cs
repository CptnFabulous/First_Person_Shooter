using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol : AIMovementBehaviour
{
    public Transform[] waypoints;
    public bool endToEnd;
    public bool reverse;

    public float waypointReachedDistance = 0.1f;


    int index;
    //int indexChangeValue = 1;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        Debug.Log("Setting destination for " + ai.name + " from Patrol behaviour");

        ai.na.SetDestination(waypoints[index].position); // Agent moves towards waypoint
        if (Vector3.Distance(ai.transform.position, ai.na.destination) <= waypointReachedDistance) // If the agent has reached the waypoint
        {
            if (reverse == true) // Is the agent going forwards or backwards along its path?
            {
                ForwardNewPosition(); // If forwards, move to next waypoint
            }
            else
            {
                ReverseNewPosition(); // If backwards, move to previous waypoint
            }
        }
    }
    

    public void ForwardNewPosition()
    {
        index += 1; // Adds to index, so the agent moves to the next waypoint on the list
        if (index >= waypoints.Length)
        {
            if (endToEnd == true)
            {
                reverse = !reverse;
                index = waypoints.Length - 1;
            }
            else
            {
                index = 0;
            }
            
        }
    }

    public void ReverseNewPosition()
    {
        index -= 1; // Subtracts from index, so the agent moves to the previous waypoint on the list
        if (index < 0)
        {
            if (endToEnd == true)
            {
                reverse = !reverse;
                index = 0;
            }
            else
            {
                index = waypoints.Length - 1;
            }
            
        }
    }

   
}
