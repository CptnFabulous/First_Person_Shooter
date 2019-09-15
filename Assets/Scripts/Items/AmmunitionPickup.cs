using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmunitionPickup : ItemPickup
{
    public AmmunitionType ammoType;

    public void OnValidate()
    {
        if (ammoType == AmmunitionType.None)
        {
            value = Mathf.Clamp(value, 0, 100);
            consumeAll = true;
        }
    }

    public override void OnTriggerEnter(Collider c)
    {
        AmmunitionInventory ai = c.GetComponent<AmmunitionInventory>();
        if (ai != null)
        {
            if (ammoType == AmmunitionType.None)
            {
                //refill all ammo by a percentage
                /*
                foreach()
                {

                }
                */
            }
            else if (ai.GetStock(ammoType) <= ai.GetMax(ammoType))
            {
                value -= ai.Collect(ammoType, value);
                if (consumeAll == true)
                {
                    value = 0;
                }
            }

            base.OnTriggerEnter(c);
        }

        
    }
}
