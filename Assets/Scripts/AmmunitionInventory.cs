﻿using System.Collections;
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
    Infinite
}

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
            /*
            var existing = _inventory.Find((entry) => { return entry.name == ammoNames[i]; });
            existing.name = ammoNames[i];
            */

            var existing = _inventory.Find((entry) => { return entry.refName == ammoNames[i]; });
            existing.refName = ammoNames[i];

            existing.stockCurrent = Mathf.Min(existing.stockCurrent, existing.stockMax);
            inventory.Add(existing);
        }
        _inventory = inventory;
    }
#endif






    /*
    // Represents an inventory slot containing some count of some item.
    [System.Serializable] // This makes the struct visible in the Inspector.
    public class Slot
    {

        //public ItemType type;
        //public int quantity;
        public GameObject Weapon;
        //public GameObject ItemObject;
    }
    

    
    // Exposes a list of slots in the Inspector.
    public List<Slot> slots;



    Slot SelectedSlot;
    Slot PrevSlot;

    int itemSlotValue;

    public Slot GetSlot(int index)
    {
        return slots[index];
    }

    void CheckSlots()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            Slot current = GetSlot(i); // Get each slot
            //Do thing you want to check the slot for


            
        }
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    */
}