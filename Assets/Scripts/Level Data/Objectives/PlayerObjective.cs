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
    public PlayerObjective[] prerequisites;
    public UnityEvent onCompletion;

    /// <summary>
    /// Checks in Update() if this objective is complete. Only needs to be overridden for functions that aren't updated via events
    /// </summary>
    public virtual void CheckCompletion()
    {
        //Complete();
        return;
    }

    public void ActivateCheck()
    {
        bool allComplete = true;
        foreach (PlayerObjective p in prerequisites)
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