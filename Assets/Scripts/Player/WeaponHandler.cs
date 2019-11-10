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
    //public Weapon equippedWeapon;



    public RangedWeapon[] equippedWeapons;
    //public GameObject[] equippedObjects;
    public int weaponIndex;

    public RangedWeapon CurrentWeapon()
    {
        return equippedWeapons[weaponIndex];
    }

    private void Awake()
    {
        ph = GetComponent<PlayerHandler>();
    }

    

    // Start is called before the first frame update
    void Start()
    {
        SwitchWeapon(weaponIndex);
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetButtonDown("SelectWeapon"))
        {
            int i = weaponIndex + 1;
            if (i >= equippedWeapons.Length)
            {
                i = 0;
            }

            SwitchWeapon(i);
        }
        
        if (Input.GetAxis("Mouse ScrollWheel") != 0 && CurrentWeapon().gameObject.activeSelf == true && CurrentWeapon().firingModes.Length > 1)
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

            i += CurrentWeapon().firingModeIndex;

            if (i > CurrentWeapon().firingModes.Length - 1)
            {
                i = 0;
            }
            else if (i < 0)
            {
                i = CurrentWeapon().firingModes.Length - 1;
            }

            CurrentWeapon().SwitchWeaponMode(i);
        }
    }

    void SwitchWeapon(int index)
    {
        index = Mathf.Clamp(index, 0, equippedWeapons.Length - 1);

        for(int i = 0; i < equippedWeapons.Length; i++)
        {
            RangedWeapon rw = equippedWeapons[i];

            if (i == index)
            {
                rw.Draw();
                print(rw.name + " has been drawn.");
            }
            else if (equippedWeapons[i].gameObject.activeSelf == true)
            {
                rw.Holster();
                print(rw.name + " has been holstered.");
            }
        }
        weaponIndex = index;
    }

    private void LateUpdate()
    {
        accuracyModifier.CheckStatDuration();
    }
}
