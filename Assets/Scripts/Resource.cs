using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Resource
{
    #if UNITY_EDITOR
    [HideInInspector, Tooltip("A name used to easily reference the resource")]
    public string refName;
    #endif
    [Tooltip("The maximum allowed capacity.")]
    public int max;
    [Tooltip("The amount currently available.")]
    public int current;
    [Tooltip("The level where the current value is considered critically low.")]
    public int critical;

    public Resource(string name)
    {
        refName = name;
    }

    public Resource(int _max, int _current, int _critical)
    {
        max = _max;
        current = _current;
        critical = _critical;

    }

    public Resource(string name, int _max, int _current, int _critical)
    {
        refName = name;
        max = _max;
        current = _current;
        critical = _critical;

    }

    public float PercentageFull
    {
        get
        {
            return current / max;
        }
    }

    public bool IsCritical
    {
        get
        {
            if (current <= critical)
            {
                return true;
            }
            return false;
        }
    }

}