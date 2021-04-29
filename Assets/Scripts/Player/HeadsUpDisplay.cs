
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerHandler))]
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
    public Text interactInstruction;
    public Image interactButtonPrompt;

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
            Character c = Character.FromObject(lookingAt.collider.gameObject);
            if (c != null)
            {
                if (ph.HostileTowards(c))
                {
                    ColourReticle(enemyColour);
                }
                else
                {
                    ColourReticle(allyColour);
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

        opticsOverlay.gameObject.SetActive(opticsOverlay.graphic.sprite != null);

        Gun rw = ph.wh.CurrentWeapon();
        if (rw != null)
        {
            GunGeneralStats ggs = rw.firingModes[rw.firingModeIndex].general;
            GunOpticsStats gos = rw.firingModes[rw.firingModeIndex].optics;
            
            
            bool activeWeapon = !ph.wh.IsSwitchingWeapon;
            bool noIronsights = gos == null || gos.disableReticle == false;
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
                if (gos == null)
                {
                    ADSTransition(0, null);
                }

                if (noIronsights)
                {
                    #region Calculate reticle width
                    
                    float spread = ph.wh.standingAccuracy.Calculate() + ggs.projectileSpread; // Combines the player's accuracy stat with the spread of their current weapon

                    Vector3 reticleOffsetPoint = Quaternion.AngleAxis(spread, playerHead.right) * playerHead.forward;
                    reticleOffsetPoint = playerHead.position + reticleOffsetPoint * ggs.range;

                    Debug.DrawLine(playerHead.position, reticleOffsetPoint, Color.blue);
                    Debug.DrawLine(playerHead.position, playerHead.position + playerHead.forward * ggs.range, Color.red);

                    reticleOffsetPoint = hudCamera.WorldToScreenPoint(reticleOffsetPoint); // Obtains the screen position of this point
                    Vector2 canvasOffset = reticleCentre.rectTransform.rect.center;
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, reticleOffsetPoint, hudCamera, out canvasOffset); // Converts screen point value to its appropriate location on the heads up display canvas
                    float reticleRadius = Vector2.Distance(reticleCentre.rectTransform.rect.center, canvasOffset); // Obtains the width of the weapon's cone of fire at the maximum range, in canvas space

                    // Adjust reticleRadius to match the canvas size
                    reticleUp.rectTransform.anchoredPosition = new Vector3(0, reticleRadius);
                    reticleDown.rectTransform.anchoredPosition = new Vector3(0, -reticleRadius);
                    reticleLeft.rectTransform.anchoredPosition = new Vector3(-reticleRadius, 0);
                    reticleRight.rectTransform.anchoredPosition = new Vector3(reticleRadius, 0);
                    #endregion
                }


                firingMode.text = rw.firingModes[rw.firingModeIndex].name;
                firingModeIcon.sprite = rw.firingModes[rw.firingModeIndex].hudIcon;

                ammoCounter.text = AmmoInfo(rw, rw.firingModeIndex);

                
                GunMagazineStats m = rw.firingModes[rw.firingModeIndex].magazine;

                if (ggs.consumesAmmo == false)
                {
                    if (m == null)
                    {
                        FillMeter(ammoBar, 1, 1);
                    }
                    else
                    {
                        FillMeter(ammoBar, m.data.current, m.data.max);
                    }
                }
                else
                {
                    if (m == null)
                    {
                        FillMeter(ammoBar, ph.a.GetStock(ggs.ammoType), ph.a.GetMax(ggs.ammoType));
                    }
                    else
                    {
                        FillMeter(ammoBar, m.data.current, m.data.max);
                    }
                }
            }
        }

        #endregion
    }

    public void PopulateInteractionMenu(Interactable i)
    {
        interactWindow.gameObject.SetActive(true);

        //interactButtonPrompt.sprite = 

        interactObjectName.text = i.name;

        if (i.InProgress == true)
        {
            //interactInstruction.text = i.Current().inProgressMessage;
            interactInstruction.text = i.inProgressMessage;
        }
        else
        {
            //interactInstruction.text = i.Current().instructionMessage;
            interactInstruction.text = i.instructionMessage;
        }

    }

    public void HideInteractionMenu()
    {
        interactWindow.gameObject.SetActive(false);
    }

    string AmmoInfo(Gun rw, int firingModeIndex)
    {
        GunGeneralStats ggs = rw.firingModes[firingModeIndex].general;
        GunMagazineStats m = rw.firingModes[firingModeIndex].magazine;

        string s = "";

        if (ggs.consumesAmmo == false)
        {
            if (m == null)
            {
                s = "INFINITE";
            }
            else
            {
                s = m.data.current + "/INF";
            }
        }
        else
        {
            if (m == null)
            {
                s = ph.a.GetStock(ggs.ammoType).ToString();
            }
            else
            {
                s = m.data.current + "/" + (ph.a.GetStock(ggs.ammoType) - m.data.current);
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

    public void PopulateWeaponWheel(Gun rw, int firingModeIndex)
    {
        selectorWeaponName.text = rw.name;
        selectorWeaponFiringMode.text = rw.firingModes[firingModeIndex].name;
        selectorWeaponRemainingAmmunition.text = AmmoInfo(rw, firingModeIndex);
        selectorWeaponImage.sprite = rw.weaponSelectorIcon; // Possibly substitute for finding object mesh if weapon has a 3D model
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
}


/*

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
            Character c = Character.FromObject(lookingAt.collider.gameObject);
            if (c != null)
            {
                if (ph.HostileTowards(c))
                {
                    ColourReticle(enemyColour);
                }
                else
                {
                    ColourReticle(allyColour);
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

        opticsOverlay.gameObject.SetActive(opticsOverlay.graphic.sprite != null);

        RangedWeapon rw = ph.wh.CurrentWeapon();
        if (rw != null)
        {
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
                    float spread = ph.wh.standingAccuracy.Calculate() + rw.accuracy.projectileSpread; // Combines the player's accuracy stat with the spread of their current weapon

                    Vector3 reticleOffsetPoint = Quaternion.AngleAxis(spread, playerHead.right) * playerHead.forward;
                    reticleOffsetPoint = playerHead.position + reticleOffsetPoint * rw.accuracy.range;

                    Debug.DrawLine(playerHead.position, reticleOffsetPoint, Color.blue);
                    Debug.DrawLine(playerHead.position, playerHead.position + playerHead.forward * rw.accuracy.range, Color.red);

                    reticleOffsetPoint = hudCamera.WorldToScreenPoint(reticleOffsetPoint); // Obtains the screen position of this point
                    Vector2 canvasOffset = reticleCentre.rectTransform.rect.center;
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, reticleOffsetPoint, hudCamera, out canvasOffset); // Converts screen point value to its appropriate location on the heads up display canvas
                    float reticleRadius = Vector2.Distance(reticleCentre.rectTransform.rect.center, canvasOffset); // Obtains the width of the weapon's cone of fire at the maximum range, in canvas space

                    // Adjust reticleRadius to match the canvas size
                    reticleUp.rectTransform.anchoredPosition = new Vector3(0, reticleRadius);
                    reticleDown.rectTransform.anchoredPosition = new Vector3(0, -reticleRadius);
                    reticleLeft.rectTransform.anchoredPosition = new Vector3(-reticleRadius, 0);
                    reticleRight.rectTransform.anchoredPosition = new Vector3(reticleRadius, 0);
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
                        FillMeter(ammoBar, m.data.current, m.data.max);
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
                        FillMeter(ammoBar, m.data.current, m.data.max);
                    }
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
                s = m.data.current + "/INF";
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
                s = m.data.current + "/" + (ph.a.GetStock(a.ammoType) - m.data.current);
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
}
*/