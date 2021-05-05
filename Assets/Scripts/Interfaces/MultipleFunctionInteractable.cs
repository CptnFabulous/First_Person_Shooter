using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MultipleFunctionInteractable : Interactable
{
    [System.Serializable]
    public struct Interaction
    {
        public UnityEvent onInteract;
        public float cooldown;
        public string instructionMessage;
        public string inProgressMessage;
        public string deniedMessage;

        public Interaction(UnityEvent _onInteract, float _cooldown, string _instructionMessage, string _inProgressMessage, string _deniedMessage)
        {
            onInteract = _onInteract;
            cooldown = _cooldown;
            instructionMessage = _instructionMessage;
            inProgressMessage = _inProgressMessage;
            deniedMessage = _deniedMessage;
        }
        
        public static Interaction Empty()
        {
            return new Interaction(null, 0.5f, "Interact", "In progress", "Cannot interact");
        }
        
    }

    public Interaction[] additionalInteractions;
    public int interactionIndex;

    public Interaction CurrentPrompt()
    {
        if (interactionIndex <= 0)
        {
            return new Interaction(onInteract, cooldown, instructionMessage, inProgressMessage, deniedMessage);
        }
        else
        {
            return additionalInteractions[interactionIndex - 1];
        }
    }






    public override void OnInteract(PlayerHandler ph)
    {
        
    }


}
