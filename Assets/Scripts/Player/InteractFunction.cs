using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractFunction : MonoBehaviour
{
    public PlayerHandler playerHandler;
    public float maxDistance;
    public LayerMask interactMask = ~0;

    Interactable LookingAt()
    {
        RaycastHit thingFound;
        if (Physics.Raycast(transform.position, transform.forward, out thingFound, maxDistance, interactMask))
        {
            // Find every Interactable monobehaviour on the gameObject
            Interactable[] array = thingFound.collider.GetComponents<Interactable>();
            for (int c = 0; c < array.Length; c++)
            {
                // Find the first Interactable script that is enabled for interaction
                if (array[c].enabled == true)
                {
                    return array[c];
                    //c = array.Length; // Prematurely end loop once an enabled Interactable has been found, so time is not wasted.
                }
            }

            // If an object is found, return whatever interactable the object might have. If the object isn't interactable, it will return null.
            //i = thingFound.collider.GetComponent<Interactable>();
        }

        return null;
    }

    // Update is called once per frame
    void Update()
    {
        Interactable i = LookingAt();

        // If the player is looking at something interactable, display the tooltip window.
        if (i != null)
        {
            playerHandler.hud.PopulateInteractionMenu(i);

            // If the player presses the interact button
            // If the player can currently interact with the object
            // If the object is not cooling down or in the middle of performing an action
            if (Input.GetButtonDown("Interact") && i.CanPlayerInteract(playerHandler) && i.InProgress == false)
            {
                i.OnInteract(playerHandler);
            }
        }
        else
        {
            playerHandler.hud.HideInteractionMenu();
        }
    }
}
