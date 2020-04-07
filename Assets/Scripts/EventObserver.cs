using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
}