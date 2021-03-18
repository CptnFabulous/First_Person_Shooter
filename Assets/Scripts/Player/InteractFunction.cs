using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractFunction : MonoBehaviour
{
    public PlayerHandler playerHandler;
    public float maxDistance;

    public HeadsUpDisplay hud;

    public LayerMask interactMask = ~0;
    
    /*
    // Start is called before the first frame update
    void Start()
    {
        
    }
    */


    Interactable LookingAt()
    {
        RaycastHit thingFound;
        if (Physics.Raycast(transform.position, transform.forward, out thingFound, maxDistance, interactMask))
        {
            // If an object is found, return whatever interactable the object might have. If the object isn't interactable, it will return null.
            return thingFound.collider.GetComponent<Interactable>();
        }

        return null;
    }

    // Update is called once per frame
    void Update()
    {
        Interactable i = LookingAt();
        if (i != null && i.NotDisabled == true)
        {
            hud.PopulateInteractionMenu(i);

            if (Input.GetButtonDown("Interact"))
            {
                i.OnInteract(playerHandler);
            }
        }
        else
        {
            hud.HideInteractionMenu();
        }
    }


    

}
