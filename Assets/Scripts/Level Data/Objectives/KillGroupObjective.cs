using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillGroupObjective : PlayerObjective
{
    public List<Character> enemies;

    public override void CompletedCheck()
    {
        if (enemies.Count <= 0) // Check if no enemies are alive
        {
            Complete();
        }
    }

    public override string DisplayCriteria()
    {
        return name + ": " + enemies.Count + " remaining";
    }

    public void UpdateObjective(KillMessage km)
    {
        if (state == ObjectiveState.Active)
        {
            foreach (Character c in enemies)
            {
                if (c == km.victim)
                {
                    enemies.Remove(c);
                }
            }
        }
    }
}