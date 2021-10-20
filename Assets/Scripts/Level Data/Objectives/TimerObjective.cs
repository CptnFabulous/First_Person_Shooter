using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerObjective : PlayerObjective
{
    public int hours;
    [Range(0, 59)]
    public int minutes;
    [Range(0, 59)]
    public float seconds;

    float timeElapsed;
    
    // Update is called once per frame
    void Update()
    {
        timeElapsed += Time.deltaTime;
    }

    float TotalTime()
    {
        return (hours * 3600) + (minutes * 60) + seconds;
    }

    public override void CheckCompletion()
    {
        if (timeElapsed >= TotalTime())
        {
            Complete();
        }
    }

    public override string DisplayCriteria()
    {
        float t = TotalTime() - timeElapsed;
        int h = 0;
        int m = 0;
        float s = 0;
        while (t >= 3600)
        {
            t -= 3600;
            h += 1;
        }
        while (t >= 60)
        {
            t -= 60;
            m += 1;
        }
        s = t;
        return name + ": " + h + ":" + m + ":" + Mathf.RoundToInt(s) + " remaining";
    }
}