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
public abstract class PlayerObjective : MonoBehaviour
{
    public bool mandatory;
    public ObjectiveState state = ObjectiveState.Active;
    public PlayerObjective[] activateCriteria;
    public UnityEvent onCompletion;

    public virtual void CompletedCheck()
    {
        Complete();
    }

    public void ActivateCheck()
    {
        bool allComplete = true;
        foreach (PlayerObjective p in activateCriteria)
        {
            if (p.state != ObjectiveState.Completed)
            {
                allComplete = false;
            }
        }
        if (allComplete)
        {
            Activate();
        }
    }

    public void Complete()
    {
        state = ObjectiveState.Completed;
        onCompletion.Invoke();
    }

    public virtual string DisplayCriteria()
    {
        return name;
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