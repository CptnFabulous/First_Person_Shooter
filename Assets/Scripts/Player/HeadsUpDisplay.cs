using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

[RequireComponent(typeof (PlayerHandler))]
public class HeadsUpDisplay : MonoBehaviour
{
    [HideInInspector] public PlayerHandler ph;

    public Canvas hudCanvas;
    public Camera hudCamera;

    [Header("General elements")]
    public Color resourceNormalColour;
    public Color resourceCriticalColour;

    public LayerMask lookDetection;
    public float lookRange;
    public float interactRange;

    [Header("Objectives")]
    public Text objectiveList;

    [Header("Minimap")]
    public Camera minimapCamera;
    public float minCameraDistance;
    public float maxCameraDistance;
    public LayerMask terrainDetection = ~0;

    [Header("Interaction")]
    public RectTransform interactWindow;
    public Text interactObjectName;
    public Text interactPrompt;

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
    //public Text reserveCounter;
    public Text firingMode;
    public Image firingModeIcon;
    public Image ammoBar;

    [Header("Weapon feedback")]
    public AudioClip damageFeedback;
    public AudioClip criticalFeedback;
    public AudioClip killFeedback;
    public ColourTransitionEffect damagePing;
    public ColourTransitionEffect criticalPing;

    [Header("Weapon selector")]
    public Text selectorWeaponName;
    public Text selectorWeaponFiringMode;
    public Text selectorWeaponRemainingAmmunition;
    public Image selectorWeaponImage;

    private void Awake()
    {
        ph = GetComponent<PlayerHandler>();
    }

    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RectTransform rt = hudCanvas.GetComponent<RectTransform>();
        Transform playerHead = ph.pc.head.transform;

        #region Objectives
        objectiveList.text = ObjectiveList();
        #endregion

        #region Minimap

        float height = maxCameraDistance;
        RaycastHit spaceAbovePlayerCheck;
        if (Physics.Raycast(transform.position, transform.up, out spaceAbovePlayerCheck, maxCameraDistance, terrainDetection))
        {
            height = Vector3.Distance(transform.position, spaceAbovePlayerCheck.point);
            height = Mathf.Clamp(height, minCameraDistance, maxCameraDistance);
        }

        minimapCamera.transform.localPosition = new Vector3(0, height, 0);
        #endregion

        #region Interaction

        #endregion

        #region Health HUD
        healthCounter.text = ph.ph.health.current.ToString();
        FillMeter(healthBar, ph.ph.health.current, ph.ph.health.max);
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
        if (Physics.Raycast(playerHead.position, transform.forward, out lookingAt, lookRange, lookDetection))
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
        bool activeWeapon = !ph.wh.IsSwitchingWeapon;
        bool noIronsights = rw.optics == null || rw.optics.disableReticle == false;
        reticleCentre.gameObject.SetActive(!activeWeapon || noIronsights);
        reticleUp.gameObject.SetActive(activeWeapon && noIronsights);
        reticleDown.gameObject.SetActive(activeWeapon && noIronsights);
        reticleLeft.gameObject.SetActive(activeWeapon && noIronsights);
        reticleRight.gameObject.SetActive(activeWeapon && noIronsights);
        ammoDisplay.gameObject.SetActive(activeWeapon);
        opticsOverlay.gameObject.SetActive(activeWeapon);
        opticsTransition.gameObject.SetActive(activeWeapon);

        if (activeWeapon == true)
        {
            if (rw.optics == null)
            {
                ADSTransition(0, null);
            }

            if (noIronsights)
            {
                #region Calculate reticle width
                float spread = ph.wh.accuracyModifier.NewFloat(ph.wh.standingAccuracy) + rw.accuracy.projectileSpread; // Combines the player's accuracy stat with the spread of their current weapon


                Vector3 reticleOffsetPoint = Quaternion.AngleAxis(spread, playerHead.right) * playerHead.forward;
                reticleOffsetPoint = playerHead.position + reticleOffsetPoint * rw.accuracy.range;

                Debug.DrawLine(playerHead.position, reticleOffsetPoint, Color.blue);
                Debug.DrawLine(playerHead.position, playerHead.position + playerHead.forward * rw.accuracy.range, Color.red);

                reticleOffsetPoint = hudCamera.WorldToScreenPoint(reticleOffsetPoint); // Obtains the screen position of this point
                Vector2 canvasOffset = reticleCentre.rectTransform.rect.center;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, reticleOffsetPoint, hudCamera, out canvasOffset); // Converts screen point value to its appropriate location on the heads up display canvas
                float reticleRadius = Vector2.Distance(reticleCentre.rectTransform.rect.center, canvasOffset); // Obtains the width of the weapon's cone of fire at the maximum range, in canvas space


                //reticleOffsetPoint = hudCamera.WorldToViewportPoint(reticleOffsetPoint);
                //Vector2 canvasOffset = new Vector2(reticleOffsetPoint.x * hudCanvas.pixelRect.width, reticleOffsetPoint.y * hudCanvas.pixelRect.height);
                //float reticleRadius = Vector2.Distance(reticleCentre.rectTransform.rect.center, canvasOffset); // Obtains the width of the weapon's cone of fire at the maximum range, in canvas space

                // Adjust reticleRadius to match the canvas size
                reticleUp.rectTransform.anchoredPosition = rt.up * reticleRadius;
                reticleDown.rectTransform.anchoredPosition = rt.up * -reticleRadius;
                reticleLeft.rectTransform.anchoredPosition = rt.right * -reticleRadius;
                reticleRight.rectTransform.anchoredPosition = rt.right * reticleRadius;
                #endregion
            }


            firingMode.text = rw.firingModes[rw.firingModeIndex].name;
            firingModeIcon.sprite = rw.firingModes[rw.firingModeIndex].hudIcon;

            ammoCounter.text = AmmoInfo(rw, rw.firingModeIndex);

            AmmunitionStats a = rw.GetAmmunitionStats(rw.firingModeIndex);
            MagazineStats m = rw.GetMagazineStats(rw.firingModeIndex);

            if (a == null)
            {
                if (m == null)
                {
                    FillMeter(ammoBar, 1, 1);
                }
                else
                {
                    FillMeter(ammoBar, m.magazine.current, m.magazine.max);
                }
            }
            else
            {
                if (m == null)
                {
                    FillMeter(ammoBar, ph.a.GetStock(a.ammoType), ph.a.GetMax(a.ammoType));
                }
                else
                {
                    FillMeter(ammoBar, m.magazine.current, m.magazine.max);
                }
            }
        }
        #endregion
    }

    public void PopulateInteractionMenu(Interactable i)
    {
        if (interactWindow.gameObject.activeSelf == false)
        {
            interactWindow.gameObject.SetActive(true);
            interactObjectName.text = i.name;
            interactPrompt.text = i.instruction;
        }
        
    }

    string AmmoInfo(RangedWeapon rw, int firingModeIndex)
    {
        AmmunitionStats a = rw.GetAmmunitionStats(firingModeIndex);
        MagazineStats m = rw.GetMagazineStats(firingModeIndex);

        string s = "";

        if (a == null)
        {
            if (m == null)
            {
                s = "INFINITE";
            }
            else
            {
                s = m.magazine.current + "/INF";
            }
        }
        else
        {
            if (m == null)
            {
                s = ph.a.GetStock(a.ammoType).ToString();
            }
            else
            {
                s = m.magazine.current + "/" + (ph.a.GetStock(a.ammoType) - m.magazine.current);
            }
        }

        return s;
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

    public void PopulateWeaponWheel(RangedWeapon rw, int firingModeIndex)
    {
        selectorWeaponName.text = rw.name;
        selectorWeaponFiringMode.text = rw.firingModes[firingModeIndex].name;
        selectorWeaponRemainingAmmunition.text = AmmoInfo(rw, firingModeIndex);
        selectorWeaponImage.sprite = rw.weaponModel.GetComponent<SpriteRenderer>().sprite; // Substitute for finding object mesh if weapon has a 3D model
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

    string ObjectiveList()
    {
        string list = "Objectives:";

        ObjectiveHandler oh = FindObjectOfType<ObjectiveHandler>();
        if (oh != null)
        {
            bool activeObjectives = false; // Used to check if there are any active objectives to display

            foreach (PlayerObjective o in oh.objectives)
            {
                if (o.state == ObjectiveState.Active)
                {
                    activeObjectives = true;
                    list += "\n";

                    if (o.mandatory == false)
                    {
                        list += "OPTIONAL: ";
                    }

                    list += o.DisplayCriteria();
                }
            }

            if (activeObjectives == false) // If there are no active objectives, list an alternate message
            {
                list += "\n";
                list += "All completed";
            }
        }
        else
        {
            list = "No objectives";
        }

        return list;
    }

    /*
    public string CapitaliseText(string text)
    {
        string lowercase = "abcdefghijklmnopqrstuvwxyz";
        string uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        for (int i = 0; i < lowercase.Length; i++)
        {
            for (int l = 0; l < text.Length; l++)
            {
                if (text[l] == lowercase[i])
                {
                    //text[l] = uppercase[i];
                }
            }
            
        }

        

        return text;
    }
    */
}