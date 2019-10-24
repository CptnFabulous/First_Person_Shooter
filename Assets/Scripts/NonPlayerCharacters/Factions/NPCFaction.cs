using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
public enum FactionAffiliation
{
    Allied,
    Neutral,
    Hostile
}
*/

[CreateAssetMenu(fileName = "New NPC Faction", menuName = "ScriptableObjects/NPC Faction", order = 1)]
public class NPCFaction : ScriptableObject
{
    public Color factionColour;
    public string description;

    public NPCFaction[] alliedFactions;
    public NPCFaction[] enemyFactions;

    public bool IsFriendlyTo(NPCFaction faction)
    {
        if (this == faction) // If factions are the same
        {
            return true;
        }
        else
        {
            foreach (NPCFaction f in alliedFactions) // Checks allied factions
            {
                if (f == faction) // If target's faction is present in allied factions
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool IsHostileTo(NPCFaction faction)
    {
        foreach (NPCFaction f in enemyFactions) // Checks enemy factions
        {
            if (f == faction) // If target's faction is present in enemy factions
            {
                return true;
            }
        }
        return false;
    }

    /*
    public FactionAffiliation FactionState(NPCFaction faction)
    {
        if (this == faction) // If factions are the same
        {
            return FactionAffiliation.Allied;
        }
        else
        {
            foreach (NPCFaction f in alliedFactions) // Checks allied factions
            {
                if (f == faction) // If target's faction is present in allied factions
                {
                    return FactionAffiliation.Allied;
                }
            }

            foreach (NPCFaction f in enemyFactions) // Checks enemy factions
            {
                if (f == faction) // If target's faction is present in enemy factions
                {
                    return FactionAffiliation.Hostile;
                }
            }
        }
        return FactionAffiliation.Neutral;
    }
    */
}
