using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerHealth))]
[RequireComponent(typeof (WeaponHandler))]
public class HeadsUpDisplay : MonoBehaviour
{
    [Header("References")]
    
    public CanvasScaler hud;
    WeaponHandler weapons;
    PlayerHealth health;
    
    


    public Image weaponGraphic;

    [Header("Health elements")]
    public GameObject healthDisplay;
    public Text healthCounter;
    public Color normalColour;
    public Color criticalColour;

    [Header("Ranged Weapon elements")]
    public GameObject reticle;
    public Image reticleUp;
    public Image reticleDown;
    public Image reticleLeft;
    public Image reticleRight;
    Vector2 reticlePositions;

    public GameObject ammoDisplay;
    public Text ammoCounter;

    [Header("Camera")]
    public Camera camera;
    public float fieldOfView = 60;




    //[Header("Health Meter")]


    /*
    public RectTransform heartGridOrigin;
    public Vector2Int heartGridSize;
    public enum gridAxis
    {
        x,
        y
    }
    public gridAxis heartRowOrientation;
    public int heartNumber;
    public int heartSpacing;
    int heartRowLength;
    int heartRowNumber;
    */


    // Use this for initialization
    void Start()
    {
        
        health = GetComponent<PlayerHealth>();
        weapons = GetComponent<WeaponHandler>();

        /*
        prs = GetComponent<PlayerResources>();
        healthPrev = prs.healthCurrent;

        HealthMeterUpdate();
        */
    }

    // Update is called once per frame
    void Update()
    {

        /*
        if (prs.healthCurrent != healthPrev)
        {
            //HealthMeterUpdate();
        }
        healthPrev = prs.healthCurrent;
        */

        

        HealthHUD();

        WeaponType equippedType = weapons.equippedWeapon.type; // Finds type of equipped weapon
        switch(equippedType) // Checks type
        {
            case WeaponType.projectile:// If projectile weapon
                ProjectileHUD(); // Display appropriate HUD
                break;
            case WeaponType.melee: // If melee weapon
                MeleeHUD(); // Display appropriate HUD
                break;
            case WeaponType.throwable: // If throwable weapon
                ThrowableHUD(); // Display appropriate HUD
                break;
        }

    }

    void HealthHUD()
    {
        healthCounter.text = health.currentHealth + "/" + health.maxHealth;
        if (health.currentHealth <= health.maxHealth / 100 * health.criticalPercentage)
        {
            healthCounter.color = criticalColour;
            // Do other stuff for critical health e.g. greyscale screen, warnings
        }
        else
        {
            healthCounter.color = normalColour;
        }
    }

    #region Weapon HUDs
    void ProjectileHUD()
    {
        ProjectileWeapon epw = weapons.equippedWeapon.GetComponent<ProjectileWeapon>();

        //reticlePositions = new Vector2(epw.projectileSpread * Screen.height / camera.fieldOfView, epw.projectileSpread * Screen.height / camera.fieldOfView);
        float a = ModifyStat.NewFloat(weapons.standingAccuracy, weapons.accuracyModifier);
        reticlePositions = new Vector2((a + epw.projectileSpread) * Screen.height / camera.fieldOfView, (a + epw.projectileSpread) * Screen.height / camera.fieldOfView);
        reticleUp.rectTransform.anchoredPosition = new Vector3(0, reticlePositions.y, 0);
        reticleDown.rectTransform.anchoredPosition = new Vector3(0, -reticlePositions.y, 0);
        reticleLeft.rectTransform.anchoredPosition = new Vector3(-reticlePositions.x, 0, 0);
        reticleRight.rectTransform.anchoredPosition = new Vector3(reticlePositions.x, 0, 0);


        if (epw.ammoPerShot <= 0 && epw.magazineCapacity <= 0) // If infinite ammunition and does not require reloading
        {
            ammoCounter.text = "INFINITE";
        }
        else if (epw.ammoPerShot <= 0) // If empties magazine but can be reloaded infinitely
        {
            ammoCounter.text = epw.roundsInMagazine + "/INF";
        }
        else if (epw.magazineCapacity <= 0) // If consumes ammunition but does not require reloading
        {
            ammoCounter.text = weapons.ammoSupply.GetStock(epw.caliber).ToString();
        }
        else // if normal weapon, i.e. consumes ammunition and must be reloaded
        {
            ammoCounter.text = epw.roundsInMagazine + "/" + (weapons.ammoSupply.GetStock(epw.caliber) - epw.roundsInMagazine);
        }


        
    }
    void MeleeHUD()
    {

    }
    void ThrowableHUD()
    {

    }
    #endregion

























    /*
    void HealthMeterUpdate()
    {

        if (heartRowOrientation == gridAxis.x)
        {
            heartRowLength = heartGridSize.x;
            heartRowNumber = heartGridSize.y;
        }
        else
        {
            heartRowLength = heartGridSize.y;
            heartRowNumber = heartGridSize.x;
        }

        int hn = 0;
        for (int hrn = 0; hrn < heartRowNumber; hrn++)
        {
            for (int hrl = 0; hrl < heartRowLength; hrl++)
            {
                if (hn < heartNumber)
                {
                    print("Row " + hrn + ", number " + hrl);
                    hn++;
                }
                else
                {
                    hrn = heartRowNumber;
                    hrl = heartRowLength;
                }
            }
        }
    }
    */
}