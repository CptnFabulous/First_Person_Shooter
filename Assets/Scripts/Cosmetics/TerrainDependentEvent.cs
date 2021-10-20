using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TerrainDependentEvent : MonoBehaviour
{
    
    [System.Serializable]
    public class EventForSpecificTerrain : UnityEvent<TerrainType>
    {
        public string name;
    }
    

    public enum TerrainType
    {
        Dirt,
        Foliage,
        Wood,
        Stone,
        Metal,
        Flesh
    }


    public EventForSpecificTerrain[] possibleEvents;




    void Reset()
    {
        OnValidate();
    }
    void OnValidate() // Ensure our inventory list always matches the enum in the event of code changes. You could also use a custom editor to maintain this more efficiently.
    {
        // Converts old array into a list, so still valid variables can be selected and saved
        List<EventForSpecificTerrain> oldEvents = new List<EventForSpecificTerrain>(possibleEvents);
        string[] names = System.Enum.GetNames(typeof(TerrainType)); // Obtains name strings for all types
        EventForSpecificTerrain[] newOutcomes = new EventForSpecificTerrain[names.Length]; // Then creates an appropriately sized array of
        for (int i = 0; i < newOutcomes.Length; i++) // For each required ammo type
        {
            // Check if a previous version of it already exists with variables assigned
            EventForSpecificTerrain existingType = oldEvents.Find(old => old.name == names[i]);
            if (existingType == null) // If it doesn't, it's new
            {
                existingType = new EventForSpecificTerrain();
                existingType.name = names[i];
            }
            // Saves resource in the correct slot in the array
            newOutcomes[i] = existingType;
        }
        possibleEvents = newOutcomes;
    }



    public void Invoke(TerrainType t)
    {
        //possibleEvents[(int)t].n
        possibleEvents[(int)t].Invoke(t);
    }
}
