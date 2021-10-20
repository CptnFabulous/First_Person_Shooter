using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemPickup : MonoBehaviour
{
    public bool pickupOnContact = true;
    public bool consumeImmediately = true;

    
    

    public virtual void OnTriggerEnter(Collider c)
    {
        if (pickupOnContact == true)
        {
            PlayerHandler ph = c.GetComponent<PlayerHandler>();
            if (ph != null)
            {
                Pickup(ph);
            }
        }
    }

    public virtual void Pickup(PlayerHandler ph)
    {
        // Do item pickup stuff

        

        if (consumeImmediately == true)
        {
            //print("Item has been picked up");
            Destroy(gameObject);
        }
    }
}
