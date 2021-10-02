using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Events;
public class ObjectiveHandler : MonoBehaviour
{
    static ObjectiveHandler current;
    public static ObjectiveHandler Current
    {
        get
        {
            if (current == null)
            {
                current = FindObjectOfType<ObjectiveHandler>();
            }
            return current;
        }
    }
    
    
    EventObserver eo;

    public PlayerObjective[] objectives;

    public LevelCompleteScreen screen;
    public string nextLevelName;

    bool levelCompleted;

    private void Awake()
    {
        // Add important functions to eventobserver
        eo = GetComponent<EventObserver>();
        eo.OnKill += CheckKillObjectives;


        //CompleteLevel();
    }

    // Start is called before the first frame update
    void Start()
    {
        objectives = GetComponentsInChildren<PlayerObjective>();



    }

    // Update is called once per frame
    void Update()
    {
        #region Checks objective statuses
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
        #endregion

        #region Checks level completion
        bool allObjectivesCompleted = true;
        foreach (PlayerObjective o in objectives)
        {
            if (o.mandatory == true && (o.state != ObjectiveState.Completed && o.state != ObjectiveState.Disabled))
            {
                allObjectivesCompleted = false;
            }
        }

        if (allObjectivesCompleted && !levelCompleted)
        {
            print("Level completed");
            CompleteLevel();
        }
        #endregion
    }

    

    public void CheckKillObjectives(KillMessage km)
    {
        if (km.attacker.GetComponent<PlayerHandler>() != null)
        {
            print("Checking kill objectives");
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

    
    public string KillFeedMessage(KillMessage km)
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

        if (km.attacker != null)
        {
            deathMessage += " by " + km.attacker.name;
        }
        deathMessage += "!";

        return deathMessage;

    }
    




    void CompleteLevel()
    {
        levelCompleted = true;

        // For some reason, finding the GameStateHandler from the PlayerHandler does not work, but finding the GameStateHandler directly does.
        GameStateHandler[] players = FindObjectsOfType<GameStateHandler>();
        foreach (GameStateHandler gsh in players)
        {
            gsh.WinGame();
        }

        /*
        PlayerHandler[] players = FindObjectsOfType<PlayerHandler>();
        foreach (PlayerHandler ph in players)
        {
            print("Player found");
            print(ph.gsh.CurrentState());
            ph.gsh.WinGame();
        }
        */
        screen.gameObject.SetActive(true);
        screen.GenerateScreen(this);
    }
}
