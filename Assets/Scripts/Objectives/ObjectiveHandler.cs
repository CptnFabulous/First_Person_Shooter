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
        eo.OnKill += CheckKillObjectives;
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
    /*
    public void CheckInteractObjectives(InteractMessage im)
    {
        foreach (InteractObjective o in objectives)
        {
            if (im.interactable == o.objectiveInteractable)
            {

            }
        }
    }
    */

    /*
    public void ReceiveKill(KillMessage km)
    {
        string deathMessage = km.victim.name + " was ";
        switch (km.causeOfDeath)
        {
            case DamageType.Shot:
                deathMessage += "riddled with bullets";
                break;
            case DamageType.CriticalShot:
                deathMessage += "fatally shot";
                break;
            case DamageType.BlownUp:
                deathMessage += "blown up";
                break;
            case DamageType.Gibbed:
                deathMessage += "splattered to giblets";
                break;
            case DamageType.Burned:
                deathMessage += "burned to a crisp";
                break;
            case DamageType.Bludgeoned:
                deathMessage += "bludgeoned to a pulp";
                break;
            case DamageType.FallDamage:
                deathMessage += "reduced to a flat red stain";
                break;
            default:
                deathMessage += "killed";
                break;
        }
        deathMessage += " by " + km.attacker.name + "!";
        print(deathMessage);

        if (objectiveHandler != null)
        {
            objectiveHandler.CheckKillObjectives(km);
        }

    }
    */
}
