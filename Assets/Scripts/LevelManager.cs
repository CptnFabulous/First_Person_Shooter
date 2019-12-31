using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public ObjectiveHandler objectiveHandler;
    
    /*
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    */
    
    public void ReceiveKill(Character attacker, Character victim, DamageType killMethod)
    {
        string deathMessage = victim.name + " was ";
        switch (killMethod)
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
            default:
                deathMessage += "killed";
                break;
        }
        deathMessage += " by " + attacker.name + "!";
        print(deathMessage);

        if (objectiveHandler != null)
        {
            objectiveHandler.CheckKillObjectives(attacker, victim, killMethod);
        }
    }

    public void ReceiveInteraction(PlayerHandler player, Interactable interactable)
    {
        
    }
    

    public static void TransmitKill(Character attacker, Character victim, DamageType killMethod)
    {
        LevelManager lm = FindObjectOfType<LevelManager>();
        if (lm != null)
        {
            lm.ReceiveKill(attacker, victim, killMethod);
        }
    }

    public static void TransmitInteraction(PlayerHandler player, Interactable interactable)
    {
        LevelManager lm = FindObjectOfType<LevelManager>();
        if (lm != null)
        {
            lm.ReceiveInteraction(player, interactable);
        }
    }
}
