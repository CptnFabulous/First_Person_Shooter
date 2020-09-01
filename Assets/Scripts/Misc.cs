using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Misc
{


    #region Calculating perpendicular Vector3 directions
    public static Vector3 PerpendicularRight(Vector3 forward, Vector3 worldUp)
    {
        return Vector3.Cross(forward, worldUp).normalized;
    }

    public static Vector3 PerpendicularUp(Vector3 forward, Vector3 worldUp)
    {
        Vector3 worldRight = PerpendicularRight(forward, worldUp);
        return Vector3.Cross(forward, worldRight).normalized;
    }

    public static Vector3 AngledDirection(Vector3 axes, Vector3 forward, Vector3 upward)
    {
        Vector3 right = PerpendicularRight(forward, upward);
        Vector3 up = PerpendicularUp(forward, upward);
        return Quaternion.AngleAxis(axes.x, right) * Quaternion.AngleAxis(axes.y, up) * Quaternion.AngleAxis(axes.z, forward) * forward;
    }
    #endregion
}

public class DirectionValues
{
    public Vector3 position;
    public Vector3 forward;
    public Vector3 worldUp;

    public static DirectionValues FromTransform(Transform t)
    {
        return new DirectionValues {position = t.position, forward = t.forward, worldUp = t.up };
    }

}
