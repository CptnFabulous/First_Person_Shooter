﻿using System.Collections;
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

    #region Inverse clamp
    public static float InverseClamp(float value, float min, float max)
    {
        if (value > max)
        {
            return min;
        }
        else if (value < min)
        {
            return max;
        }
        else
        {
            return value;
        }
    }

    public static int InverseClamp(int value, int min, int max)
    {
        if (value > max)
        {
            return min;
        }
        else if (value < min)
        {
            return max;
        }
        else
        {
            return value;
        }
    }
    #endregion

    public static float InverseCurveEvaluate(AnimationCurve curve, float t)
    {
        float curveMin = curve.keys[0].value;
        float curveMax = curve.keys[0].value;
        for (int i = 0; i < curve.keys.Length; i++)
        {
            curveMin = Mathf.Min(curveMin, curve.keys[i].value);
            curveMax = Mathf.Max(curveMax, curve.keys[i].value);
        }
        float range = curveMax - curveMin;

        return (1 - (curve.Evaluate(t) - curveMin) / range) * range + curveMin;
    }
}
