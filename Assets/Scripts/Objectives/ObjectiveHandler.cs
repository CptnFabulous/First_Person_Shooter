/*
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

    public void UpdateObjective(KillMessage km)
    {
        if (state == ObjectiveState.Active)
        {
            foreach (Character c in enemies)
            {
                if (c == km.victim)
                {
                    enemies.Remove(c);
                }
            }
        }
        
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
    public Interactable objectiveInteractable;
    [HideInInspector] public bool completed;

    public void UpdateObjective(PlayerHandler player, Interactable interactable)
    {
        if (interactable == objectiveInteractable)
        {
            CompletedCheck();
        }
    }
}



public class ObjectiveHandler : MonoBehaviour
{
    //public KillQuantityObjective[] enemyQuotas;
    //public KillGroupObjective[] targets;
    //public PursuitObjective[] locations;
    //public CollectionObjective[] items;
    //public InteractObjective[] interactables;
    public PlayerObjective[] objectives;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Checks if objectives are completed
        //CheckObjectiveList(enemyQuotas);
        //CheckObjectiveList(targets);
        //CheckObjectiveList(locations);
        //CheckObjectiveList(items);
        //CheckObjectiveList(interactables);
        CheckObjectiveList(objectives);
    }

    void CheckObjectiveList(PlayerObjective[] list)
    {
        foreach (PlayerObjective o in list)
        {
            if (o.state == ObjectiveState.Active)
            {
                o.CompletedCheck();
            }
        }
    }

    public void CheckKillObjectives(KillMessage km)
    {
        if (km.attacker.GetComponent<PlayerHandler>() != null)
        {
            print("Attacker is a player");
            foreach (KillQuantityObjective o in enemyQuotas)
            {
                print("o");
                if (o.state == ObjectiveState.Active && (km.victim == o.enemyType || km.victim.properName == o.enemyType.properName))
                {
                    o.amountEliminated += 1;
                    print("Killed");
                }
            }

            foreach (KillGroupObjective o in targets)
            {
                foreach (Character c in o.enemies)
                {
                    if (c == km.victim)
                    {
                        o.enemies.Remove(c);
                    }
                }
            }
        }
    }

    public void CheckInteractObjectives(PlayerHandler player, Interactable interactable)
    {
        foreach (InteractObjective o in interactables)
        {
            if (interactable == o.objectiveInteractable)
            {

            }
        }
    }

    void CheckObjective(PlayerObjective o)
    {
        if (o is KillQuantityObjective)
        {
            print("Objective is a KillQuantityObjective");
            return;
        }
    }
}
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class ObjectiveHandler : MonoBehaviour
{
    public PlayerObjective[] objectives;

    // Start is called before the first frame update
    void Start()
    {
        objectives = GetComponentsInChildren<PlayerObjective>();
    }

    // Update is called once per frame
    void Update()
    {
        // Checks if objectives are completed

        foreach (PlayerObjective o in objectives)
        {
            if (o.state == ObjectiveState.Active)
            {
                o.CompletedCheck();
            }
        }
    }

    public void CheckKillObjectives(KillMessage km)
    {
        if (km.attacker.GetComponent<PlayerHandler>() != null)
        {
            
            foreach (PlayerObjective o in objectives)
            {
                KillQuantityObjective kqo = o as KillQuantityObjective;
                if (kqo)
                {
                    kqo.UpdateObjective(km);
                }

                KillGroupObjective kgo = o as KillGroupObjective;
                if (kgo)
                {
                    kgo.UpdateObjective(km);
                }
            }
        }
    }

    public void CheckInteractObjectives(PlayerHandler player, Interactable interactable)
    {
        foreach (InteractObjective o in objectives)
        {
            if (interactable == o.objectiveInteractable)
            {

            }
        }
    }

    void CheckObjective(PlayerObjective o)
    {
        if (o is KillQuantityObjective)
        {
            print("Objective is a KillQuantityObjective");
            return;
        }
    }
}
