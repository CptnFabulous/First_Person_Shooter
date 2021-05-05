using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DoorInteractable : Interactable
{
    [Header("Door specific variables")]
    public bool isLocked;
    public Animator animationHandler;
    public string openBoolName;
    public bool openAtStart;

    public void Start()
    {
        animationHandler.SetBool(openBoolName, openAtStart);
    }

    public override void OnInteract(PlayerHandler ph)
    {
        animationHandler.SetBool(openBoolName, !animationHandler.GetBool(openBoolName));
        Debug.Log("Toggled door state, frame " + Time.frameCount);
        base.OnInteract(ph);
    }

    public override bool CanPlayerInteract(PlayerHandler ph)
    {
        return !isLocked;
    }
}
