using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractFunction : MonoBehaviour
{
    Transform head;
    float maxDistance;

    PlayerHandler ph;
    
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
        if (Physics.Raycast(head.position, head.forward, out thingFound, maxDistance))
        {
            Interactable i = thingFound.collider.GetComponent<Interactable>();
            if (i != null)
            {
                if (Input.GetButtonDown("Interact"))
                {
                    i.OnInteract(ph);
                }
            }
        }
    }
}
