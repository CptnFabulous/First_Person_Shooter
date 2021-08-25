using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[System.Serializable]
public class Timer
{
    public float delay; // How long the timer lasts, in seconds
    public bool unscaled; // Does the timer ignore the time scale?
    float startTime; // The recorded time of reset. Subtracted from current time for time passed

    public float Progress
    {
        get
        {
            // Gets the amount of real time that has passed since the recorded time on reset
            float progress = Time.time - startTime;
            if (unscaled == false)
            {
                // If timer is scaled, multiply progress by the current time scale
                progress *= Time.timeScale;
            }

            // Divides time by the delay to get a value between 0 or 1 (or more than one if over time)
            return progress / delay;
        }
    }

    public bool IsReady
    {
        get
        {
            // Checks if enough time has elapsed
            return Progress >= 1;
        }
    }

    public void Reset()
    {
        startTime = Time.time;
    }
}
