using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Misc
{
    public static Vector3 DirectionFromTransform(Transform reference, Vector3 axes)
    {
        return Quaternion.AngleAxis(axes.x, reference.right) * Quaternion.AngleAxis(axes.y, reference.up) * Quaternion.AngleAxis(axes.z, reference.forward) * reference.forward;
    }
}
