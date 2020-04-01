using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


// The EventHandler script will exist on a gameobject located in the scene.
// All EventObserver scripts will reference this script
// EventObserver scripts will add their functions to the appropriate delegates when enabled, and remove them when disabled.
// When an appropriate game event happens, that function will use a Transmit function to search for this EventHandler trigger the referenced functions in the appropriate delegate.
public class EventHandler : MonoBehaviour
{
    public event System.Action<Character, Character> OnAttack;
    public event System.Action<Character, Character, DamageType, int> OnDamage;
    public event System.Action<Character, Character, DamageType> OnKill;
    public event System.Action<PlayerHandler, Interactable> OnInteract;
    public event System.Action<Entity, Vector3> OnSpawn;

    public static void TransmitAttack(Character attacker, Character victim)
    {
        EventHandler eh = Object.FindObjectOfType<EventHandler>(); // Search for one
        if (eh != null) // If one couldn't be found
        {
            eh.OnAttack(attacker, victim); // Call functions in EventHandler
        }
    }

    public static void TransmitDamage(Character attacker, Character victim, DamageType method, int amount)
    {
        EventHandler eh = Object.FindObjectOfType<EventHandler>(); // Search for one
        if (eh != null) // If one couldn't be found
        {
            eh.OnDamage(attacker, victim, method, amount); // Call functions in EventHandler
        }
    }
    
    public static void TransmitKill(Character killer, Character killed, DamageType causeOfDeath)
    {
        EventHandler eh = Object.FindObjectOfType<EventHandler>(); // Search for one
        if (eh != null) // If one couldn't be found
        {
            eh.OnKill(killer, killed, causeOfDeath); // Call functions in EventHandler
        }
    }
    
    public static void TransmitInteract(PlayerHandler p, Interactable i)
    {
        EventHandler eh = Object.FindObjectOfType<EventHandler>(); // Search for one
        if (eh != null) // If one couldn't be found
        {
            eh.OnInteract(p, i); // Call functions in EventHandler
        }
    }

    public static void TransmitSpawn(Entity e, Vector3 location)
    {
        EventHandler eh = Object.FindObjectOfType<EventHandler>(); // Search for one
        if (eh != null) // If one couldn't be found
        {
            eh.OnSpawn(e, location); // Call functions in EventHandler
        }
    }
}

public class EventObserver : MonoBehaviour
{
    public EventHandler eh;

    public void Awake()
    {
        if (eh == null) // If a GameEventManager is not assigned
        {
            eh = FindObjectOfType<EventHandler>(); // Search for one
            if (eh == null) // If one couldn't be found
            {
                // Instantiate object with GameEventManager
                GameObject manager = Instantiate(new GameObject("EventHandler"), Vector3.zero, Quaternion.identity);
                eh = manager.AddComponent<EventHandler>();
            }
        }
    }

    public void OnEnable() // Adds functions to GameEventManager's lists so they can be run appropriately
    {
        eh.OnAttack += Attack;
        eh.OnDamage += Damage;
        eh.OnKill += Kill;
        eh.OnInteract += Interact;
        eh.OnSpawn += Spawn;
    }

    public void OnDisable() // Removes functions from GameEventManager's lists if the object is inactive, so it doesn't eat into memory
    {
        eh.OnAttack -= Attack;
        eh.OnDamage -= Damage;
        eh.OnKill -= Kill;
        eh.OnInteract -= Interact;
        eh.OnSpawn -= Spawn;
    }

    public void Attack(Character attacker, Character victim)
    {
        
    }

    public void Damage(Character attacker, Character victim, DamageType method, int amount)
    {

    }

    public void Kill(Character killer, Character killed, DamageType causeOfDeath)
    {

    }

    public void Interact(PlayerHandler p, Interactable i)
    {

    }

    public void Spawn(Entity e, Vector3 location)
    {

    }

}

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
