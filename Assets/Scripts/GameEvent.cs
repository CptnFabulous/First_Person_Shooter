using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameEvent
{
    public static void TransmitKill(Character attacker, Character victim, DamageType killMethod)
    {
        LevelManager gm = Object.FindObjectOfType<LevelManager>();
        gm.ReceiveKill(attacker, victim, killMethod);
    }

    public static void TransmitInteraction(PlayerHandler player, Interactable interactable)
    {
        LevelManager gm = Object.FindObjectOfType<LevelManager>();
        gm.ReceiveInteraction(player, interactable);
    }
}
