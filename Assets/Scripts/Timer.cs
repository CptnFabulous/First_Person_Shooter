using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer
{
    public float time;
    float timer;

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime / time;
        if (timer > 1)
        {
            // Perform task a number of times equal to the whole number value of the float, and minus 1 each time
        }
    }

    void Reset()
    {
        timer = 0;
    }
}
