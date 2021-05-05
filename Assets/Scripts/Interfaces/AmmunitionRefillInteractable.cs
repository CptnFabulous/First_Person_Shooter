using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmunitionRefillInteractable : Interactable
{
    AmmunitionPickup ammoData;

    private void Awake()
    {
        ammoData = GetComponent<AmmunitionPickup>();
    }

    public override void OnInteract(PlayerHandler ph)
    {
        base.OnInteract(ph);

        ph.a.Collect(ammoData.ammoType, ammoData.amount);
    }

    // Checks if the player is capable of carrying this ammunition, and that they have empty space to carry it.
    public override bool CanPlayerInteract(PlayerHandler ph)
    {
        AmmunitionInventory ai = ph.a;
        if (ai != null && ai.GetStock(ammoData.ammoType) < ai.GetMax(ammoData.ammoType))
        {
            return true;
        }
        
        return false;
    }
}