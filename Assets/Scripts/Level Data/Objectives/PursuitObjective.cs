using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PursuitObjective : PlayerObjective
{
    public Transform location;
    public float radius;

    float Distance()
    {
        return Vector3.Distance(location.position, FindObjectOfType<PlayerHandler>().transform.position);
    }

    public override void CompletedCheck()
    {
        if (Distance() <= radius)
        {
            Complete();
        }
    }

    public override string DisplayCriteria()
    {
        return name + ": " + Mathf.RoundToInt(Distance()) + "m";
    }
}