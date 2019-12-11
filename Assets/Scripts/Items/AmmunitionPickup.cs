using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmunitionPickup : ItemPickup
{
    public AmmunitionType ammoType;
    public int amount;

    public void OnValidate()
    {
        
        /*
        if (ammoType == AmmunitionType.None)
        {
            amount = Mathf.Clamp(amount, 0, 100);
            consumeAll = true;
        }
        */
    }

    public override void Pickup(Collider c)
    {
        AmmunitionInventory ai = c.GetComponent<AmmunitionInventory>();
        
        if (ai != null && ai.GetStock(ammoType) <= ai.GetMax(ammoType))
        {
            amount -= ai.Collect(ammoType, amount);
            if (amount <= 0)
            {
                Destroy(gameObject);
            }

            base.Pickup(c);
        }
    }
}
