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
    
    public static void TransmitAttack(Character attacker, Character victim)
    {
        EventHandler eh = FindObjectOfType<EventHandler>(); // Search for one
        if (eh != null) // If one is found
        {
            AttackMessage m = new AttackMessage { attacker = attacker, victim = victim };
            foreach (EventObserver eo in eh.eventObservers)
            {
                eo.OnAttackMessage.Invoke(m);
            }
        }
    }
    public static void TransmitDamage(Character attacker, Character victim, DamageType method, int amount)
    {
        EventHandler eh = FindObjectOfType<EventHandler>(); // Search for one
        if (eh != null) // If one is found
        {
            DamageMessage m = new DamageMessage { attacker = attacker, victim = victim, method = method, amount = amount };
            foreach (EventObserver eo in eh.eventObservers)
            {
                eo.OnDamageMessage.Invoke(m);
            }
        }
    }
    public static void TransmitKill(Character attacker, Character victim, DamageType causeOfDeath)
    {
        EventHandler eh = FindObjectOfType<EventHandler>(); // Search for one
        if (eh != null) // If one is found
        {
            KillMessage m = new KillMessage { attacker = attacker, victim = victim, causeOfDeath = causeOfDeath };
            foreach (EventObserver eo in eh.eventObservers)
            {
                //print("Kill functions executed");
                eo.OnKillMessage.Invoke(m);
            }
        }
    }
    public static void TransmitInteract(PlayerHandler player, Interactable interactable)
    {
        EventHandler eh = FindObjectOfType<EventHandler>(); // Search for one
        if (eh != null) // If one is found
        {
            InteractMessage m = new InteractMessage { player = player, interactable = interactable };
            foreach (EventObserver eo in eh.eventObservers)
            {
                eo.OnInteractMessage.Invoke(m);
            }
        }
    }
    public static void TransmitSpawn(Entity spawned, Vector3 location)
    {
        EventHandler eh = FindObjectOfType<EventHandler>(); // Search for one
        if (eh != null) // If one is found
        {
            SpawnMessage m = new SpawnMessage { spawned = spawned, location = location };
            foreach (EventObserver eo in eh.eventObservers)
            {
                eo.OnSpawnMessage.Invoke(m);
            }
        }
    }

    /*
    // This function is run whenever a player or NPC initiates an attack
    public static void TransmitAttack(Character attacker, Character victim)
    {
        EventHandler eh = FindObjectOfType<EventHandler>(); // Search for one
        if (eh != null) // If one is found
        {
            eh.OnAttack(attacker, victim); // Call functions in EventHandler
        }
    }

    // This function is run whenever a player or NPC is damaged
    public static void TransmitDamage(Character attacker, Character victim, DamageType method, int amount)
    {
        EventHandler eh = FindObjectOfType<EventHandler>(); // Search for one
        if (eh != null) // If one is found
        {
            eh.OnDamage(attacker, victim, method, amount); // Call functions in EventHandler
        }
    }

    // This function is run whenever a player or NPC is killed
    public static void TransmitKill(Character killer, Character killed, DamageType causeOfDeath)
    {
        EventHandler eh = FindObjectOfType<EventHandler>(); // Search for one
        if (eh != null) // If one is found
        {
            eh.OnKill(killer, killed, causeOfDeath); // Call functions in EventHandler
        }
    }

    // This function is run whenever a player interacts with something in the game world
    public static void TransmitInteract(PlayerHandler p, Interactable i)
    {
        EventHandler eh = FindObjectOfType<EventHandler>(); // Search for one
        if (eh != null) // If one is found
        {
            eh.OnInteract(p, i); // Call functions in EventHandler
        }
    }

    // This function is run whenever an entity spawns into the scene
    public static void TransmitSpawn(Entity e, Vector3 location)
    {
        EventHandler eh = FindObjectOfType<EventHandler>(); // Search for one
        if (eh != null) // If one is found
        {
            eh.OnSpawn(e, location); // Call functions in EventHandler
        }
    }
    */
}

public class AttackMessage
{
    public Character attacker;
    public Character victim;

    /*
    public static void Transmit(Character attacker, Character victim)
    {
        AttackMessage m = new AttackMessage { attacker = attacker, victim = victim };
        foreach (EventObserver eo in Object.FindObjectsOfType<EventObserver>()) // Find every EventObserver and run its 'OnAttack' function
        {
            eo.OnAttack(m);
        }
    }
    */
}

public class DamageMessage
{
    public Character attacker;
    public Character victim;
    public DamageType method;
    public int amount;

    /*
    public static void Transmit(Character attacker, Character victim, DamageType method, int amount)
    {
        DamageMessage m = new DamageMessage { attacker = attacker, victim = victim, method = method, amount = amount };
        foreach (EventObserver eo in Object.FindObjectsOfType<EventObserver>())
        {
            eo.OnDamage(m);
        }
    }
    */
}

public class KillMessage
{
    public Character attacker;
    public Character victim;
    public DamageType causeOfDeath;

    /*
    public static void Transmit(Character attacker, Character victim, DamageType causeOfDeath)
    {
        KillMessage m = new KillMessage { attacker = attacker, victim = victim, causeOfDeath = causeOfDeath };
        foreach (EventObserver eo in Object.FindObjectsOfType<EventObserver>())
        {
            eo.OnKill(m);
        }
    }
    */
    public static KillMessage New(Character attacker, Character victim, DamageType causeOfDeath)
    {
        return new KillMessage { attacker = attacker, victim = victim, causeOfDeath = causeOfDeath };
    }
}

public class InteractMessage
{
    public PlayerHandler player;
    public Interactable interactable;

    /*
    public static void Transmit(PlayerHandler player, Interactable interactable)
    {
        InteractMessage m = new InteractMessage { player = player, interactable = interactable };
        foreach (EventObserver eo in Object.FindObjectsOfType<EventObserver>())
        {
            eo.OnInteract(m);
        }
    }
    */
}

public class SpawnMessage
{
    public Entity spawned;
    public Vector3 location;

    /*
    public static void Transmit(Entity spawned, Vector3 location)
    {
        SpawnMessage m = new SpawnMessage { spawned = spawned, location = location };
        foreach (EventObserver eo in Object.FindObjectsOfType<EventObserver>())
        {
            eo.OnSpawn(m);
        }
    }
    */
}

/*
public class KillMessage
{
    public Character attacker;
    public Character victim;
    public DamageType causeOfDeath;

    public static void Transmit(Character attacker, Character victim, DamageType causeOfDeath)
    {
        
        KillMessage m = new KillMessage { attacker = attacker, victim = victim, causeOfDeath = causeOfDeath };
        foreach (IKillable i in Object.FindObjectsOfType<IKillable>())
        {
            i.OnKillMessageReceived(m)
        }
        
    }
}
*/


/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// The EventHandler script will exist on a gameobject located in the scene.
// All EventObserver scripts will reference this script
// EventObserver scripts will add their functions to the appropriate delegates when enabled, and remove them when disabled.
// When an appropriate game event happens, that function will use a Transmit function to search for this EventHandler and trigger the referenced functions in the appropriate delegate.


public interface IEventObserver
{
    void OnAttackMessageReceived(AttackMessage m);
    void OnDamageMessageReceived(DamageMessage m);
    void OnKillMessageReceived(KillMessage m);
    void OnInteractMessageReceived(InteractMessage m);
    void OnSpawnMessageReceived(SpawnMessage m);
}

public class EventHandler : MonoBehaviour
{
    //public List<IEventObserver> eventObservers;

    public List<EventObserver> eventObservers;
    
    public event System.Action<Character, Character> OnAttack;
    public event System.Action<Character, Character, DamageType, int> OnDamage;
    public event System.Action<Character, Character, DamageType> OnKill;
    public event System.Action<PlayerHandler, Interactable> OnInteract;
    public event System.Action<Entity, Vector3> OnSpawn;
    
    // Add more delegates, functions, etc. if I need to add new game events in the future

    // This function is run whenever a player or NPC initiates an attack
    public static void TransmitAttack(Character attacker, Character victim)
    {
        EventHandler eh = FindObjectOfType<EventHandler>(); // Search for one
        if (eh != null) // If one is found
        {
            AttackMessage m = new AttackMessage { attacker = attacker, victim = victim };
            foreach (EventObserver eo in eh.eventObservers)
            {
                eo.OnAttackMessage.Invoke(m);
            }
        }
    }

    // This function is run whenever a player or NPC is damaged
    public static void TransmitDamage(Character attacker, Character victim, DamageType method, int amount)
    {
        EventHandler eh = FindObjectOfType<EventHandler>(); // Search for one
        if (eh != null) // If one is found
        {
            DamageMessage m = new DamageMessage { attacker = attacker, victim = victim, method = method, amount = amount };
            foreach (EventObserver eo in eh.eventObservers)
            {
                eo.OnDamageMessage.Invoke(m);
            }
        }
    }

    // This function is run whenever a player or NPC is killed
    public static void TransmitKill(Character attacker, Character victim, DamageType causeOfDeath)
    {
        EventHandler eh = FindObjectOfType<EventHandler>(); // Search for one
        if (eh != null) // If one is found
        {
            KillMessage m = new KillMessage { attacker = attacker, victim = victim, causeOfDeath = causeOfDeath };
            foreach (EventObserver eo in eh.eventObservers)
            {
                //print("Kill functions executed");
                eo.OnKillMessage.Invoke(m);
            }
        }
    }

    // This function is run whenever a player interacts with something in the game world
    public static void TransmitInteract(PlayerHandler player, Interactable interactable)
    {
        EventHandler eh = FindObjectOfType<EventHandler>(); // Search for one
        if (eh != null) // If one is found
        {
            InteractMessage m = new InteractMessage { player = player, interactable = interactable };
            foreach (EventObserver eo in eh.eventObservers)
            {
                eo.OnInteractMessage.Invoke(m);
            }
        }
    }

    // This function is run whenever an entity spawns into the scene
    public static void TransmitSpawn(Entity spawned, Vector3 location)
    {
        EventHandler eh = FindObjectOfType<EventHandler>(); // Search for one
        if (eh != null) // If one is found
        {
            SpawnMessage m = new SpawnMessage { spawned = spawned, location = location };
            foreach (EventObserver eo in eh.eventObservers)
            {
                eo.OnSpawnMessage.Invoke(m);
            }
        }
    }

    
    // This function is run whenever a player or NPC initiates an attack
    public static void TransmitAttack(Character attacker, Character victim)
    {
        EventHandler eh = FindObjectOfType<EventHandler>(); // Search for one
        if (eh != null) // If one is found
        {
            eh.OnAttack(attacker, victim); // Call functions in EventHandler
        }
    }

    // This function is run whenever a player or NPC is damaged
    public static void TransmitDamage(Character attacker, Character victim, DamageType method, int amount)
    {
        EventHandler eh = FindObjectOfType<EventHandler>(); // Search for one
        if (eh != null) // If one is found
        {
            eh.OnDamage(attacker, victim, method, amount); // Call functions in EventHandler
        }
    }

    // This function is run whenever a player or NPC is killed
    public static void TransmitKill(Character killer, Character killed, DamageType causeOfDeath)
    {
        EventHandler eh = FindObjectOfType<EventHandler>(); // Search for one
        if (eh != null) // If one is found
        {
            eh.OnKill(killer, killed, causeOfDeath); // Call functions in EventHandler
        }
    }

    // This function is run whenever a player interacts with something in the game world
    public static void TransmitInteract(PlayerHandler p, Interactable i)
    {
        EventHandler eh = FindObjectOfType<EventHandler>(); // Search for one
        if (eh != null) // If one is found
        {
            eh.OnInteract(p, i); // Call functions in EventHandler
        }
    }

    // This function is run whenever an entity spawns into the scene
    public static void TransmitSpawn(Entity e, Vector3 location)
    {
        EventHandler eh = FindObjectOfType<EventHandler>(); // Search for one
        if (eh != null) // If one is found
        {
            eh.OnSpawn(e, location); // Call functions in EventHandler
        }
    }
    
}

public class AttackMessage
{
    public Character attacker;
    public Character victim;

    
    public static void Transmit(Character attacker, Character victim)
    {
        AttackMessage m = new AttackMessage { attacker = attacker, victim = victim };
        foreach (EventObserver eo in Object.FindObjectsOfType<EventObserver>()) // Find every EventObserver and run its 'OnAttack' function
        {
            eo.OnAttack(m);
        }
    }
    
}

public class DamageMessage
{
    public Character attacker;
    public Character victim;
    public DamageType method;
    public int amount;

    
    public static void Transmit(Character attacker, Character victim, DamageType method, int amount)
    {
        DamageMessage m = new DamageMessage { attacker = attacker, victim = victim, method = method, amount = amount };
        foreach (EventObserver eo in Object.FindObjectsOfType<EventObserver>())
        {
            eo.OnDamage(m);
        }
    }
    
}

public class KillMessage
{
    public Character attacker;
    public Character victim;
    public DamageType causeOfDeath;

    
    public static void Transmit(Character attacker, Character victim, DamageType causeOfDeath)
    {
        KillMessage m = new KillMessage { attacker = attacker, victim = victim, causeOfDeath = causeOfDeath };
        foreach (EventObserver eo in Object.FindObjectsOfType<EventObserver>())
        {
            eo.OnKill(m);
        }
    }
    
    public static KillMessage New(Character attacker, Character victim, DamageType causeOfDeath)
    {
        return new KillMessage { attacker = attacker, victim = victim, causeOfDeath = causeOfDeath };
    }
}

public class InteractMessage
{
    public PlayerHandler player;
    public Interactable interactable;

    
    public static void Transmit(PlayerHandler player, Interactable interactable)
    {
        InteractMessage m = new InteractMessage { player = player, interactable = interactable };
        foreach (EventObserver eo in Object.FindObjectsOfType<EventObserver>())
        {
            eo.OnInteract(m);
        }
    }
    
}

public class SpawnMessage
{
    public Entity spawned;
    public Vector3 location;

    
    public static void Transmit(Entity spawned, Vector3 location)
    {
        SpawnMessage m = new SpawnMessage { spawned = spawned, location = location };
        foreach (EventObserver eo in Object.FindObjectsOfType<EventObserver>())
        {
            eo.OnSpawn(m);
        }
    }
    
}


public class KillMessage
{
    public Character attacker;
    public Character victim;
    public DamageType causeOfDeath;

    public static void Transmit(Character attacker, Character victim, DamageType causeOfDeath)
    {
        
        KillMessage m = new KillMessage { attacker = attacker, victim = victim, causeOfDeath = causeOfDeath };
        foreach (IKillable i in Object.FindObjectsOfType<IKillable>())
        {
            i.OnKillMessageReceived(m)
        }
        
    }
}
*/