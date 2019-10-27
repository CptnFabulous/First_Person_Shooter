using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof (PlayerHandler))]
public class HeadsUpDisplay : MonoBehaviour
{
    [Header("References")]
    
    public CanvasScaler hud;

    [HideInInspector] public PlayerHandler ph;


    public Image weaponGraphic;

    [Header("General elements")]
    public Color resourceNormalColour;
    public Color resourceCriticalColour;

    public LayerMask lookDetection;
    public float lookRange;
    public float interactRange;

    [Header("Health elements")]
    public GameObject healthDisplay;
    public Text healthCounter;

    [Header("Reticle")]
    public Image reticleCentre;
    public Image reticleUp;
    public Image reticleDown;
    public Image reticleLeft;
    public Image reticleRight;
    public Color reticleDefaultColour = Color.white;
    public Color allyColour = Color.green;
    public Color enemyColour = Color.red;

    [Header("Weapon stats")]
    public GameObject ammoDisplay;
    public Text ammoCounter;
    public Text firingMode;

    [Header("Weapon feedback")]
    public AudioClip damageFeedback;
    public AudioClip criticalFeedback;
    public AudioClip killFeedback;
    public DisappearingEffect damagePing;
    public float damagePingDuration;
    public DisappearingEffect criticalPing;
    public float criticalPingDuration;



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
        //float rp = (a + rw.accuracy.projectileSpread) * Screen.height / playerCamera.fieldOfView;
        float rp = (a + rw.accuracy.projectileSpread) * Screen.height / ph.pc.fieldOfView;

        reticleUp.rectTransform.anchoredPosition = Vector3.up * rp;
        reticleDown.rectTransform.anchoredPosition = Vector3.down * rp;
        reticleLeft.rectTransform.anchoredPosition = Vector3.left * rp;
        reticleRight.rectTransform.anchoredPosition = Vector3.right * rp;

        RaycastHit lookingAt;
        if(Physics.Raycast(ph.pc.head.transform.position, transform.forward, out lookingAt, lookRange, lookDetection))
        {
            

            switch (lookingAt.collider.tag)
            {
                case "Enemy":
                    reticleCentre.color = enemyColour;
                    reticleUp.color = enemyColour;
                    reticleDown.color = enemyColour;
                    reticleLeft.color = enemyColour;
                    reticleRight.color = enemyColour;
                    break;
                case null:
                    reticleCentre.color = reticleDefaultColour;
                    reticleUp.color = reticleDefaultColour;
                    reticleDown.color = reticleDefaultColour;
                    reticleLeft.color = reticleDefaultColour;
                    reticleRight.color = reticleDefaultColour;
                    break;
                default:
                    reticleCentre.color = reticleDefaultColour;
                    reticleUp.color = reticleDefaultColour;
                    reticleDown.color = reticleDefaultColour;
                    reticleLeft.color = reticleDefaultColour;
                    reticleRight.color = reticleDefaultColour;
                    break;
            }
        }

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

    }

    public void PlayHitMarker(bool isCritical)
    {
        if (isCritical)
        {
            AudioSource.PlayClipAtPoint(criticalFeedback, transform.position);
            criticalPing.Restart(criticalPingDuration);
            damagePing.Stop();
        }
        else
        {
            AudioSource.PlayClipAtPoint(damageFeedback, transform.position);
            damagePing.Restart(damagePingDuration);
            criticalPing.Stop();
        }
        
    }

    void HealthHUD()
    {
        healthCounter.text = ph.ph.health.current + "/" + ph.ph.health.max;
        if (ph.ph.health.IsCritical())
        {
            healthCounter.color = resourceCriticalColour;
            // Do other stuff for critical health e.g. greyscale screen, warnings
        }
        else
        {
            healthCounter.color = resourceNormalColour;
        }
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