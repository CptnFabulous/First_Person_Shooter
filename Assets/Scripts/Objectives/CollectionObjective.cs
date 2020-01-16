using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectionObjective : PlayerObjective
{
    public ItemPickup itemType;
    public int quantity;
    [HideInInspector] public int amountObtained;

    public override void CompletedCheck()
    {
        if (amountObtained >= quantity)
        {
            Complete();
        }
    }

    public override string DisplayCriteria()
    {
        return description + (": ") + amountObtained + ("/") + quantity;
    }
}