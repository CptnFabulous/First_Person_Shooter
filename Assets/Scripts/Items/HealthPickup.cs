using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : ItemPickup
{
    public int value;
    public override void Pickup(PlayerHandler ph)
    {
        Health h = ph.health;
        if (h != null)
        {
            int healthToReplenish = h.values.max - h.values.current;
            if (healthToReplenish > 0)
            {
                if (healthToReplenish >= value)
                {
                    healthToReplenish = value;
                }

                h.Damage(-healthToReplenish, ph, DamageType.Healing);
                //h.values.current += healthToReplenish;
                value -= healthToReplenish;

                base.Pickup(ph);
            }
        }
    }
}
