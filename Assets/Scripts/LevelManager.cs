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
    
    public void ReceiveKill(KillMessage km)
    {
        string deathMessage = km.victim.name + " was ";
        switch (km.killMethod)
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

    public void ReceiveInteraction(PlayerHandler player, Interactable interactable)
    {
        if (objectiveHandler != null)
        {
            objectiveHandler.CheckInteractObjectives(player, interactable);
        }
    }
}
