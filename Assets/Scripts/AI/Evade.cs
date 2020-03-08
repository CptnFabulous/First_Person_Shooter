using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Evade : AIMovementBehaviour
{
    /*
    Repurpose code for taking cover -
    Randomly sample spots around the target (somehow make sure they're all outside the specified range, and use Navmesh.SamplePosition to ensure the agent can reach those positions.
    Of these positions, find the ones that have line of sight between the agent and the target (the agent needs to stay far away from the target while also being able to attack them).
    Of these, find the position that is closest to the agent, to ensure the shortest travel time.
    */
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
