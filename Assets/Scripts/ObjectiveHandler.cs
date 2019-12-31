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
public abstract class PlayerObjective
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

[System.Serializable]
public class KillQuantityObjective : PlayerObjective
{
    public Character enemyType;
    public int quantity;
    [HideInInspector] public int amountEliminated;

    public override void CompletedCheck()
    {
        if (amountEliminated >= quantity)
        {
            Complete();
        }
    }

    public override string DisplayCriteria()
    {
        return description + ": " + amountEliminated + "/" + quantity;
    }
}

[System.Serializable]
public class KillGroupObjective : PlayerObjective
{
    public List<Character> enemies;

    public override void CompletedCheck()
    {
        if (enemies.Count <= 0) // Check if no enemies are alive
        {
            Complete();
        }
    }

    public override string DisplayCriteria()
    {
        return description + ": " + enemies.Count + " remaining";
    }
}

[System.Serializable]
public class PursuitObjective : PlayerObjective
{
    public Transform location;
    public float radius;

    float Distance()
    {
        return Vector3.Distance(location.position, Object.FindObjectOfType<PlayerHandler>().transform.position);
    }

    public override void CompletedCheck()
    {
        if (Distance() <= radius)
        {
            Complete();
        }
    }

    public override string DisplayCriteria()
    {
        return description + ": " + Mathf.RoundToInt(Distance()) + "m";
    }
}

[System.Serializable]
public class CollectionObjective : PlayerObjective
{
    public ItemPickup itemType;
    public int quantity;
    [HideInInspector] public int amountObtained;

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
    public KillQuantityObjective[] enemyQuotas;
    public KillGroupObjective[] targets;
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
        foreach (KillQuantityObjective o in enemyQuotas)
        {
            if (o.state == ObjectiveState.Active)
            {
                o.CompletedCheck();
            }
            
        }
        foreach (KillGroupObjective o in targets)
        {
            if (o.state == ObjectiveState.Active)
            {
                o.CompletedCheck();
            }
        }
        foreach (PursuitObjective o in locations)
        {
            if (o.state == ObjectiveState.Active)
            {
                o.CompletedCheck();
            }
        }
        foreach (CollectionObjective o in items)
        {
            if (o.state == ObjectiveState.Active)
            {
                o.CompletedCheck();
            }
        }
        foreach (InteractObjective o in interactables)
        {
            if (o.state == ObjectiveState.Active)
            {
                o.CompletedCheck();
            }
        }
    }

    public void CheckKillObjectives(Character attacker, Character victim, DamageType killMethod)
    {
        if (attacker.GetComponent<PlayerHandler>() != null)
        {
            print("Attacker is a player");
            foreach (KillQuantityObjective o in enemyQuotas)
            {
                print("o");
                if (o.state == ObjectiveState.Active && (victim == o.enemyType || victim.properName == o.enemyType.properName))
                {
                    o.amountEliminated += 1;
                    print("Killed");
                }
            }

            foreach (KillGroupObjective o in targets)
            {
                foreach(Character c in o.enemies)
                {
                    if (c == victim)
                    {
                        o.enemies.Remove(c);
                    }
                }
            }
        }
        
    }
}
