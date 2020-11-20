using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(AmmunitionInventory))]
public class WeaponHandler : MonoBehaviour
{

    [HideInInspector] public PlayerHandler ph;

    [Header("Stats")]
    public bool toggleAim;


    public VariableValueFloat standingAccuracy;
    public PercentageModifier runModifier;
    public PercentageModifier crouchModifier;

    [Header("Inventory")]
    public Gun[] equippedWeapons;
    public int currentWeaponIndex;
    int previousWeaponIndex;
    bool isSwitching = false;

    [Header("Switching")]
    public RadialMenu weaponSelector;

    #region Method variables
    public bool IsSwitchingWeapon
    {
        get { return isSwitching; }
    }

    public Gun CurrentWeapon()
    {
        if (equippedWeapons.Length < 1)
        {
            return null;
        }

        return equippedWeapons[currentWeaponIndex];
    }

    bool AllOtherWeaponsHolstered()
    {
        foreach (Gun rw in equippedWeapons)
        {
            if (rw.isSwitchingWeapon == true)
            {
                return false;
            }
        }
        return true;
    }
    #endregion

    private void Awake()
    {
        ph = GetComponent<PlayerHandler>();
        standingAccuracy.defaultValue = Mathf.Clamp(standingAccuracy.defaultValue, 0, 180);
    }

    // Start is called before the first frame update
    void Start()
    {
        standingAccuracy.Add(runModifier, this);
        standingAccuracy.Add(crouchModifier, this);

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
                //print("Selection made");
                StartCoroutine(SwitchWeaponAndFiringMode(weaponIndex, firingModeIndex));
            }
        }
        #endregion


        crouchModifier.SetActiveFully(ph.pc.isCrouching);
        runModifier.SetIntensity(ph.pc.MoveDirection().magnitude);
    }

    public void RefreshWeapons(int index)
    {
        Debug.Log("Refreshing weapons");

        equippedWeapons = GetComponentsInChildren<Gun>();
        foreach (Gun rw in equippedWeapons)
        {
            //rw.playerHolding = this;
            rw.gameObject.SetActive(false);
        }

        RefreshWeaponSelector();

        StartCoroutine(SwitchWeapon(index));
        //StartCoroutine(SwitchWeaponAndFiringMode(index, equippedWeapons[index].firingModeIndex));
    }

    void RefreshWeaponSelector()
    {
        #region Determine number of segments on weapon wheel
        int numberOfOptions = 0;
        foreach (Gun rw in equippedWeapons)
        {
            numberOfOptions += rw.firingModes.Length;
        }
        #endregion

        #region Determine icons to put on weapon wheel
        Sprite[] icons = new Sprite[numberOfOptions];
        int iconIndex = 0;
        foreach (Gun rw in equippedWeapons)
        {
            foreach (GunFiringMode m in rw.firingModes)
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
        if (equippedWeapons.Length < 1)
        {
            yield break;
        }

        // Clamp indexes to ensure they always switch to a proper weapon and firing mode
        weaponIndex = Mathf.Clamp(weaponIndex, 0, equippedWeapons.Length - 1);
        firingModeIndex = Mathf.Clamp(firingModeIndex, 0, equippedWeapons[weaponIndex].firingModes.Length);

        // Check if the firing mode is actually different
        if (equippedWeapons[weaponIndex] != equippedWeapons[currentWeaponIndex])
        {
            isSwitching = true;

            for (int i = 0; i < equippedWeapons.Length; i++)
            {
                Gun rw = equippedWeapons[i];

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
            //print("Weapon switch finished");
        }

        if (equippedWeapons[weaponIndex].firingModeIndex != firingModeIndex)
        {
            StartCoroutine(equippedWeapons[weaponIndex].SwitchMode(firingModeIndex));
        }


    }

    IEnumerator SwitchWeapon(int index)
    {
        Debug.Log("Weapon switch routine started");

        if (equippedWeapons.Length < 1)
        {
            yield break;
        }

        isSwitching = true;
        index = Mathf.Clamp(index, 0, equippedWeapons.Length - 1);

        for (int i = 0; i < equippedWeapons.Length; i++)
        {
            Gun rw = equippedWeapons[i];

            if (equippedWeapons[i].gameObject.activeSelf == true)
            {
                StartCoroutine(rw.Holster());
                print(rw.name + " has been holstered.");
            }
        }

        yield return new WaitUntil(() => AllOtherWeaponsHolstered());

        StartCoroutine(equippedWeapons[index].Draw());


        currentWeaponIndex = index;

        yield return new WaitUntil(() => equippedWeapons[index].isSwitchingWeapon == false);

        isSwitching = false;
    }

    void Equip(Gun rw, bool autoSwitch = true)
    {
        // Disable physics functions
        Collider c = rw.GetComponent<Collider>();
        c.enabled = false;
        Rigidbody rb = rw.GetComponent<Rigidbody>();
        rb.isKinematic = true;

        // Parent weapon to player
        rw.transform.SetParent(ph.pc.torso, true);
        rw.playerHolding = this;

        // Set position
        Transform t = rw.firingModes[rw.firingModeIndex].general.heldPosition;
        rw.transform.SetPositionAndRotation(t.position, t.rotation);

        // Enable gun script and disable gun
        rw.enabled = true;
        rw.gameObject.SetActive(false);

        //playerHolding.

        if (autoSwitch == true)
        {
            // Auto switch to new weapon
            //Gun g = equippedWeapo
        }


    }

    void Drop(int index)
    {
        float dropForce = 5;
        
        Gun droppedWeapon = equippedWeapons[index];

        // Disable and disconnect weapon from player
        droppedWeapon.enabled = false;
        droppedWeapon.playerHolding = null;
        droppedWeapon.transform.SetParent(null, true);

        // Enable physics
        Collider c = droppedWeapon.GetComponent<Collider>();
        c.enabled = true;
        Rigidbody rb = droppedWeapon.GetComponent<Rigidbody>();
        rb.isKinematic = false;

        // Toss away
        droppedWeapon.gameObject.SetActive(true);
        rb.AddForce(ph.pc.head.forward * dropForce);

        RefreshWeapons(0);
    }

}