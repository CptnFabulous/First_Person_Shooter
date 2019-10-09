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


        /*
        if (Input.GetButton("SelectWeapon")) // If weapon wheel button is held
        {

        }
        else if (Input.GetButtonUp("SelectWeapon")) // When weapon wheel button is released
        {

        }
        */

        /*
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            equippedGun.firingModeIndex += 1;
            if (equippedGun.firingModeIndex > equippedGun.firingModes.Length - 1)
            {
                equippedGun.firingModeIndex = 0;
            }
        }
        else if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            equippedGun.firingModeIndex -= 1;
            if (equippedGun.firingModeIndex < 0)
            {
                equippedGun.firingModeIndex = equippedGun.firingModes.Length - 1;
            }
        }
        */

        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            int i = 0;
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                i = 1;
            }
            else if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                i = -1;
            }

            i += equippedGun.firingModeIndex;

            if (i > equippedGun.firingModes.Length - 1)
            {
                i = 0;
            }
            else if (i < 0)
            {
                i = equippedGun.firingModes.Length - 1;
            }

            equippedGun.SwitchWeaponMode(i);
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
