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
}