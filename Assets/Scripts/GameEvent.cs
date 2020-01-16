using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameEvent
{
    public static void TransmitKill(Character attacker, Character victim, DamageType killMethod)
    {
        LevelManager lm = Object.FindObjectOfType<LevelManager>();
        if (lm != null)
        {
            lm.ReceiveKill(KillMessage.New(attacker, victim, killMethod));
        }
    }

    public static void TransmitKill(KillMessage message)
    {
        LevelManager lm = Object.FindObjectOfType<LevelManager>();
        if (lm != null)
        {
            lm.ReceiveKill(message);
        }
    }

    public static void TransmitInteraction(PlayerHandler player, Interactable interactable)
    {
        LevelManager lm = Object.FindObjectOfType<LevelManager>();
        if (lm != null)
        {
            lm.ReceiveInteraction(player, interactable);
        }
    }
}

public class KillMessage
{
    public Character attacker;
    public Character victim;
    public DamageType killMethod;

    public static KillMessage New(Character attacker, Character victim, DamageType killMethod)
    {
        return new KillMessage { attacker = attacker, victim = victim, killMethod = killMethod };
    }
}
