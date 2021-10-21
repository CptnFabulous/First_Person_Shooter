using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(AmmunitionInventory))]
public class WeaponHandler : MonoBehaviour
{

    [HideInInspector] public PlayerHandler handler;

    [Header("Stats")]
    public bool toggleAim;


    public VariableValueFloat standingAccuracy;
    public PercentageModifier runModifier;
    public PercentageModifier crouchModifier;

    [Header("Inventory")]
    public Gun[] equippedWeapons;
    public int currentWeaponIndex;
    int previousWeaponIndex;
    public bool IsSwitchingWeapon { get; private set; }
    public Transform defaultHoldingPosition;

    [Header("Switching")]
    public RadialMenu weaponSelector;
    public UnityEvent onHolsterWeapon;
    public UnityEvent onDrawWeapon;

    #region Method variables
    

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
        handler = GetComponent<PlayerHandler>();
        standingAccuracy.defaultValue = Mathf.Clamp(standingAccuracy.defaultValue, 0, 180);
    }

    // Start is called before the first frame update
    void Start()
    {
        standingAccuracy.Add(runModifier, this);
        standingAccuracy.Add(crouchModifier, this);

        GetHeldWeapons();

        weaponSelector.onValueChanged.AddListener(ShowWeaponDetails);
        weaponSelector.onValueConfirmed.AddListener(SelectWeaponAndFiringMode);

    }

    // Update is called once per frame
    void Update()
    {
        if (equippedWeapons.Length > 0)
        {
            weaponSelector.Control();
        }

        handler.movement.canLook = !weaponSelector.InSelection;
        crouchModifier.SetActiveFully(handler.movement.isCrouching);
        runModifier.SetIntensity(handler.movement.MoveDirection.magnitude);
    }

    public void GetHeldWeapons()
    {
        equippedWeapons = GetComponentsInChildren<Gun>();
        foreach (Gun rw in equippedWeapons)
        {
            //rw.playerHolding = this;
            rw.gameObject.SetActive(false);
        }

        RefreshWeaponSelector();

        /*
        if (equippedWeapons.Length > 0)
        {
            StartCoroutine(SwitchWeaponAndFiringMode(currentWeaponIndex, equippedWeapons[currentWeaponIndex].firingModeIndex));
        }
        */
        StartCoroutine(SwitchWeaponAndFiringMode(0, 0));
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

        weaponSelector.RefreshOptions(icons);
    }
    void GetIndexesFromSelector(int selectorIndex, out int weaponIndex, out int firingModeIndex)
    {
        weaponIndex = 0;
        firingModeIndex = 0;

        for (int w = 0; w < equippedWeapons.Length; w++)
        {
            for (int m = 0; m < equippedWeapons[w].firingModes.Length; m++)
            {
                if (selectorIndex == 0)
                {
                    weaponIndex = w;
                    firingModeIndex = m;
                }

                selectorIndex -= 1;
            }
        }
    }
    void ShowWeaponDetails(int index)
    {
        GetIndexesFromSelector(index, out int weaponIndex, out int firingModeIndex);
        handler.hud.PopulateWeaponWheel(equippedWeapons[weaponIndex], firingModeIndex);
    }
    void SelectWeaponAndFiringMode(int index)
    {
        GetIndexesFromSelector(index, out int weaponIndex, out int firingModeIndex);
        StartCoroutine(SwitchWeaponAndFiringMode(weaponIndex, firingModeIndex));
    }
    
    IEnumerator SwitchWeaponAndFiringMode(int weaponIndex, int firingModeIndex)
    {
        if (equippedWeapons.Length < 1)
        {
            yield break;
        }

        Debug.Log("Initiating switch");
        
        // Clamp indexes to ensure they always switch to a proper weapon and firing mode
        weaponIndex = Mathf.Clamp(weaponIndex, 0, equippedWeapons.Length - 1);
        firingModeIndex = Mathf.Clamp(firingModeIndex, 0, equippedWeapons[weaponIndex].firingModes.Length - 1);

        // If the new weapon is different from the old one, switch
        if (equippedWeapons[weaponIndex] != equippedWeapons[currentWeaponIndex] || equippedWeapons[weaponIndex].gameObject.activeSelf == false)
        {
            IsSwitchingWeapon = true;

            // Holster all currently active weapons (in case for some reason there are multiple active)
            for (int i = 0; i < equippedWeapons.Length; i++)
            {
                if (equippedWeapons[i].gameObject.activeSelf == true)
                {
                    StartCoroutine(equippedWeapons[i].Holster());
                }
            }
            onHolsterWeapon.Invoke();

            yield return new WaitUntil(() => AllOtherWeaponsHolstered());

            StartCoroutine(equippedWeapons[weaponIndex].Draw());
            currentWeaponIndex = weaponIndex;
            onDrawWeapon.Invoke();

            yield return new WaitUntil(() => equippedWeapons[weaponIndex].isSwitchingWeapon == false);

            IsSwitchingWeapon = false;
        }

        if (equippedWeapons[weaponIndex].firingModeIndex != firingModeIndex)
        {
            StartCoroutine(equippedWeapons[weaponIndex].SwitchMode(firingModeIndex));
        }
    }

    /*
    void Equip(Gun rw, bool autoSwitch = true)
    {
        // Disable physics functions
        Collider c = rw.GetComponent<Collider>();
        c.enabled = false;
        Rigidbody rb = rw.GetComponent<Rigidbody>();
        rb.isKinematic = true;

        // Parent weapon to player
        rw.transform.SetParent(handler.movement.torso, true);
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
        rb.AddForce(handler.movement.head.forward * dropForce);

        RefreshWeapons(0);
    }
    */
}