using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (PlayerHandler))]
public class PlayerHealth : Health
{
    
    [HideInInspector] public PlayerHandler ph;


    

    public void Awake()
    {
        ph = GetComponent<PlayerHandler>();
    }

    public override void TakeDamage(int damageAmount, GameObject origin, DamageType damageSource)
    {
        base.TakeDamage(damageAmount, origin, damageSource);
        ph.hud.PlayerDamageFeedback();
    }

    public override void Die(DamageType causeOfDeath, GameObject lastAttacker)
    {
        ph.ChangePlayerState(PlayerState.Dead);

        
    }


    string DeathMessage(GameObject sourceOfDeath, DamageType causeOfDeath)
    {
        string deathMessage = "You were ";

        switch (causeOfDeath)
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

        deathMessage += " by ";

        Character c = sourceOfDeath.GetComponent<Character>();
        if (c == null || c.isUnique == false)
        {
            bool startsWithVowel = false;

            string[] vowels = new string[]
            {
                "A",
                "E",
                "I",
                "O",
                "U",
                "a",
                "e",
                "i",
                "o",
                "u"
            };

            for (int i = 0; i < vowels.Length; i++)
            {
                if (sourceOfDeath.name.StartsWith(vowels[i]))
                {
                    startsWithVowel = true;
                }
            }

            if (startsWithVowel)
            {
                deathMessage += "an";
            }
            else
            {
                deathMessage += "a";
            }
        }

        deathMessage += " " + sourceOfDeath.name + "!";

        return deathMessage;
    }
}
