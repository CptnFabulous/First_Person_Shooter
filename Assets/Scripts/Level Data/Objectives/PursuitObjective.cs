using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PursuitObjective : PlayerObjective
{
    public Transform location;
    public float completionThreshold = 1;

    float PlayerDistanceToTarget
    {
        get
        {
            return Vector3.Distance(location.position, ObjectiveHandler.Current.CurrentPlayer.transform.position);
        }
    }

    public override void CheckCompletion()
    {
        //Debug.Log("Checking completion status");
        if (PlayerDistanceToTarget <= completionThreshold)
        {
            //Debug.Log("Objective completed");
            Complete();
        }
    }

    public override string DisplayCriteria()
    {
        return name + ": " + Mathf.RoundToInt(PlayerDistanceToTarget) + "m";
    }
}