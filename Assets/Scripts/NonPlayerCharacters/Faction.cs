using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum FactionState
{
    Allied,
    Neutral,
    Hostile
}


[CreateAssetMenu(fileName = "New NPC Faction", menuName = "ScriptableObjects/NPC Faction", order = 1)]
public class Faction : ScriptableObject
{
    public Color factionColour;
    public string description;

    public Faction[] alliedFactions;
    public Faction[] enemyFactions;
    
    public FactionState Affiliation(Faction faction)
    {
        if (this == faction) // If factions are the same
        {
            return FactionState.Allied;
        }
        else
        {
            foreach (Faction f in alliedFactions) // Checks allied factions
            {
                if (f == faction) // If target's faction is present in allied factions
                {
                    return FactionState.Allied;
                }
            }

            foreach (Faction f in enemyFactions) // Checks enemy factions
            {
                if (f == faction) // If target's faction is present in enemy factions
                {
                    return FactionState.Hostile;
                }
            }
        }
        return FactionState.Neutral;
    }
}
