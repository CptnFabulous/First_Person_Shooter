using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct Resource
{
    // Include name field for editing convenience only - not needed in-game.
#if UNITY_EDITOR
    [HideInInspector]
    public string refName;
#endif
    public int max;
    public int current;
    public float criticalThreshold;
}

public static class Stat
{
    public static int Current(Resource r)
    {
        return r.current;
    }

    public static int Max(Resource r)
    {
        
        return r.max;
    }

    public static float Critical(Resource r)
    {
        return r.criticalThreshold;
    }

    public static void ChangeCurrent(Resource r, int newValue)
    {
        newValue = Mathf.Clamp(newValue, 0, r.max);
        Resource rs = r;
        rs.current = newValue;
        r = rs;
    }
    public static void ChangeMax(Resource r, int newValue)
    {
        Resource rs = r;
        rs.max = newValue;
        r = rs;
    }
    public static void ChangeCritical(Resource r, float newValue)
    {
        Resource rs = r;
        rs.criticalThreshold = newValue;
        r = rs;
    }
}
