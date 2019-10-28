using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Timer
{
    public float time;
    float timer;

    public void Count()
    {
        timer += Time.deltaTime / time;
    }

    public void Reset()
    {
        timer = 0;
    }

    public bool TimeIsUp()
    {
        if (timer >= 1)
        {
            return true;
        }
        return false;
    }
}
