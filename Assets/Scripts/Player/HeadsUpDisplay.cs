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

    [HideInInspector] public PlayerHandler ph;


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

    public GameObject ammoDisplay;
    public Text ammoCounter;
    public Text firingMode;

    [Header("Camera")]
    public Camera playerCamera;
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

    private void Awake()
    {
        ph = GetComponent<PlayerHandler>();
    }

    // Use this for initialization
    void Start()
    {
        
        

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

        RangedWeapon rw = ph.wh.equippedGun;

        float a = ph.wh.accuracyModifier.NewFloat(ph.wh.standingAccuracy);
        float rp = (a + rw.accuracy.projectileSpread) * Screen.height / playerCamera.fieldOfView;
        reticleUp.rectTransform.anchoredPosition = Vector3.up * rp;
        reticleDown.rectTransform.anchoredPosition = Vector3.down * rp;
        reticleLeft.rectTransform.anchoredPosition = Vector3.left * rp;
        reticleRight.rectTransform.anchoredPosition = Vector3.right * rp;

        firingMode.text = rw.firingModes[rw.firingModeIndex].name;

        if (rw.ammunition == null)
        {
            if (rw.magazine == null)
            {
                ammoCounter.text = "INFINITE";
            }
            else
            {
                ammoCounter.text = rw.magazine.magazine.current + "/INF";
            }
        }
        else
        {
            if (rw.magazine == null)
            {
                ammoCounter.text = ph.a.GetStock(rw.ammunition.ammoType).ToString();
            }
            else
            {
                ammoCounter.text = rw.magazine.magazine.current + "/" + (ph.a.GetStock(rw.ammunition.ammoType) - rw.magazine.magazine.current);
            }
        }



        /*
        WeaponType equippedType = ph.wh.equippedWeapon.type; // Finds type of equipped weapon
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
        */

    }

    void HealthHUD()
    {
        healthCounter.text = ph.h.health.current + "/" + ph.h.health.max;
        if (ph.h.health.IsCritical())
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
        ProjectileWeapon epw = ph.wh.equippedWeapon.GetComponent<ProjectileWeapon>();
        
        //reticlePositions = new Vector2(epw.projectileSpread * Screen.height / camera.fieldOfView, epw.projectileSpread * Screen.height / camera.fieldOfView);
        float a = ph.wh.accuracyModifier.NewFloat(ph.wh.standingAccuracy);
        
        float rp = (a + epw.projectileSpread) * Screen.height / playerCamera.fieldOfView;
        reticleUp.rectTransform.anchoredPosition = Vector3.up * rp;
        reticleDown.rectTransform.anchoredPosition = Vector3.down * rp;
        reticleLeft.rectTransform.anchoredPosition = Vector3.left * rp;
        reticleRight.rectTransform.anchoredPosition = Vector3.right * rp;

        /*
        float rp = (a + epw.projectileSpread) * Screen.height / playerCamera.fieldOfView;
        reticleUp.rectTransform.anchoredPosition = reticle.transform.up * rp;
        reticleDown.rectTransform.anchoredPosition = -reticle.transform.up * rp;
        reticleLeft.rectTransform.anchoredPosition = -reticle.transform.right * rp;
        reticleRight.rectTransform.anchoredPosition = reticle.transform.right * rp;
        */

        if (epw.caliber == AmmunitionType.None)
        {
            if (epw.magazineCapacity <= 0)
            {
                ammoCounter.text = "INFINITE";
            }
            else
            {
                ammoCounter.text = epw.roundsInMagazine + "/INF";
            }
        }
        else
        {
            if (epw.magazineCapacity <= 0)
            {
                ammoCounter.text = ph.a.GetStock(epw.caliber).ToString();
            }
            else
            {
                ammoCounter.text = epw.roundsInMagazine + "/" + (ph.a.GetStock(epw.caliber) - epw.roundsInMagazine);
            }
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