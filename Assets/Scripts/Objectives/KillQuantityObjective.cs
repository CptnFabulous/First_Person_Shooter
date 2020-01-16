using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillQuantityObjective : PlayerObjective
{
    public Character enemyType;
    public int quantity;
    [HideInInspector] public int amountEliminated;

    public override void CompletedCheck()
    {
        if (amountEliminated >= quantity)
        {
            Complete();
        }
    }

    public override string DisplayCriteria()
    {
        return description + ": " + amountEliminated + "/" + quantity;
    }

    public void UpdateObjective(KillMessage km)
    {
        if (state == ObjectiveState.Active && (km.victim == enemyType || km.victim.properName == enemyType.properName))
        {
            amountEliminated += 1;
            print("Killed");
        }
    }
}