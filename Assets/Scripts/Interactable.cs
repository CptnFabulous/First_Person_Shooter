using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[RequireComponent(typeof(Collider))]
public class Interactable : MonoBehaviour
{
    public bool enabled = true;
    /*
    private bool _enabled;
    public bool Enabled
    {
        get
        {
            return _enabled;
        }

        set
        {
            _enabled = value;
            foreach (var obj in GetComponents<Interactable>())
            {
                obj._enabled = value;
            }
        }
    }
    */

    public UnityEvent onInteract;
    public float cooldown = 0.5f;
    public string instructionMessage = "Interact";
    public string inProgressMessage = "In progress";
    public string deniedMessage = "Cannot interact";

    public bool InProgress { get; private set; } = false;
    
    IEnumerator coolingDown;
    
    public virtual void OnInteract(PlayerHandler ph)
    {
        onInteract.Invoke();
        EventObserver.TransmitInteract(ph, this);
        coolingDown = Cooldown(cooldown);
        StartCoroutine(coolingDown);
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



/*
[RequireComponent(typeof(Collider))]
public class Interactable : MonoBehaviour
{
    [System.Serializable]
    public struct Interaction
    {
        public bool enabled;
        public UnityEvent onInteract;
        public float cooldown;
        public string instructionMessage;
        public string inProgressMessage;
        public string disabledMessage;
    }

    public Interaction[] possibleInteractions = new Interaction[1];
    public int currentlyActive;

    public bool InProgress { get; private set; } = false;

    IEnumerator coolingDown;

    public Interaction Current()
    {
        return possibleInteractions[currentlyActive];
    }

    public virtual void OnInteract(PlayerHandler ph)
    {
        // Set something up so f the object is merely locked and doesn’t meet the criteria for some reason, it will display a locked message, and if the interactability is disabled, nothing will occur.

        Interaction i = possibleInteractions[currentlyActive];
        i.onInteract.Invoke();
        EventObserver.TransmitInteract(ph, this);
        coolingDown = Cooldown(i.cooldown);
        StartCoroutine(coolingDown);
    }

    IEnumerator Cooldown(float duration)
    {
        InProgress = true;
        yield return new WaitForSeconds(duration);
        InProgress = false;
    }
}
*/
