using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondState : State<ArtificialIntelligence>
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

    public override void EnterState(ArtificialIntelligence owner)
    {
        Debug.Log("Entering SecondState");
    }

    public override void ExitState(ArtificialIntelligence owner)
    {
        Debug.Log("Exiting SecondState");
    }

    public override void Update(ArtificialIntelligence owner)
    {
        Debug.Log("SecondState is currently active");
        if (owner.switchState) // Checks for variables inside owner script using owner.whatevervariable
        {
            owner.stateMachine.ChangeState(FirstState.Instance);
        }
    }

    public override void FixedUpdate(ArtificialIntelligence owner)
    {
        throw new System.NotImplementedException();
    }

    public override void LateUpdate(ArtificialIntelligence owner)
    {
        throw new System.NotImplementedException();
    }
}
