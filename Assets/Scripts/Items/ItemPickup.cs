using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemPickup : MonoBehaviour
{
    public bool consumeImmediately = true;
    

    public virtual void OnTriggerEnter(Collider c)
    {
        if (c.GetComponent<PlayerHandler>() != null)
        {
            Pickup(c);
        }
    }

    public virtual void Pickup(Collider c)
    {
        // Do item pickup stuff

        if (consumeImmediately == true)
        {
            print("blah");
            Destroy(gameObject);
        }
    }
}
