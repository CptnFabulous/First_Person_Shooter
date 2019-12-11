using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : ItemPickup
{
    public int value;
    public override void Pickup(Collider c)
    {
        PlayerHealth ph = c.GetComponent<PlayerHealth>();
        if (ph != null)
        {
            int healthToReplenish = ph.health.max - ph.health.current;

            if (healthToReplenish >= value)
            {
                healthToReplenish = value;
            }

            ph.health.current += healthToReplenish;
            value -= healthToReplenish;

            

            base.Pickup(c);
        }


    }
}
