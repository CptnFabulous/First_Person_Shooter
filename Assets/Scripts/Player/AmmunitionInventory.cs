using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AmmunitionType
{
    Pistol,
    Buckshot,
    Slugs,
    Rifle,
    Grenade,
    TaserCharges
}

public class AmmunitionInventory : MonoBehaviour
{
    public Resource[] ammoTypes;

    public int GetStock(AmmunitionType type) // Since our enum is "really" an integer, we can use it as an index to jump straight to the entry we want.
    {
        return ammoTypes[(int)type].current;
    }
    public int GetMax(AmmunitionType type) // Since our enum is "really" an integer, we can use it as an index to jump straight to the entry we want.
    {
        return ammoTypes[(int)type].max;
    }
    public int Collect(AmmunitionType type, int amount) // Returns amount collected, so you can choose to not consume pickups if you're already full (ie. return value is zero).
    {
        Resource held = ammoTypes[(int)type];
        int collect = Mathf.Min(amount, held.max - held.current);
        held.current += collect;
        ammoTypes[(int)type] = held;
        return collect;
    }
    public int Spend(AmmunitionType type, int amount) // Returns the amount actually spent, in case firing a full round would drop us below 0 ammo, you can scale down the last shot. You could also implement a TrySpend that aborts for insufficient ammo.
    {
        Resource held = ammoTypes[(int)type];
        int spend = Mathf.Min(amount, held.current);
        held.current -= spend;
        ammoTypes[(int)type] = held;
        return spend;
    }

    public int TryCollect(AmmunitionType type, int amount, bool onlyAcceptWhole) // Returns amount collected, so you can choose to not consume pickups if you're already full (ie. return value is zero).
    {
        Resource held = ammoTypes[(int)type];
        // If total space is less than the amount of ammo, only pick up enough to fill total space
        int amountThatCanBePickedUp = Mathf.Min(amount, held.max - held.current);

        if (onlyAcceptWhole && amountThatCanBePickedUp < amount)
        {
            return amount;
        }

        held.current += amountThatCanBePickedUp;
        ammoTypes[(int)type] = held;
        // If only part of the amount was picked up, subtract from the original amount to return the remainder.
        amount -= amountThatCanBePickedUp;
        return amount;
    }
    public bool TrySpend(AmmunitionType type, int amount)
    {
        Resource held = ammoTypes[(int)type];
        if (held.current >= amount)
        {
            held.current -= amount;
            ammoTypes[(int)type] = held;
            return true;
        }
        return false;
    }


    void Reset()
    {
        OnValidate();
    }
    void OnValidate() // Ensure our inventory list always matches the enum in the event of code changes. You could also use a custom editor to maintain this more efficiently.
    {
        // Converts old array into a list, so still valid variables can be selected and saved
        List<Resource> oldInventory = new List<Resource>(ammoTypes);
        string[] names = System.Enum.GetNames(typeof(AmmunitionType)); // Obtains name strings for all ammunition types
        Resource[] newAmmoTypes = new Resource[names.Length]; // Then creates an appropriately sized array of resource variables
        for (int i = 0; i < newAmmoTypes.Length; i++) // For each required ammo type
        {
            // Check if a previous version of it already exists with variables assigned
            Resource existingType = oldInventory.Find(old => old.refName == names[i]);
            if (existingType == null) // If it doesn't, the ammo type is new
            {
                existingType = new Resource(names[i], 100, 100, 20); // So declare a new version of it, and assign default values.
            }
            // Saves resource in the correct slot in the array
            newAmmoTypes[i] = existingType;
            // Clamps minimum amount to make sure it doesn't violate the carrying stats
            newAmmoTypes[i].current = Mathf.Clamp(newAmmoTypes[i].current, 0, newAmmoTypes[i].max);
        }
        ammoTypes = newAmmoTypes;
    }
}