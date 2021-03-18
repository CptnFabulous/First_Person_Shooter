using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class Interactable : MonoBehaviour
{
    public UnityEvent onInteract;
    public float cooldown = 0.5f;
    public string instruction = "Interact";
    public string disabledMessage = "Cannot interact";

    public bool NotDisabled { get; private set; } = true;
    
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
        NotDisabled = false;
        yield return new WaitForSeconds(duration);
        NotDisabled = true;
    }
    




    public void PrintMessage(string s)
    {
        print(s);
    }

}
