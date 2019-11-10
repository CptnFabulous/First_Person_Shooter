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
    public Image healthBar;
    public ColourTransitionEffect damageFlash;
    public AudioClip damageNoise;

    [Header("Reticle/Aiming")]
    public Image reticleCentre;
    public Image reticleUp;
    public Image reticleDown;
    public Image reticleLeft;
    public Image reticleRight;
    public Color reticleDefaultColour = Color.white;
    public Color allyColour = Color.green;
    public Color enemyColour = Color.red;

    public ColourTransitionEffect opticsOverlay;
    public ColourTransitionEffect opticsTransition;

    [Header("Weapon stats")]
    public GameObject ammoDisplay;
    public Text ammoCounter;
    public Text firingMode;
    public Image ammoBar;

    [Header("Weapon feedback")]
    public AudioClip damageFeedback;
    public AudioClip criticalFeedback;
    public AudioClip killFeedback;
    public ColourTransitionEffect damagePing;
    public float damagePingDuration;
    public ColourTransitionEffect criticalPing;
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


        #region Health HUD
        healthCounter.text = ph.ph.health.current + "/" + ph.ph.health.max;
        FillMeter(healthBar, ph.ph.health.current, ph.ph.health.max);
        //healthBar.fillAmount = (float)ph.ph.health.current / (float)ph.ph.health.max;
        if (ph.ph.health.IsCritical())
        {
            healthCounter.color = resourceCriticalColour;
            // Do other stuff for critical health e.g. greyscale screen, warnings
        }
        else
        {
            healthCounter.color = resourceNormalColour;
        }
        #endregion

        #region Basic reticle and interacting
        RaycastHit lookingAt;
        if (Physics.Raycast(ph.pc.head.transform.position, transform.forward, out lookingAt, lookRange, lookDetection))
        {
            Character c = null;
            DamageHitbox d = lookingAt.collider.GetComponent<DamageHitbox>();
            if (d != null)
            {
                c = Character.FromHitbox(d);
                if (c != null)
                {
                    switch (ph.faction.Affiliation(c.faction))
                    {
                        case FactionState.Allied:
                            ColourReticle(allyColour);
                            break;
                        case FactionState.Hostile:
                            ColourReticle(enemyColour);
                            break;
                        default:
                            ColourReticle(reticleDefaultColour);
                            break;
                    }
                }
                else
                {
                    ColourReticle(reticleDefaultColour);
                }
            }
            else
            {
                ColourReticle(reticleDefaultColour);
            }

            // Do interacting stuff
        }
        else
        {
            ColourReticle(reticleDefaultColour);
        }
        #endregion

        #region Weapon HUD
        RangedWeapon rw = ph.wh.CurrentWeapon();
        bool activeWeapon = rw.gameObject.activeSelf;
        reticleUp.gameObject.SetActive(activeWeapon);
        reticleDown.gameObject.SetActive(activeWeapon);
        reticleLeft.gameObject.SetActive(activeWeapon);
        reticleRight.gameObject.SetActive(activeWeapon);
        ammoDisplay.gameObject.SetActive(activeWeapon);
        opticsOverlay.gameObject.SetActive(activeWeapon);
        opticsTransition.gameObject.SetActive(activeWeapon);

        if (activeWeapon == true)
        {
            if (rw.optics == null)
            {
                ADSTransition(0, null);
            }

            float a = ph.wh.accuracyModifier.NewFloat(ph.wh.standingAccuracy);
            float rp = (a + rw.accuracy.projectileSpread) * Screen.height / ph.pc.fieldOfView;
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
                    ammoBar.fillAmount = 1;
                }
                else
                {
                    ammoCounter.text = rw.magazine.magazine.current + "/INF";
                    FillMeter(ammoBar, rw.magazine.magazine.current, rw.magazine.magazine.max);
                }
            }
            else
            {
                if (rw.magazine == null)
                {
                    ammoCounter.text = ph.a.GetStock(rw.ammunition.ammoType).ToString();
                    FillMeter(ammoBar, ph.a.GetStock(rw.ammunition.ammoType), ph.a.GetMax(rw.ammunition.ammoType));
                }
                else
                {
                    ammoCounter.text = rw.magazine.magazine.current + "/" + (ph.a.GetStock(rw.ammunition.ammoType) - rw.magazine.magazine.current);
                    FillMeter(ammoBar, rw.magazine.magazine.current, rw.magazine.magazine.max);
                }
            }
        }
        #endregion
    }

    void ColourReticle(Color c)
    {
        reticleCentre.color = c;
        reticleUp.color = c;
        reticleDown.color = c;
        reticleLeft.color = c;
        reticleRight.color = c;
    }

    void FillMeter(Image i, float current, float max)
    {
        i.fillAmount = current / max;
    }

    public void ADSTransition(float timer, Sprite opticsGraphic)
    {
        if (opticsGraphic != null)
        {
            opticsOverlay.graphic.sprite = opticsGraphic;
        }
        opticsOverlay.SetTo(timer);
        opticsTransition.SetTo(timer);

        if (timer > 0.5f)
        {
            reticleCentre.enabled = false;
            reticleUp.enabled = false;
            reticleDown.enabled = false;
            reticleLeft.enabled = false;
            reticleRight.enabled = false;
        }
        else
        {
            reticleCentre.enabled = true;
            reticleUp.enabled = true;
            reticleDown.enabled = true;
            reticleLeft.enabled = true;
            reticleRight.enabled = true;
        }
    }

    public void PlayHitMarker(bool isCritical)
    {
        if (isCritical)
        {
            AudioSource.PlayClipAtPoint(criticalFeedback, transform.position);
            criticalPing.Play();
            damagePing.Stop();
        }
        else
        {
            AudioSource.PlayClipAtPoint(damageFeedback, transform.position);
            damagePing.Play();
            criticalPing.Stop();
        }
    }

    public void PlayerDamageFeedback()
    {
        damageFlash.Play();

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