using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FactionAllegiance
{
    Allied,
    Hostile
}

[System.Serializable]
public class FactionRelationship
{
    public NPCFaction faction;
    public FactionAllegiance allegiance;
}

[CreateAssetMenu(fileName = "New NPC Faction", menuName = "ScriptableObjects/NPC Faction", order = 1)]
public class NPCFaction : ScriptableObject
{
    public Color factionColour;
    public string description;

    public FactionRelationship[] relationships;
}
