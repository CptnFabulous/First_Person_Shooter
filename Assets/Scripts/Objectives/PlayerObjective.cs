using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum ObjectiveState
{
    Inactive,
    Active,
    Completed,
    Disabled
}

[System.Serializable]
public class PlayerObjective : MonoBehaviour
{
    public string description;
    public ObjectiveState state = ObjectiveState.Active;
    public UnityEvent onCompletion;

    public virtual void CompletedCheck()
    {
        Complete();
    }

    public void Complete()
    {
        state = ObjectiveState.Completed;
        onCompletion.Invoke();
    }

    public virtual string DisplayCriteria()
    {
        return description;
    }

    public void Activate()
    {
        state = ObjectiveState.Active;
    }

    public void Deactivate()
    {
        state = ObjectiveState.Inactive;
    }
}