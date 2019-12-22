using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum ObjectiveState
{
    Inactive,
    Active,
    Completed,
}

[System.Serializable]
public abstract class PlayerObjective
{
    public string description;
    ObjectiveState state;
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
}

[System.Serializable]
public class EliminationObjective : PlayerObjective
{
    public Character[] enemies;
    public int quantity;
    int amountEliminated;

    public override void CompletedCheck()
    {
        if (amountEliminated >= quantity)
        {
            Complete();
        }
    }

    public override string DisplayCriteria()
    {
        return description + (": ") + amountEliminated + ("/") + quantity;
    }
}

[System.Serializable]
public class PursuitObjective : PlayerObjective
{
    public Transform location;
    public float radius;

    float Distance(Transform playerTransform)
    {
        return Vector3.Distance(location.position, playerTransform.position);
    }

    public override void CompletedCheck()
    {
        if (/*Distance()*/1 <= radius)
        {
            Complete();
        }
    }

    public override string DisplayCriteria()
    {
        return description + (": ") + /*Distance() + */("m");
    }
}

[System.Serializable]
public class CollectionObjective : PlayerObjective
{
    public ItemPickup itemType;
    public int quantity;
    int amountObtained;

    public override void CompletedCheck()
    {
        if (amountObtained >= quantity)
        {
            Complete();
        }
    }

    public override string DisplayCriteria()
    {
        return description + (": ") + amountObtained + ("/") + quantity;
    }
}

[System.Serializable]
public class InteractObjective : PlayerObjective
{
    public Interactable interactable;
}

public class ObjectiveHandler : MonoBehaviour
{
    public EliminationObjective[] targets;
    public PursuitObjective[] locations;
    public CollectionObjective[] items;
    public InteractObjective[] interactables;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Checks if objectives are completed
        foreach (EliminationObjective o in targets)
        {
            o.CompletedCheck();
        }
        foreach (PursuitObjective o in locations)
        {
            o.CompletedCheck();
        }
        foreach (CollectionObjective o in items)
        {
            o.CompletedCheck();
        }
        foreach (InteractObjective o in interactables)
        {
            o.CompletedCheck();
        }
    }
}
