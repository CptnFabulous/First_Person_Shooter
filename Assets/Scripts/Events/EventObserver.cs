using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventObserver : MonoBehaviour
{
    /*
    // These events can use multiple parameters at once, but I opted to use events with just one each, so I can easily alter required parameters on the fly
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
        #region Find an EventHandler in the scene, and make one if there isn't. This ensures there is only one EventObserver, and that I do not have to manually add one.

        if (eventHandler == null) // If an EventHandler is not assigned
        {
            eventHandler = FindObjectOfType<EventHandler>(); // Search for one
            if (eventHandler == null) // If one couldn't be found
            {
                // Instantiate object with an EventHandler
                GameObject manager = new GameObject("EventHandler");
                manager.AddComponent<EventHandler>();
                eventHandler = manager.GetComponent<EventHandler>(); // Search for the now created EventHandler
            }
        }
        #endregion

        eventHandler.eventObservers.Add(this);


        
    }

    private void OnDestroy()
    {
        eventHandler.eventObservers.Remove(this);
    }
    


    




    public static void TransmitAttack(AttackMessage m)
    {
        EventHandler eh = FindObjectOfType<EventHandler>(); // Search for an EventHandler
        if (eh != null) // If one is found
        {
            // Refresh the list by removing all null entries, so no errors occur
            eh.eventObservers.RemoveAll(e => e == null);
            
            foreach (EventObserver eo in eh.eventObservers)
            {
                // If the gameobject is active and the delegate has any functions waiting to be ran, run it
                if (eo.gameObject.activeSelf == true && eo.OnAttack != null)
                {
                    eo.OnAttack(m);
                }
            }
        }
    }

    /*
    public static void AddAttackReceiver(System.Action<AttackMessage> action, MonoBehaviour original)
    {
        EventObserver eo = original.GetComponentInParent<EventObserver>();
        if (eo == null)
        {
            eo = original.transform.root.gameObject.AddComponent<EventObserver>();
        }
        
        eo.OnAttack += action;
    }
    */

    /*
    // This function is run whenever a player or NPC initiates an attack
    public static void TransmitAttack(Character attacker, List<Character> victims, float range, float speed)
    {
        EventHandler eh = FindObjectOfType<EventHandler>(); // Search for an EventHandler
        if (eh != null) // If one is found
        {
            Character[] array = victims.ToArray();
            AttackMessage m = AttackMessage.New(attacker, array, range, speed); // Generate new message
            foreach (EventObserver eo in eh.eventObservers)
            {
                if (eo.gameObject.activeSelf == true && eo.OnAttack != null) // If the gameobject is active and the delegate has any functions waiting to be ran, run it
                {
                    eo.OnAttack(m);
                }
            }
        }
    }

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
    */

    // This function is run whenever a player or NPC is damaged
    public static void TransmitDamage(Character attacker, Character victim, DamageType method, int amount)
    {
        EventHandler eh = FindObjectOfType<EventHandler>(); // Search for an EventHandler
        if (eh != null) // If one is found
        {
            DamageMessage m = new DamageMessage(attacker, victim, method, amount); // Generate new message
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
            KillMessage m = new KillMessage(attacker, victim, causeOfDeath); // Generate new message
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
            InteractMessage m = new InteractMessage(player, interactable); // Generate new message
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
            SpawnMessage m = new SpawnMessage(spawned, location); // Generate new message
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