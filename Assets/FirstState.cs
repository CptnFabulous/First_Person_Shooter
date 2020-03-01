using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstState : State<ArtificialIntelligence>
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

    public override void EnterState(ArtificialIntelligence owner)
    {
        Debug.Log("Entering FirstState");
    }

    public override void ExitState(ArtificialIntelligence owner)
    {
        Debug.Log("Exiting FirstState");
    }

    public override void Update(ArtificialIntelligence owner)
    {
        Debug.Log("FirstState is currently active");
        if (!owner.switchState) // Checks for variables inside owner script using owner.whatevervariable
        {
            owner.stateMachine.ChangeState(SecondState.Instance);
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
