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



    public static void Transmit(AttackMessage message)
    {
        if (OnAttack != null)
        {
            OnAttack.Invoke(message);
        }
        
    }
    public static void RefreshWithFunction(System.Action<AttackMessage> function, bool isAdding)
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

    public static void Transmit(DamageMessage message)
    {
        if (OnDamage != null)
        {
            OnDamage.Invoke(message);
        }
    }
    public static void RefreshWithFunction(System.Action<DamageMessage> function, bool isAdding)
    {
        if (isAdding)
        {
            OnDamage += function;
        }
        else
        {
            OnDamage -= function;
        }
    }

    public static void Transmit(KillMessage message)
    {
        OnKill.Invoke(message);
    }
    public static void RefreshWithFunction(System.Action<KillMessage> function, bool isAdding)
    {
        if (isAdding)
        {
            OnKill += function;
        }
        else
        {
            OnKill -= function;
        }
    }

    public static void Transmit(InteractMessage message)
    {
        OnInteract.Invoke(message);
    }
    public static void RefreshWithFunction(System.Action<InteractMessage> function, bool isAdding)
    {
        if (isAdding)
        {
            OnInteract += function;
        }
        else
        {
            OnInteract -= function;
        }
    }

    public static void Transmit(SpawnMessage message)
    {
        OnSpawn.Invoke(message);
    }
    public static void RefreshWithFunction(System.Action<SpawnMessage> function, bool isAdding)
    {
        if (isAdding)
        {
            OnSpawn += function;
        }
        else
        {
            OnSpawn -= function;
        }
    }







}
