using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public abstract class Interactable : MonoBehaviour
{
    public string instruction;

    public virtual void OnInteract(PlayerHandler ph)
    {
        GameEvent.TransmitInteraction(ph, this);
    }
}
