using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniversalAmmunitionPickup : ItemPickup
{
    [Range(0, 100)] public float percentageValue;

    public override void Pickup(Collider c)
    {
        AmmunitionInventory ai = c.GetComponent<AmmunitionInventory>();
        if (ai != null)
        {
            
            for (int i = 0; i < System.Enum.GetValues(typeof(AmmunitionType)).Length; i++)
            {
                AmmunitionType a = (AmmunitionType)i;
                if (a != AmmunitionType.None)
                {
                    int amountToRestore = Mathf.RoundToInt(ai.GetMax(a) * (percentageValue / 100));
                    ai.Collect(a, amountToRestore);
                }
            }
        }

        base.Pickup(c);
    }
}
