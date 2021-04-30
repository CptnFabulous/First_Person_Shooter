using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : ItemPickup
{
    public int value;
    public override void Pickup(PlayerHandler ph)
    {
        PlayerHealth h = ph.ph;
        if (h != null)
        {
            int healthToReplenish = h.health.max - h.health.current;
            if (healthToReplenish > 0)
            {
                if (healthToReplenish >= value)
                {
                    healthToReplenish = value;
                }

                h.health.current += healthToReplenish;
                value -= healthToReplenish;

                base.Pickup(ph);
            }
        }
    }
}
