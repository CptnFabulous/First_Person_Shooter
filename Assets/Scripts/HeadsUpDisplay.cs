using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeadsUpDisplay : MonoBehaviour
{
    [Header("References")]
    public Camera camera;
    public CanvasScaler hud;
    public Weapon weaponEquipped;
    
    int healthPrev;

    [Header("Weapon Reticle")]
    public Image reticleUp;
    public Image reticleDown;
    public Image reticleLeft;
    public Image reticleRight;

    Vector2 reticlePositions;

    [Header("Weapon Ammunition")]
    public Image weaponGraphic;
    public Text ammoCounter;




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
        reticlePositions = new Vector2(weaponEquipped.weaponSpread * Screen.height / camera.fieldOfView, weaponEquipped.weaponSpread * Screen.height / camera.fieldOfView);
        reticleUp.rectTransform.anchoredPosition = new Vector3(0, reticlePositions.y, 0);
        reticleDown.rectTransform.anchoredPosition = new Vector3(0, -reticlePositions.y, 0);
        reticleLeft.rectTransform.anchoredPosition = new Vector3(-reticlePositions.x, 0, 0);
        reticleRight.rectTransform.anchoredPosition = new Vector3(reticlePositions.x, 0, 0);
        */


        /*
        if (prs.healthCurrent != healthPrev)
        {
            //HealthMeterUpdate();
        }
        healthPrev = prs.healthCurrent;
        */
    }

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