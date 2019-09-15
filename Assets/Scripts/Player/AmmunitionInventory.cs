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
    [System.Serializable]
    public struct AmmoEntry
    {

        // Include name field for editing convenience only - not needed in-game.
#if UNITY_EDITOR
        [HideInInspector]
        public string refName;
#endif
        //public string name;
        public int stockMax;
        public int stockCurrent;
    }

    // Keep a private list of ammo stocks, so we can enforce capacity rules.
    //public List<AmmoEntry> inventory = new List<AmmoEntry>();
    [SerializeField] List<AmmoEntry> inventory = new List<AmmoEntry>();

    public int GetStock(AmmunitionType type) // Since our enum is "really" an integer, we can use it as an index to jump straight to the entry we want.
    {
        if (type == AmmunitionType.None)
        {
            return int.MaxValue;
        }

        return inventory[(int)type].stockCurrent;
    }

    public int GetMax(AmmunitionType type) // Since our enum is "really" an integer, we can use it as an index to jump straight to the entry we want.
    {
        if (type == AmmunitionType.None)
        {
            return int.MaxValue;
        }

        return inventory[(int)type].stockMax;
    }


    public int Collect(AmmunitionType type, int amount) // Returns amount collected, so you can choose to not consume pickups if you're already full (ie. return value is zero).
    {
        if (type == AmmunitionType.None)
        {
            return amount;
        }

        AmmoEntry held = inventory[(int)type];
        int collect = Mathf.Min(amount, held.stockMax - held.stockCurrent);
        held.stockCurrent += collect;
        inventory[(int)type] = held;
        return collect;
    }

    
    public int Spend(AmmunitionType type, int amount) // Returns the amount actually spent, in case firing a full round would drop us below 0 ammo, you can scale down the last shot. You could also implement a TrySpend that aborts for insufficient ammo.
    {
        if (type == AmmunitionType.None)
        {
            return amount;
        }

        AmmoEntry held = inventory[(int)type];
        int spend = Mathf.Min(amount, held.stockCurrent);
        held.stockCurrent -= spend;
        inventory[(int)type] = held;
        return spend;
    }

    
#if UNITY_EDITOR
    void Reset() { OnValidate(); }
    void OnValidate() // Ensure our inventory list always matches the enum in the event of code changes. You could also use a custom editor to maintain this more efficiently.
    {
        var ammoNames = System.Enum.GetNames(typeof(AmmunitionType));
        var inv = new List<AmmoEntry>(ammoNames.Length);
        for (int i = 0; i < ammoNames.Length; i++)
        {
            if (ammoNames[i] != "None") // For each actual ammo type, generate stock variables
            {
                var existing = inventory.Find((entry) => { return entry.refName == ammoNames[i]; });
                existing.refName = ammoNames[i];
                existing.stockCurrent = Mathf.Min(existing.stockCurrent, existing.stockMax);
                inv.Add(existing);
            }
        }
        inventory = inv;
    }
#endif
}

/*
//public class AmmoInventory : MonoBehaviour
public class AmmunitionInventory : MonoBehaviour
{

    [System.Serializable]
    public struct AmmoEntry
    {
        
        // Include name field for editing convenience only - not needed in-game.
#if UNITY_EDITOR
        [HideInInspector]
        public string refName;
#endif
        //public string name;
        public int stockMax;
        public int stockCurrent;
    }

    // Keep a private list of ammo stocks, so we can enforce capacity rules.
    [SerializeField]
    List<AmmoEntry> _inventory = new List<AmmoEntry>();

    // Since our enum is "really" an integer, we can use it
    // as an index to jump straight to the entry we want.
    public int GetStock(AmmunitionType type)
    {
        return _inventory[(int)type].stockCurrent;
    }

    // Returns amount collected, so you can choose to not consume
    // pickups if you're already full (ie. return value is zero).
    public int Collect(AmmunitionType type, int amount)
    {
        AmmoEntry held = _inventory[(int)type];
        int collect = Mathf.Min(amount, held.stockMax - held.stockCurrent);
        held.stockCurrent += collect;
        _inventory[(int)type] = held;
        return collect;
    }

    // Returns the amount actually spent, in case firing a full round
    // would drop us below 0 ammo, you can scale down the last shot.
    // You could also implement a TrySpend that aborts for insufficient ammo.
    public int Spend(AmmunitionType type, int amount)
    {
        AmmoEntry held = _inventory[(int)type];
        int spend = Mathf.Min(amount, held.stockCurrent);
        held.stockCurrent -= spend;
        _inventory[(int)type] = held;
        return spend;
    }

    // Ensure our inventory list always matches the enum in the event of code changes.
    // You could also use a custom editor to maintain this more efficiently.
#if UNITY_EDITOR
    void Reset() { OnValidate(); }
    void OnValidate()
    {
        var ammoNames = System.Enum.GetNames(typeof(AmmunitionType));
        var inventory = new List<AmmoEntry>(ammoNames.Length);
        for (int i = 0; i < ammoNames.Length; i++)
        {
            
            if (ammoNames[i] == "Infinite")
            {
                print(ammoNames[i]);
            }

            var existing = _inventory.Find((entry) => { return entry.refName == ammoNames[i]; });
            existing.refName = ammoNames[i];
            existing.stockCurrent = Mathf.Min(existing.stockCurrent, existing.stockMax);
            inventory.Add(existing);
        }
        _inventory = inventory;
    }
#endif
}
*/

//public class AmmoInventory : MonoBehaviour
