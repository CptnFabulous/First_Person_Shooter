/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventObserver : MonoBehaviour
{
    public EventHandler eventHandler;

    
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
                eventHandler = FindObjectOfType<EventHandler>(); // Search for the now created EventHandler
            }
        }
    }

    public void OnEnable() // Adds functions to GameEventManager's lists so they can be run appropriately
    {
        eventHandler.OnAttack += Attack;
        eventHandler.OnDamage += Damage;
        eventHandler.OnKill += Kill;
        eventHandler.OnInteract += Interact;
        eventHandler.OnSpawn += Spawn;
    }

    public void OnDisable() // Removes functions from GameEventManager's lists if the object is inactive, so it doesn't eat into memory
    {
        eventHandler.OnAttack -= Attack;
        eventHandler.OnDamage -= Damage;
        eventHandler.OnKill -= Kill;
        eventHandler.OnInteract -= Interact;
        eventHandler.OnSpawn -= Spawn;
    }

    public void Attack(Character attacker, Character victim)
    {
        //BroadcastMessage("OnAttackReceived", attackMessage);
        // somehow assign functions here
    }

    public void Damage(Character attacker, Character victim, DamageType method, int amount)
    {
        // somehow assign functions here
    }

    public void Kill(Character killer, Character killed, DamageType causeOfDeath)
    {
        // somehow assign functions here
    }

    public void Interact(PlayerHandler p, Interactable i)
    {
        // somehow assign functions here
    }

    public void Spawn(Entity e, Vector3 location)
    {
        // somehow assign functions here
    }

    
    public static void TransmitAttack(Character attacker, Character victim)
    {
        foreach (EventObserver eo in FindObjectsOfType<EventObserver>())
        {
            eo.Attack(attacker, victim);
        }
    }
    
}
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class EventObserver : MonoBehaviour
{
    public UnityEvent<AttackMessage> OnAttackMessage;
    public UnityEvent<DamageMessage> OnDamageMessage;
    public UnityEvent<KillMessage> OnKillMessage;
    public UnityEvent<InteractMessage> OnInteractMessage;
    public UnityEvent<SpawnMessage> OnSpawnMessage;



    public EventHandler eventHandler;


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
                eventHandler = FindObjectOfType<EventHandler>(); // Search for the now created EventHandler
            }
        }
    }

    private void OnEnable()
    {
        eventHandler.eventObservers.Add(this);
    }

    private void OnDisable()
    {
        eventHandler.eventObservers.Remove(this);
    }
}