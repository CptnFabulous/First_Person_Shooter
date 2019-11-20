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
    public RangedWeapon[] equippedWeapons;
    public int currentWeaponIndex;
    bool isSwitching = false;

    /*
    public bool IsSwitchingWeapon()
    {
        return isSwitching;
    }
    */

    public bool IsSwitchingWeapon
    {
        get { return isSwitching; }
    }
    

    [Header("Switching")]
    public RadialMenu weaponSelector;

    public RangedWeapon CurrentWeapon()
    {
        return equippedWeapons[currentWeaponIndex];
    }

    private void Awake()
    {
        ph = GetComponent<PlayerHandler>();
    }

    

    // Start is called before the first frame update
    void Start()
    {
        RefreshWeapons(currentWeaponIndex);
    }

    // Update is called once per frame
    void Update()
    {
        #region Weapon selector
        weaponSelector.WheelHandler();

        ph.pc.canLook = !weaponSelector.MenuIsActive();

        if (weaponSelector.MenuIsActive())
        {
            int wheelIndex = weaponSelector.ReturnIndex();

            int weaponIndex = 0;
            int firingModeIndex = 0;

            for (int w = 0; w < equippedWeapons.Length; w++)
            {
                for (int m = 0; m < equippedWeapons[w].firingModes.Length; m++)
                {
                    if (wheelIndex == 0)
                    {
                        weaponIndex = w;
                        firingModeIndex = m;
                    }

                    wheelIndex -= 1;
                }
            }

            ph.hud.PopulateWeaponWheel(equippedWeapons[weaponIndex], firingModeIndex);

            if (weaponSelector.SelectionMade()) // If player has made a selection and exited the weapon wheel
            {
                print("Selection made");
                StartCoroutine(SwitchWeaponAndFiringMode(weaponIndex, firingModeIndex));
            }
        }
        #endregion
    }

    private void LateUpdate()
    {
        accuracyModifier.CheckStatDuration();
    }

    void RefreshWeapons(int index)
    {
        equippedWeapons = GetComponentsInChildren<RangedWeapon>();
        foreach (RangedWeapon rw in equippedWeapons)
        {
            rw.playerHolding = this;
            rw.gameObject.SetActive(false);
        }

        RefreshWeaponSelector();

        StartCoroutine(SwitchWeapon(index));
    }

    void RefreshWeaponSelector()
    {
        #region Determine number of segments on weapon wheel
        int numberOfOptions = 0;
        foreach(RangedWeapon rw in equippedWeapons)
        {
            numberOfOptions += rw.firingModes.Length;
        }
        #endregion

        #region Determine icons to put on weapon wheel
        Sprite[] icons = new Sprite[numberOfOptions];
        int iconIndex = 0;
        foreach (RangedWeapon rw in equippedWeapons)
        {
            foreach(FiringMode m in rw.firingModes)
            {
                icons[iconIndex] = m.hudIcon;
                iconIndex++;
            }
        }
        #endregion

        weaponSelector.RefreshWheel(numberOfOptions, icons);
    }
    
    IEnumerator SwitchWeaponAndFiringMode(int weaponIndex, int firingModeIndex)
    {
        weaponIndex = Mathf.Clamp(weaponIndex, 0, equippedWeapons.Length - 1);
        firingModeIndex = Mathf.Clamp(firingModeIndex, 0, equippedWeapons[weaponIndex].firingModes.Length);

        if (equippedWeapons[weaponIndex] != equippedWeapons[currentWeaponIndex])
        {
            isSwitching = true;

            for (int i = 0; i < equippedWeapons.Length; i++)
            {
                RangedWeapon rw = equippedWeapons[i];

                if (equippedWeapons[i].gameObject.activeSelf == true)
                {
                    StartCoroutine(rw.Holster());
                }
            }

            yield return new WaitUntil(() => AllOtherWeaponsHolstered());

            StartCoroutine(equippedWeapons[weaponIndex].Draw());
            currentWeaponIndex = weaponIndex;

            yield return new WaitUntil(() => equippedWeapons[weaponIndex].isSwitchingWeapon == false);

            isSwitching = false;
            print("Weapon switch finished");
        }

        if (equippedWeapons[weaponIndex].firingModeIndex != firingModeIndex)
        {
            print("Index is different");
            equippedWeapons[weaponIndex].SwitchWeaponMode(firingModeIndex);
        }


    }

    IEnumerator SwitchWeapon(int index)
    {
        isSwitching = true;
        index = Mathf.Clamp(index, 0, equippedWeapons.Length - 1);

        for (int i = 0; i < equippedWeapons.Length; i++)
        {
            RangedWeapon rw = equippedWeapons[i];

            if (equippedWeapons[i].gameObject.activeSelf == true)
            {
                StartCoroutine(rw.Holster());
                print(rw.name + " has been holstered.");
            }
        }

        yield return new WaitUntil(() => AllOtherWeaponsHolstered());

        StartCoroutine(equippedWeapons[index].Draw());
        print(equippedWeapons[index].name + " has been drawn.");
        currentWeaponIndex = index;

        yield return new WaitUntil(() => equippedWeapons[index].isSwitchingWeapon == false);

        isSwitching = false;
    }
    
    bool AllOtherWeaponsHolstered()
    {
        foreach(RangedWeapon rw in equippedWeapons)
        {
            if (rw.isSwitchingWeapon == true)
            {
                return false;
            }
        }
        return true;
    }

}
