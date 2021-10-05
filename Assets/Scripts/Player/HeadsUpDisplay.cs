
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;
using UnityEngine.Events;

public class HeadsUpDisplay : MonoBehaviour
{
    public PlayerHandler player;
    public Camera hudCamera;
    Canvas hudCanvas;
    RectTransform canvasTransform;

    [Header("General elements")]
    public Color resourceNormalColour;
    public Color resourceCriticalColour;
    
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
    public Sprite interactable;
    public Sprite denied;
    public Sprite inProgress;
    

    [Header("Health elements")]
    public GameObject healthDisplay;
    public Text healthCounter;
    public Image healthBar;
    public UnityEvent onDamage;
    public UnityEvent onHeal;

    [Header("Movement elements")]
    public UnityEvent effectsOnCrouch;
    public UnityEvent effectsOnStand;

    [Header("Reticle/Aiming")]
    public Image reticleCentre;
    public Image reticleUp;
    public Image reticleDown;
    public Image reticleLeft;
    public Image reticleRight;
    public Color reticleDefaultColour = Color.white;
    public Color allyColour = Color.green;
    public Color enemyColour = Color.red;
    public LayerMask lookDetection;
    public float lookRange = 100;

    [Header("Weapon stats")]
    public GameObject ammoDisplay;
    public Text ammoCounter;
    //public Text reserveCounter;
    public Text firingMode;
    public Image firingModeIcon;
    public Image ammoBar;

    [Header("Weapon feedback")]
    public UnityEvent damageEffect;
    public UnityEvent criticalEffect;
    public UnityEvent killEffect;

    [Header("Weapon optics")]
    public ColourTransitionEffect opticsOverlay;
    public ColourTransitionEffect opticsTransition;

    [Header("Weapon selector")]
    public Text selectorWeaponName;
    public Text selectorWeaponFiringMode;
    public Text selectorWeaponRemainingAmmunition;
    public Image selectorWeaponImage;

    private void Awake()
    {
        //player = GetComponent<PlayerHandler>();
        hudCanvas = GetComponent<Canvas>();
        canvasTransform = hudCanvas.GetComponent<RectTransform>();
    }

    // Use this for initialization
    void Start()
    {
        SetCrouchVisualEffects(player.movement.isCrouching);
        PopulateInteractionMenu(null);

        EventJunction.Subscribe(CheckToUpdateHealthMeter, true);
        EventJunction.Subscribe(CheckForHitMarker, true);
        EventJunction.Subscribe(CheckForKillMarker, true);
    }
    
    // Update is called once per frame
    void Update()
    {
        Transform playerHead = player.movement.head.transform;

        #region Objectives
        UpdateObjectiveList();
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


        #region Basic reticle and interacting
        RaycastHit lookingAt;
        if (Physics.Raycast(playerHead.position, transform.forward, out lookingAt, lookRange, lookDetection))
        {
            Character c = Character.FromObject(lookingAt.collider.gameObject);
            if (c != null)
            {
                if (player.HostileTowards(c))
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

        Gun rw = player.weapons.CurrentWeapon();
        if (rw != null)
        {
            GunGeneralStats ggs = rw.firingModes[rw.firingModeIndex].general;
            GunOpticsStats gos = rw.firingModes[rw.firingModeIndex].optics;
            
            
            bool activeWeapon = !player.weapons.IsSwitchingWeapon;
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
                    
                    float spread = player.weapons.standingAccuracy.Calculate() + ggs.projectileSpread; // Combines the player's accuracy stat with the spread of their current weapon

                    Vector3 reticleOffsetPoint = Quaternion.AngleAxis(spread, playerHead.right) * playerHead.forward;
                    reticleOffsetPoint = playerHead.position + reticleOffsetPoint * ggs.range;

                    //Debug.DrawLine(playerHead.position, reticleOffsetPoint, Color.blue);
                    //Debug.DrawLine(playerHead.position, playerHead.position + playerHead.forward * ggs.range, Color.red);

                    reticleOffsetPoint = hudCamera.WorldToScreenPoint(reticleOffsetPoint); // Obtains the screen position of this point
                    Vector2 canvasOffset = reticleCentre.rectTransform.rect.center;
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasTransform, reticleOffsetPoint, hudCamera, out canvasOffset); // Converts screen point value to its appropriate location on the heads up display canvas
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

                /*
                switch(ggs.consumesAmmo == false, m == null)
                {
                    case (true, true):
                        // Infinite ammo, meter is always full
                        ammoBar.fillAmount = 1;
                        break;
                    case (false, true):
                        float percentage
                        ammoBar.fillAmount = ph.a.ammoTypes[()]
                        break;
                    default:
                        ammoBar.fillAmount = m.data.PercentageFull;
                        break;
                }
                */
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
                        FillMeter(ammoBar, player.ammo.GetStock(ggs.ammoType), player.ammo.GetMax(ggs.ammoType));
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




    
    


    void FillMeter(Image i, float current, float max)
    {
        i.fillAmount = current / max;
    }
    public void PlaySoundEffect(AudioClip clip)
    {
        player.audio.PlayOneShot(clip);
    }

    public void SetCrouchVisualEffects(bool isCrouching)
    {
        if (isCrouching)
        {
            effectsOnCrouch.Invoke();
        }
        else
        {
            effectsOnStand.Invoke();
        }
    }
    public void PopulateInteractionMenu(Interactable i)
    {
        // If this is run with i set to null, this means there is nothing to interact with.
        if (i == null)
        {
            interactWindow.gameObject.SetActive(false);
            return;
        }

        interactWindow.gameObject.SetActive(true);

        interactObjectName.text = i.name;

        if (i.InProgress == true)
        {
            interactInstruction.text = i.inProgressMessage;
            interactButtonPrompt.sprite = inProgress;

        }
        else if (i.CanPlayerInteract(player) == false)
        {
            interactInstruction.text = i.deniedMessage;
            interactButtonPrompt.sprite = denied;
        }
        else
        {
            interactInstruction.text = i.instructionMessage;
            interactButtonPrompt.sprite = interactable;
        }
    }
    public void CheckToUpdateHealthMeter(DamageMessage damageData)
    {
        if (damageData.victim == player.health)
        {
            healthCounter.text = player.health.values.current.ToString();
            FillMeter(healthBar, player.health.values.current, player.health.values.max);
            if (player.health.values.IsCritical)
            {
                healthCounter.color = resourceCriticalColour;
                // Do other stuff for critical health e.g. greyscale screen, warnings
            }
            else
            {
                healthCounter.color = resourceNormalColour;
            }
        }
        
        
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
                s = player.ammo.GetStock(ggs.ammoType).ToString();
            }
            else
            {
                s = m.data.current + "/" + (player.ammo.GetStock(ggs.ammoType) - m.data.current);
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
    public void PopulateWeaponWheel(Gun rw, int firingModeIndex)
    {
        selectorWeaponName.text = rw.name;
        selectorWeaponFiringMode.text = rw.firingModes[firingModeIndex].name;
        selectorWeaponRemainingAmmunition.text = AmmoInfo(rw, firingModeIndex);
        selectorWeaponImage.sprite = rw.weaponSelectorIcon; // Possibly substitute for finding object mesh if weapon has a 3D model
    }
    public void CheckForHitMarker(DamageMessage message)
    {
        if (message.attacker == player)
        {
            damageEffect.Invoke();
        }
    }
    public void CheckForKillMarker(KillMessage message)
    {
        if (message.attacker == player)
        {
            killEffect.Invoke();
        }
    }

    void UpdateObjectiveList()
    {
        ObjectiveHandler oh = ObjectiveHandler.Current;
        if (oh != null)
        {
            string list = "";

            bool activeObjectives = false; // Used to check if there are any active objectives to display

            foreach (PlayerObjective o in oh.objectives)
            {
                if (o.state == ObjectiveState.Active)
                {
                    activeObjectives = true;

                    if (o.mandatory == false)
                    {
                        list += "OPTIONAL: ";
                    }

                    list += o.DisplayCriteria();

                    list += "\n";
                }
            }

            if (activeObjectives == false) // If there are no active objectives, list an alternate message
            {
                list += "All completed";
            }

            objectiveList.gameObject.SetActive(true);
            objectiveList.text = list;
        }
        else
        {
            objectiveList.gameObject.SetActive(false);
        }
    }




    
}