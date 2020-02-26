using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondState : State<AI>
{
    private static SecondState instance;

    private SecondState()
    {
        if (instance != null) // Cancels function if an instance already exists
        {
            return;
        }

        instance = this; // Creates new instance of state
    }

    public static SecondState Instance
    {
        get
        {
            if (instance == null)
            {
                new SecondState();
            }

            return instance;
        }
    }

    public override void EnterState(AI owner)
    {
        Debug.Log("Entering FirstState");
    }

    public override void ExitState(AI owner)
    {
        Debug.Log("FirstState is currently active");
    }

    public override void Update(AI owner)
    {
        Debug.Log("Exiting FirstState");
    }
}
