using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractObjective : PlayerObjective
{
    public Interactable objectiveInteractable;
    [HideInInspector] public bool completed;
    
    /*
    public override void CheckCompletion()
    {
        return; // As only a single interactable is being checked, CompletedCheck is unnecessary as Complete() can be run from UpdateObjective() if the object has been interacted with
    }
    */
    
    public void UpdateObjective(PlayerHandler player, Interactable interactable)
    {
        if (interactable == objectiveInteractable)
        {
            Complete();
        }
    }
}