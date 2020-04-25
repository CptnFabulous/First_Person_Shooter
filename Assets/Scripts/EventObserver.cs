using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventObserver : MonoBehaviour
{
    /*
    public event System.Action<Character, Character> OnAttack;
    public event System.Action<Character, Character, DamageType, int> OnDamage;
    public event System.Action<KillMessage> OnKill;
    public event System.Action<PlayerHandler, Interactable> OnInteract;
    public event System.Action<Entity, Vector3> OnSpawn;
    */

    public event System.Action<AttackMessage> OnAttack;
    public event System.Action<DamageMessage> OnDamage;
    public event System.Action<KillMessage> OnKill;
    public event System.Action<InteractMessage> OnInteract;
    public event System.Action<SpawnMessage> OnSpawn;

    [HideInInspector] public EventHandler eventHandler;


    public void Awake()
    {
        if (eventHandler == null) // If a GameEventManager is not assigned
        {
            eventHandler = FindObjectOfType<EventHandler>(); // Search for one
            if (eventHandler == null) // If one couldn't be found
            {
                // Instantiate object with GameEventManager
                GameObject manager = Instantiate(new GameObject("EventHandler"), Vector3.zero, Quaternion.identity);
                manager.AddComponent<EventHandler>();
                eventHandler = manager.GetComponent<EventHandler>(); // Search for the now created EventHandler
            }
        }

        if (eventHandler != null)
        {
            eventHandler.eventObservers.Add(this);
        }
    }

    private void OnDestroy()
    {
        eventHandler.eventObservers.Remove(this);
    }

    /*
    private void OnEnable()
    {
        eventHandler.eventObservers.Add(this);
    }

    private void OnDisable()
    {
        eventHandler.eventObservers.Remove(this);
    }
    */

    // This function is run whenever a player or NPC initiates an attack
    public static void TransmitAttack(Character attacker, Character victim, float range, float speed)
    {
        EventHandler eh = FindObjectOfType<EventHandler>(); // Search for an EventHandler
        if (eh != null) // If one is found
        {
            AttackMessage m = AttackMessage.New(attacker, victim, range, speed); // Generate new message
            foreach (EventObserver eo in eh.eventObservers)
            {
                if (eo.gameObject.activeSelf == true && eo.OnAttack != null) // If the gameobject is active and the delegate has any functions waiting to be ran, run it
                {
                    eo.OnAttack(m);
                }
            }
        }

    }

    // This function is run whenever a player or NPC is damaged
    public static void TransmitDamage(Character attacker, Character victim, DamageType method, int amount)
    {
        EventHandler eh = FindObjectOfType<EventHandler>(); // Search for an EventHandler
        if (eh != null) // If one is found
        {
            DamageMessage m = DamageMessage.New(attacker, victim, method, amount); // Generate new message
            foreach (EventObserver eo in eh.eventObservers)
            {
                if (eo.gameObject.activeSelf == true && eo.OnDamage != null) // If the gameobject is active and the delegate has any functions waiting to be ran, run it
                {
                    eo.OnDamage(m);
                }
            }
        }
    }

    // This function is run whenever a player or NPC is killed
    public static void TransmitKill(Character attacker, Character victim, DamageType causeOfDeath)
    {
        EventHandler eh = FindObjectOfType<EventHandler>(); // Search for an EventHandler
        if (eh != null) // If one is found
        {
            KillMessage m = KillMessage.New(attacker, victim, causeOfDeath); // Generate new message
            foreach (EventObserver eo in eh.eventObservers)
            {
                if (eo.gameObject.activeSelf == true && eo.OnKill != null) // If the gameobject is active and the delegate has any functions waiting to be ran, run it
                {
                    eo.OnKill(m);
                }
            }
        }
    }

    // This function is run whenever a player interacts with something in the game world
    public static void TransmitInteract(PlayerHandler player, Interactable interactable)
    {
        EventHandler eh = FindObjectOfType<EventHandler>(); // Search for an EventHandler
        if (eh != null) // If one is found
        {
            InteractMessage m = InteractMessage.New(player, interactable); // Generate new message
            foreach (EventObserver eo in eh.eventObservers)
            {
                if (eo.gameObject.activeSelf == true && eo.OnInteract != null) // If the gameobject is active and the delegate has any functions waiting to be ran, run it
                {
                    eo.OnInteract(m);
                }
            }
        }
    }

    // This function is run whenever an entity spawns into the scene
    public static void TransmitSpawn(Entity spawned, Vector3 location)
    {
        EventHandler eh = FindObjectOfType<EventHandler>(); // Search for an EventHandler
        if (eh != null) // If one is found
        {
            SpawnMessage m = SpawnMessage.New(spawned, location); // Generate new message
            foreach (EventObserver eo in eh.eventObservers)
            {
                if (eo.gameObject.activeSelf == true && eo.OnSpawn != null) // If the gameobject is active and the delegate has any functions waiting to be ran, run it
                {
                    eo.OnSpawn(m);
                }
            }
        }
    }

    /*
    public static void TransmitKill(Character attacker, Character victim, DamageType causeOfDeath)
    {
        EventHandler eh = FindObjectOfType<EventHandler>(); // Search for one
        if (eh != null) // If one is found
        {
            print("Event Handler Found");
            KillMessage m = KillMessage.New(attacker, victim, causeOfDeath);
            print(eh.eventObservers);
            foreach (EventObserver eo in eh.eventObservers)
            {
                print("Sending message to event observer " + eo.name);
                //eo.OnKill(m); // Call functions in EventHandler
                //eo?.OnKill.Invoke(m); Should add a safety for NullReferenceExceptions, but only works in C# 6 or greater, apparently
                
                if (eo.OnKill != null)
                {
                    eo.OnKill(m);
                }
            }
        }
    }
    */
}