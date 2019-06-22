using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerResources : MonoBehaviour
{
    [Header("Standard resources")]
    public int healthMax;
    public int healthCurrent;
    public int fuelMax;
    public int fuelCurrent;

    [System.Serializable] // This makes the struct visible in the Inspector.
    public struct AmmunitionInventory
    {
        /*
        // Include name field for editing convenience only - not needed in-game.
        #if UNITY_EDITOR
        [HideInInspector]
        #endif
        */
        public string name;
        public AmmunitionType correspondingAmmo;
        public int stockMax;
        public int stockCurrent;
    }

    public List<AmmunitionInventory> ammoInventory;
    //List<AmmunitionInventory> ammoInv = new List<AmmunitionInventory>();


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


            /*
            ItemIcon item = current.Item.GetComponent<ItemIcon>(); // Try getting the ItemIcon component
            if (item != null)
            {
                current.icon.GetComponent<Image>().sprite = current.Item.GetComponent<ItemIcon>().hotbarIcon;
            }
            else
            {
                current.icon.GetComponent<Image>().sprite = EmptySlotIcon;
            }
            */
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
}
