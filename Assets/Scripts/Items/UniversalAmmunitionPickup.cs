using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniversalAmmunitionPickup : ItemPickup
{
    [Range(0, 100)] public float percentageValue;

    public override void Pickup(PlayerHandler ph)
    {
        AmmunitionInventory ai = ph.a;
        if (ai != null)
        {
            bool hasBeenConsumed = false;
            for (int i = 0; i < System.Enum.GetValues(typeof(AmmunitionType)).Length; i++)
            {
                AmmunitionType a = (AmmunitionType)i;
                if (ai.GetStock(a) < ai.GetMax(a))
                {
                    int amountToRestore = Mathf.RoundToInt(ai.GetMax(a) * (percentageValue / 100));
                    amountToRestore = Mathf.Clamp(amountToRestore, 1, ai.GetMax(a));
                    ai.Collect(a, amountToRestore);
                    hasBeenConsumed = true;
                }
            }

            if (hasBeenConsumed == true)
            {
                Destroy(gameObject);
            }
        }
    }
}
