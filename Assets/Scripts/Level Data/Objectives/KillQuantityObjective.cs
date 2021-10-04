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
        return name + ": " + amountEliminated + "/" + quantity;
    }

    public void UpdateObjective(KillMessage km)
    {
        print("Checking quantity objective");
        if (state == ObjectiveState.Active && (km.victim == enemyType || km.victim.GetComponent<Entity>().properName == enemyType.properName))
        {
            print("Enemy eliminated");
            amountEliminated += 1;
        }
    }
}