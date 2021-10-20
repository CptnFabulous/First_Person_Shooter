using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectionObjective : PlayerObjective
{
    public ItemPickup itemType;
    public int quantity;
    [HideInInspector] public int amountObtained;

    public override void CheckCompletion()
    {
        if (amountObtained >= quantity)
        {
            Complete();
        }
    }

    public override string DisplayCriteria()
    {
        return name + (": ") + amountObtained + ("/") + quantity;
    }

    public void UpdateObjective()
    {

    }
}