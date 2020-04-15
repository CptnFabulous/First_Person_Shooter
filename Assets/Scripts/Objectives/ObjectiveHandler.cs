using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class ObjectiveHandler : MonoBehaviour
{
    EventObserver eo;

    public PlayerObjective[] objectives;

    private void Awake()
    {
        eo = GetComponent<EventObserver>();
        //eo.OnKillMessage.AddListener((km) => CheckKillObjectives(km));
        // Add important functions to eventobserver
    }

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
            if (o.state == ObjectiveState.Inactive)
            {
                o.ActivateCheck();
            }
        }
    }

    public bool LevelComplete()
    {
        bool completed = true;
        foreach (PlayerObjective o in objectives)
        {
            if (o.mandatory == true && o.state != ObjectiveState.Completed)
            {
                completed = false;
            }
        }
        return completed;
    }

    public void CheckKillObjectives(KillMessage km)
    {
        print("Checking kill objectives");
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

    public void CheckInteractObjectives(InteractMessage im)
    {
        foreach (InteractObjective o in objectives)
        {
            if (im.interactable == o.objectiveInteractable)
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
