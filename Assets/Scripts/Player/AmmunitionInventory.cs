using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AmmunitionType
{

    /*Rimfire,
    PistolLight,
    PistolHeavy,
    Magnum,
    PDW,
    RifleLight,
    RifleHeavy,
    SniperLight,
    SniperHeavy,
    Buckshot,
    Slugs,
    Grenade,
    Napalm,
    */
    Pistol,
    Buckshot,
    Slugs,
    DragonsBreath,
    Rifle,
    Grenade,
    Petrol,
    None
}

public class AmmunitionInventory : MonoBehaviour
{
    public List<Resource> inventory = new List<Resource>();

    public int GetStock(AmmunitionType type) // Since our enum is "really" an integer, we can use it as an index to jump straight to the entry we want.
    {
        if (type == AmmunitionType.None)
        {
            return int.MaxValue;
        }

        return inventory[(int)type].current;
    }

    public int GetMax(AmmunitionType type) // Since our enum is "really" an integer, we can use it as an index to jump straight to the entry we want.
    {
        if (type == AmmunitionType.None)
        {
            return int.MaxValue;
        }

        return inventory[(int)type].max;
    }

    public int Collect(AmmunitionType type, int amount) // Returns amount collected, so you can choose to not consume pickups if you're already full (ie. return value is zero).
    {
        if (type == AmmunitionType.None)
        {
            return amount;
        }

        Resource held = inventory[(int)type];
        int collect = Mathf.Min(amount, held.max - held.current);
        held.current += collect;
        inventory[(int)type] = held;
        return collect;
    }

    public int Spend(AmmunitionType type, int amount) // Returns the amount actually spent, in case firing a full round would drop us below 0 ammo, you can scale down the last shot. You could also implement a TrySpend that aborts for insufficient ammo.
    {
        if (type == AmmunitionType.None)
        {
            return amount;
        }

        Resource held = inventory[(int)type];
        int spend = Mathf.Min(amount, held.current);
        held.current -= spend;
        inventory[(int)type] = held;
        return spend;
    }

#if UNITY_EDITOR
    void Reset() { OnValidate(); }
    void OnValidate() // Ensure our inventory list always matches the enum in the event of code changes. You could also use a custom editor to maintain this more efficiently.
    {
        var ammoNames = System.Enum.GetNames(typeof(AmmunitionType));
        var inv = new List<Resource>(ammoNames.Length);
        for (int i = 0; i < ammoNames.Length; i++)
        {
            if (ammoNames[i] != "None") // For each actual ammo type, generate stock variables
            {
                var existing = inventory.Find((entry) => { return entry.refName == ammoNames[i]; });
                existing.refName = ammoNames[i];
                existing.current = Mathf.Min(existing.current, existing.max);
                inv.Add(existing);
            }
        }
        inventory = inv;
    }
#endif
}