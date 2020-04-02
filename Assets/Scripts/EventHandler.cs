using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The EventHandler script will exist on a gameobject located in the scene.
// All EventObserver scripts will reference this script
// EventObserver scripts will add their functions to the appropriate delegates when enabled, and remove them when disabled.
// When an appropriate game event happens, that function will use a Transmit function to search for this EventHandler and trigger the referenced functions in the appropriate delegate.
public class EventHandler : MonoBehaviour
{
    public event System.Action<Character, Character> OnAttack;
    public event System.Action<Character, Character, DamageType, int> OnDamage;
    public event System.Action<Character, Character, DamageType> OnKill;
    public event System.Action<PlayerHandler, Interactable> OnInteract;
    public event System.Action<Entity, Vector3> OnSpawn;

    public static void TransmitAttack(Character attacker, Character victim)
    {
        EventHandler eh = FindObjectOfType<EventHandler>(); // Search for one
        if (eh != null) // If one is found
        {
            eh.OnAttack(attacker, victim); // Call functions in EventHandler
        }
    }

    public static void TransmitDamage(Character attacker, Character victim, DamageType method, int amount)
    {
        EventHandler eh = FindObjectOfType<EventHandler>(); // Search for one
        if (eh != null) // If one is found
        {
            eh.OnDamage(attacker, victim, method, amount); // Call functions in EventHandler
        }
    }

    public static void TransmitKill(Character killer, Character killed, DamageType causeOfDeath)
    {
        EventHandler eh = FindObjectOfType<EventHandler>(); // Search for one
        if (eh != null) // If one is found
        {
            eh.OnKill(killer, killed, causeOfDeath); // Call functions in EventHandler
        }
    }

    public static void TransmitInteract(PlayerHandler p, Interactable i)
    {
        EventHandler eh = FindObjectOfType<EventHandler>(); // Search for one
        if (eh != null) // If one is found
        {
            eh.OnInteract(p, i); // Call functions in EventHandler
        }
    }

    public static void TransmitSpawn(Entity e, Vector3 location)
    {
        EventHandler eh = FindObjectOfType<EventHandler>(); // Search for one
        if (eh != null) // If one is found
        {
            eh.OnSpawn(e, location); // Call functions in EventHandler
        }
    }
}