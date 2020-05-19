using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractFunction : MonoBehaviour
{
    public PlayerHandler playerHandler;
    public float maxDistance;

    public HeadsUpDisplay hud;
    
    /*
    // Start is called before the first frame update
    void Start()
    {
        
    }
    */

    // Update is called once per frame
    void Update()
    {
        RaycastHit thingFound;
        if (Physics.Raycast(transform.position, transform.forward, out thingFound, maxDistance))
        {
            Interactable i = thingFound.collider.GetComponent<Interactable>();
            if (i != null)
            {
                hud.PopulateInteractionMenu(i);
                
                if (Input.GetButtonDown("Interact"))
                {
                    i.OnInteract(playerHandler);
                }
            }
        }
    }
}
