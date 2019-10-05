using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (PlayerController))]
[RequireComponent(typeof (AmmunitionInventory))]
public class WeaponHandler : MonoBehaviour
{
    
    [HideInInspector] public PlayerHandler ph;

    [Header("Stats")]
    public bool toggleAim;
    [Range(0, 180)]
    public float standingAccuracy;
    public StatModifier accuracyModifier = new StatModifier();
    public float runMultiplier;
    public float crouchMultiplier;
    float modifiedAccuracy;
    

    [Header("Inventory")]
    public Weapon equippedWeapon;


    public RangedWeapon equippedGun;

    private void Awake()
    {
        ph = GetComponent<PlayerHandler>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (pc.isCrouching)
        {
            accuracyModifier.ApplyEffect("Crouching", crouchMultiplier, Time.deltaTime);
        }
        */



        if (Input.GetButton("SelectWeapon")) // If weapon wheel button is held
        {

        }
        else if (Input.GetButtonUp("SelectWeapon")) // When weapon wheel button is released
        {

        }
    }

    void SwitchWeapon( WeaponData wd)
    {

    }

    private void LateUpdate()
    {
        accuracyModifier.CheckStatDuration();
    }
}
