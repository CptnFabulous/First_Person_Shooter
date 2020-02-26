using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// https://youtu.be/PaLD1t-kIwM?t=1095
// Reference video, also go back to start of video to get info on why he uses namespaces, it seems to add unneeded complexity but there might be a good reason for it

// https://www.youtube.com/watch?v=YdERlPfwUb0
// https://www.youtube.com/watch?v=dYi-i83sq5g
// Other state machine videos

[System.Serializable]
public class StateMachine<T>
{
    public State<T> currentState { get; private set; }
    public T Owner;

    public StateMachine(T o) // o for owner
    {
        Owner = o;
        currentState = null;
    }

    public void ChangeState(State<T> newState)
    {
        if (currentState != null) // Exits state, if a state is currently assigned
        {
            currentState.ExitState(Owner);
        }
        currentState = newState;
        currentState.EnterState(Owner);
    }

    public void Update()
    {
        if (currentState != null)
        {
            currentState.Update(Owner);
        }
    }
}

public abstract class State<T>
{
    public abstract void EnterState(T owner);

    public abstract void Update(T owner);

    public abstract void ExitState(T owner);
}
