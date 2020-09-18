using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer
{
    public float time;
    float timer;

    // Update is called once per frame
    public void Update()
    {
        timer += Time.deltaTime / time;
        if (timer > 1)
        {
            // Perform task a number of times equal to the whole number value of the float, and minus 1 each time
            while(timer > 1)
            {
                // Do action
                Action();
                timer -= 1;
            }
        }
    }

    public void Action()
    {

    }

    public void Reset()
    {
        timer = 0;
    }
}
