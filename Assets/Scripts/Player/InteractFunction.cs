using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractFunction : MonoBehaviour
{
    public PlayerHandler playerHandler;
    public HeadsUpDisplay hud;
    public float maxDistance;
    public LayerMask interactMask = ~0;
    
    /*
    // Start is called before the first frame update
    void Start()
    {
        
    }
    */


    // Update is called once per frame
    void Update()
    {
        #region Is the player looking at something interactable?
        Interactable i = null;
        RaycastHit thingFound;
        if (Physics.Raycast(transform.position, transform.forward, out thingFound, maxDistance, interactMask))
        {
            /*
            Interactable[] array = thingFound.collider.GetComponents<Interactable>();
            List<Interactable> interactableStates = new List<Interactable>(array);
            */

            // Find every Interactable monobehaviour on the gameObject
            // Isolate the ones that are enabled
            // Select one





            // If an object is found, return whatever interactable the object might have. If the object isn't interactable, it will return null.
            i = thingFound.collider.GetComponent<Interactable>();
        }
        #endregion

        #region Display interaction prompt and allow button input
        if (i != null && i.enabled == true)
        {
            //Debug.Log(Time.frameCount);
            hud.PopulateInteractionMenu(i);

            


            if (i.InProgress == false && Input.GetButtonDown("Interact"))
            {
                i.OnInteract(playerHandler);
            }
        }
        else
        {
            hud.HideInteractionMenu();
        }
        #endregion
    }




}
