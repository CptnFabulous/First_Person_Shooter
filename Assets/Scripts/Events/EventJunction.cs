using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventJunction
{
    public static event System.Action<AttackMessage> OnAttack;
    public static event System.Action<DamageMessage> OnDamage;
    public static event System.Action<KillMessage> OnKill;
    public static event System.Action<InteractMessage> OnInteract;
    public static event System.Action<SpawnMessage> OnSpawn;


    public static void TransmitAttack(AttackMessage message)
    {
        OnAttack(message);
    }
    public static void TransmitDamage(DamageMessage message)
    {
        OnDamage(message);
    }
    public static void TransmitKill(KillMessage message)
    {
        OnKill(message);
    }
    public static void TransmitInteraction(InteractMessage message)
    {
        OnInteract(message);
    }
    public static void TransmitSpawning(SpawnMessage message)
    {
        OnSpawn(message);
    }

    public static void UpdateAttackReceiver(System.Action<AttackMessage> function, bool isAdding)
    {
        if (isAdding)
        {
            OnAttack += function;
        }
        else
        {
            OnAttack -= function;
        }
    }
}
