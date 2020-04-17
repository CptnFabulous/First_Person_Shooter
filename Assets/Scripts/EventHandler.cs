using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// The EventHandler script will exist on a gameobject located in the scene.
// All EventObserver scripts will reference this script
// EventObserver scripts will add their functions to the appropriate delegates when enabled, and remove them when disabled.
// When an appropriate game event happens, that function will use a Transmit function to search for this EventHandler and trigger the referenced functions in the appropriate delegate.

public class EventHandler : MonoBehaviour
{
    public List<EventObserver> eventObservers;
    // Add more delegates, functions, etc. if I need to add new game events in the future
}

public class AttackMessage
{
    public Character attacker;
    public Character victim;

    public static AttackMessage New(Character attacker, Character victim)
    {
        return new AttackMessage { attacker = attacker, victim = victim };
    }
}

public class DamageMessage
{
    public Character attacker;
    public Character victim;
    public DamageType method;
    public int amount;

    public static DamageMessage New(Character attacker, Character victim, DamageType method, int amount)
    {
        return new DamageMessage { attacker = attacker, victim = victim, method = method, amount = amount };
    }
}

public class KillMessage
{
    public Character attacker;
    public Character victim;
    public DamageType causeOfDeath;
    public static KillMessage New(Character attacker, Character victim, DamageType causeOfDeath)
    {
        return new KillMessage { attacker = attacker, victim = victim, causeOfDeath = causeOfDeath };
    }
}

public class InteractMessage
{
    public PlayerHandler player;
    public Interactable interactable;

    public static InteractMessage New(PlayerHandler player, Interactable interactable)
    {
        return new InteractMessage { player = player, interactable = interactable };
    }
}

public class SpawnMessage
{
    public Entity spawned;
    public Vector3 location;

    public static SpawnMessage New(Entity spawned, Vector3 location)
    {
        return new SpawnMessage { spawned = spawned, location = location };
    }
}