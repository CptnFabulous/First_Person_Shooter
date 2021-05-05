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