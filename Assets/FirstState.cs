using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstState : State<AI>
{
    private static FirstState instance;

    private FirstState()
    {
        if (instance != null) // Cancels function if an instance already exists
        {
            return;
        }

        instance = this; // Creates new instance of state
    }

    public static FirstState Instance
    {
        get
        {
            if (instance == null)
            {
                new FirstState();
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
        Debug.Log("Exiting FirstState");
    }

    public override void Update(AI owner)
    {
        Debug.Log("FirstState is currently active");
        if (!owner.switchState)
        {
            owner.stateMachine.ChangeState(SecondState.Instance);
        }
    }
    public override void FixedUpdate(AI owner)
    {
        throw new System.NotImplementedException();
    }

    public override void LateUpdate(AI owner)
    {
        throw new System.NotImplementedException();
    }
}
