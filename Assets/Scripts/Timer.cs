using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Timer
{
    public float delay;
    float timer;

    // Update is called once per frame
    public void Update()
    {
        timer += Time.deltaTime / delay;
        if (timer > 1)
        {
            // Perform task a number of times equal to the whole number value of the float, and minus 1 each time
            while(timer > 1)
            {
                // Do action
                //Action();
                timer -= 1;
            }
        }
    }

    public void Sandwich(float f)
    {
        Debug.Log("Sandwich");
    }

    public bool IsReady()
    {
        if (timer > 1)
        {
            return true;
        }

        timer += Time.deltaTime / delay;

        //Action actionToPerform = Sandwich;
        //PerformAction(actionToPerform(5));


        //Action a = Sandwich;
        //PerformAction<Sandwich>(Sandwich());


        //a.Invoke();

        return false;

        //PerformAction<Upda>

        

    }

    public void PerformAction(Action a)
    {
        while (timer > 1)
        {
            a.Invoke();
            timer -= 1;
        }
    }

    public void PerformAction<T>(T myFunction)
    {
        while (timer > 1)
        {

            //Action a = myFunction;
            //a.Invoke();
            timer -= 1;
        }
    }

    /*
    public void Action(Function F)
    {
        while (timer > 1)
        {
            // Do action
            F();
            timer -= 1;
        }
    }
    */

    public void Reset()
    {
        timer = 0;
    }
}
