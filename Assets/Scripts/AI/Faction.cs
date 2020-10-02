using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New NPC Faction", menuName = "ScriptableObjects/NPC Faction", order = 1)]
public class Faction : ScriptableObject
{
    public Color factionColour;
    public string description;

    public Faction[] allies;
    public bool friendlyFire;

    public bool HostileTowards(Faction other)
    {
        if (other == this && friendlyFire == false)
        {
            return false;
        }

        foreach (Faction f in allies)
        {
            if ( f == other)
            {
                return false;
            }
        }

        return true;
    }
}
