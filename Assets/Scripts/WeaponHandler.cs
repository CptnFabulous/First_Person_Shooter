using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (PlayerController))]
[RequireComponent(typeof (AmmunitionInventory))]
public class WeaponHandler : MonoBehaviour
{
    [HideInInspector]
    public PlayerController pc;
    [HideInInspector]
    public AmmunitionInventory ammoSupply;

    [Header("Stats")]
    [Range(0, 180)]
    public float standingAccuracy;
    public float runMultiplier = 3;
    public float crouchMultiplier = -1;
    float accuracyModifier;
    float modifiedAccuracy;
    

    [Header("Inventory")]
    public Weapon equippedWeapon;
    public Weapon[] slots;
    
    // Start is called before the first frame update
    void Start()
    {
        pc = GetComponent<PlayerController>();
        ammoSupply = GetComponent<AmmunitionInventory>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButton("SelectWeapon")) // If weapon wheel button is held
        {

        }
        else if (Input.GetButtonUp("SelectWeapon")) // When weapon wheel button is released
        {

        }
    }
}
