using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillGroupObjective : PlayerObjective
{
    public List<Health> enemies;


    private void Awake()
    {
        if (enemies == null || enemies.Count == 0)
        {
            enemies = new List<Health>(GetComponentsInChildren<Health>(false));
        }
    }
    /*
    public override void CheckCompletion()
    {
        return; // Redundant, 
    }
    */
    public override string DisplayCriteria()
    {
        return name + ": " + enemies.Count + " remaining";
    }

    public void UpdateObjective(KillMessage km)
    {
        if (state != ObjectiveState.Active)
        {
            return;
        }

        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i] == km.victim)
            {
                enemies.Remove(enemies[i]);
                break;
            }
        }

        if (enemies.Count <= 0)
        {
            Complete();
        }
    }
}