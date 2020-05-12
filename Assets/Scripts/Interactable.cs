using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class Interactable : MonoBehaviour
{
    public string instruction;
    public UnityEvent onInteract;

    public virtual void OnInteract(PlayerHandler ph)
    {
        onInteract.Invoke();
        EventObserver.TransmitInteract(ph, this);
    }

    public void PrintMessage(string s)
    {
        print(s);
    }
}
