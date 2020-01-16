using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractObjective : PlayerObjective
{
    public Interactable objectiveInteractable;
    [HideInInspector] public bool completed;

    public void UpdateObjective(PlayerHandler player, Interactable interactable)
    {
        if (interactable == objectiveInteractable)
        {
            CompletedCheck();
        }
    }
}