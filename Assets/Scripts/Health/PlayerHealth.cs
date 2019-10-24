using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (PlayerHandler))]
public class PlayerHealth : Health
{
    
    [HideInInspector] public PlayerHandler ph;

    public override void Awake()
    {
        base.Awake();
        ph = GetComponent<PlayerHandler>();
    }
    

    public override void Die(DamageType causeOfDeath, GameObject lastAttacker)
    {
        string deathMessage = name + " was "; 
        switch(causeOfDeath)
        {
            case DamageType.Shot:
                deathMessage += "shot to death";
                break;
            case DamageType.CriticalShot:
                deathMessage += "shot in the head";
                break;
            case DamageType.BlownUp:
                deathMessage += "blown up";
                break;
            case DamageType.Gibbed:
                deathMessage += "splattered to pieces";
                break;
            case DamageType.Burned:
                deathMessage += "burned to a crisp";
                break;
            case DamageType.KnockedOut:
                deathMessage += "knocked unconscious";
                break;
            default:
                deathMessage += "killed";
                break;
        }
        deathMessage += " by " + lastAttacker.name + "!";

        print(deathMessage);

        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        ph.pc.enabled = false;
        ph.wh.equippedWeapon.enabled = false;
        ph.wh.enabled = false;
        ph.hud.enabled = false;
    }
    
}
