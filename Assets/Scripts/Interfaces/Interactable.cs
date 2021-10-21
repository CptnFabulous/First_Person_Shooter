using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[RequireComponent(typeof(Collider))]
public class Interactable : MonoBehaviour
{
    [System.Serializable]
    public class InteractionEvent : UnityEvent<PlayerHandler> { }

    public bool enabled = true;
    public InteractionEvent onInteract;
    public float cooldown = 0.5f;
    public string instructionMessage = "Interact";
    public string inProgressMessage = "In progress";
    public string deniedMessage = "Cannot interact";

    public bool InProgress { get; private set; } = false;

    
    IEnumerator coolingDown;
    
    public virtual void OnInteract(PlayerHandler ph)
    {
        onInteract.Invoke(ph);
        EventJunction.Transmit(new InteractMessage(ph, this));

        if (cooldown > 0)
        {
            coolingDown = Cooldown(cooldown);
            StartCoroutine(coolingDown);
        }
        
    }

    IEnumerator Cooldown(float duration)
    {
        InProgress = true;
        yield return new WaitForSeconds(duration);
        InProgress = false;
    }

    // Is something preventing this particular player from being able to interact with this object?
    public virtual bool CanPlayerInteract(PlayerHandler ph)
    {
        return true;
    }
}