﻿using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;





#region Stat classes
[System.Serializable]
public class FireControlStats
{
    public string name;
    public float roundsPerMinute;
    public int maxBurst;
    [HideInInspector] public float fireTimer;
    [HideInInspector] public float burstCounter;
}

[System.Serializable]
public class AccuracyStats
{
    public string name;
    [Range(0, 180)] public float projectileSpread;
    public float range;
    public LayerMask rayDetection;
}

[System.Serializable]
public class RecoilStats
{
    public string name;
    public float recoil;
    public float recoilApplyRate;
    public float recoilRecovery;
}

[System.Serializable]
public class OpticsStats
{
    public string name;
    public float magnification;
    public float transitionTime;
    public float moveSpeedReductionPercentage;
    public Transform sightLine;
    public Transform aimPosition;
    public Sprite scopeGraphic;
    public bool disableReticle;
}

[System.Serializable]
public class AmmunitionStats
{
    public string name;
    public bool consumesAmmo;
    public AmmunitionType ammoType;
    public int ammoPerShot = 1;
}

[System.Serializable]
public class MagazineStats
{
    public string name;
    public Resource data;
    public int roundsReloaded;
    public float reloadTime;
}

[System.Serializable]
public class ProjectileStats
{
    public string name;
    [Header("Generic stats")]
    public Projectile prefab;
    public Transform muzzle;
    public int projectileCount;
    public float velocity;
    public float diameter;
    public float gravityMultiplier;
    public LayerMask hitDetection = ~0;

    [Header("General stats")]
    public int damage;
    public float knockback;

    [Header("Kinetic projectile")]
    public float criticalMultiplier;

    [Header("Explosion stats")]
    public float directHitMultiplier;
    public float blastRadius;
    public float explosionTime;
    public AnimationCurve damageFalloff;
    public AnimationCurve knockbackFalloff;


    public GameObject NewProjectile(Character origin)
    {
        GameObject launchedProjectile = prefab.gameObject;

        Projectile p = launchedProjectile.GetComponent<Projectile>();
        p.velocity = velocity;
        p.diameter = diameter; 
        p.gravityMultiplier = gravityMultiplier;
        p.hitDetection = hitDetection;
        p.origin = origin;

        KineticProjectile kp = launchedProjectile.GetComponent<KineticProjectile>();
        if (kp != null)
        {
            kp.damage = damage;
            kp.knockback = knockback;
            kp.criticalMultiplier = criticalMultiplier;
        }

        ExplosiveProjectile ep = launchedProjectile.GetComponent<ExplosiveProjectile>();
        if (ep != null)
        {
            ep.damage = damage;
            ep.directHitMultiplier = directHitMultiplier;
            ep.blastRadius = blastRadius;
            ep.explosionTime = explosionTime;
            ep.knockback = knockback;
            ep.damageFalloff = damageFalloff;
            ep.knockbackFalloff = knockbackFalloff;
        }

        return launchedProjectile;
    }

    public Projectile NewProjectileClass(Character origin)
    {
        Projectile p = prefab;
        p.velocity = velocity;
        p.diameter = diameter;
        p.gravityMultiplier = gravityMultiplier;
        p.hitDetection = hitDetection;
        p.origin = origin;

        KineticProjectile kp = p.GetComponent<KineticProjectile>();
        if (kp != null)
        {
            kp.damage = damage;
            kp.knockback = knockback;
            kp.criticalMultiplier = criticalMultiplier;
        }

        ExplosiveProjectile ep = p.GetComponent<ExplosiveProjectile>();
        if (ep != null)
        {
            ep.damage = damage;
            ep.directHitMultiplier = directHitMultiplier;
            ep.blastRadius = blastRadius;
            ep.explosionTime = explosionTime;
            ep.knockback = knockback;
            ep.damageFalloff = damageFalloff;
            ep.knockbackFalloff = knockbackFalloff;
        }

        return p;
    }
}

[System.Serializable]
public class CosmeticsStats
{
    public string name;
    public Transform heldPosition;
    public UnityEvent effectsOnFire;
}
#endregion

[System.Serializable]
public class FiringMode
{
    [Header("Stat profile references")]
    public string name;
    public int fireControlMode;
    public int accuracyMode;
    public int projectileMode;
    public int recoilMode;

    public bool hasOptics;
    public int opticsMode;

    public int ammunitionMode;
    public bool feedsFromMagazine;

    public int magazineMode;
    public int miscStatsMode;

    [Header("Other")]
    public Sprite hudIcon;
    public float switchSpeed;

    [Header("Cosmetics")]
    public Transform heldPosition;
    public AudioClip firingNoise;
    public MuzzleFlashEffect muzzleFlash;
    public float muzzleFlashRelativeDuration;
    public ParticleSystem shellEjection;
}


public class RangedWeapon : MonoBehaviour
{
    #region Stats
    [Header("Fire controls")]
    public FireControlStats[] fireControlModes;
    [Header("Accuracy stats")]
    public AccuracyStats[] accuracyModes;
    [Header("Recoil stats")]
    public RecoilStats[] recoilModes;
    [Header("Optics")]
    public OpticsStats[] opticsModes;
    [Header("Ammunition types")]
    public AmmunitionStats[] ammunitionModes;
    [Header("Magazines")]
    public MagazineStats[] magazineModes;
    [Header("Projectiles")]
    public ProjectileStats[] projectileModes;
    [Header("Cosmetics")]
    public CosmeticsStats cosmeticsModes;

    [Header("Firing modes")]
    public FiringMode[] firingModes;

    [HideInInspector] public FireControlStats fireControls;
    [HideInInspector] public AccuracyStats accuracy;
    [HideInInspector] public ProjectileStats projectile;
    [HideInInspector] public OpticsStats optics;
    [HideInInspector] public RecoilStats recoil;
    [HideInInspector] public AmmunitionStats ammunition;
    [HideInInspector] public MagazineStats magazine;
    [HideInInspector] public CosmeticsStats cosmetics;
    #endregion

    #region Additional variables for weapon function
    // Additional optics stats
    [HideInInspector] public bool isAiming;
    [HideInInspector] public float zoomVariable;
    [HideInInspector] public float zoomTimer;
    [HideInInspector] public PercentageModifier sensitivityWhileAiming;
    [HideInInspector] public PercentageModifier speedWhileAiming;

    // Additional recoil stats
    [HideInInspector] public float recoilToApply;
    [HideInInspector] public Vector2 aimOrigin;
    [HideInInspector] public Vector2 aimCurrent;

    // Additional magazine stats
    [HideInInspector] public bool isReloading;
    [HideInInspector] public float reloadTimer;
    #endregion

    #region Universal variables (variables that can be used multiple times for different firing modes)
    [Header("Universal variables")]
    public float switchSpeed;
    public Transform weaponModel;
    public AudioSource weaponSoundSource;
    public Transform holsterPosition;
    public int firingModeIndex;

    [HideInInspector] public WeaponHandler playerHolding;

    // Switching modes
    [HideInInspector] public bool isSwitchingWeapon;
    [HideInInspector] public bool isSwitchingFireMode;

    // Moving weapon model
    bool isAnimating = false;

    float attackMessageLimitTimer = float.MaxValue;
    float attackMessageLimitDelay = 1;
    #endregion

    private void OnValidate()
    {
        if (fireControlModes.Length <= 0)
        {
            fireControlModes = new FireControlStats[1];
        }

        if (accuracyModes.Length <= 0)
        {
            accuracyModes = new AccuracyStats[1];
        }

        if (projectileModes.Length <= 0)
        {
            projectileModes = new ProjectileStats[1];
        }

        if (firingModes.Length <= 0)
        {
            firingModes = new FiringMode[1];
        }

        foreach (FiringMode fm in firingModes)
        {
            fm.fireControlMode = Mathf.Clamp(fm.fireControlMode, 0, fireControlModes.Length - 1);
            fm.accuracyMode = Mathf.Clamp(fm.accuracyMode, 0, accuracyModes.Length - 1);
            fm.projectileMode = Mathf.Clamp(fm.projectileMode, 0, projectileModes.Length - 1);
            fm.recoilMode = Mathf.Clamp(fm.recoilMode, -1, recoilModes.Length - 1);
            fm.opticsMode = Mathf.Clamp(fm.opticsMode, -1, opticsModes.Length - 1);
            fm.ammunitionMode = Mathf.Clamp(fm.ammunitionMode, -1, ammunitionModes.Length - 1);
            fm.magazineMode = Mathf.Clamp(fm.magazineMode, -1, magazineModes.Length - 1);
        }

        firingModeIndex = Mathf.Clamp(firingModeIndex, 0, firingModes.Length - 1);
    }

    #region Get optional weapon stats
    public OpticsStats GetOpticsStats(int index)
    {
        if (firingModes[index].opticsMode <= -1 || opticsModes.Length <= 0)
        {
            return null;
        }
        return opticsModes[firingModes[index].opticsMode];
    }

    public RecoilStats GetRecoilStats(int index)
    {
        if (firingModes[index].recoilMode <= -1 || recoilModes.Length <= 0)
        {
            return null;
        }
        return recoilModes[firingModes[index].recoilMode];
    }

    public AmmunitionStats GetAmmunitionStats(int index)
    {
        if (firingModes[index].ammunitionMode <= -1 || ammunitionModes.Length <= 0)
        {
            return null;
        }
        return ammunitionModes[firingModes[index].ammunitionMode];
    }

    public MagazineStats GetMagazineStats(int index)
    {
        if (firingModes[index].magazineMode <= -1 || magazineModes.Length <= 0)
        {
            return null;
        }
        return magazineModes[firingModes[index].magazineMode];
    }
    #endregion


    private void Start()
    {
        //ResetWeaponMoveVariables();

        playerHolding = GetComponentInParent<WeaponHandler>();


        sensitivityWhileAiming = new PercentageModifier(0, true, false);
        playerHolding.ph.pc.sensitivityModifier.Add(sensitivityWhileAiming, this);
        speedWhileAiming = new PercentageModifier();
        playerHolding.ph.pc.movementSpeed.Add(speedWhileAiming, this);
    }
    
    void Update()
    {
        #region Makes easy references to the appropriate sets of stats so I don't have to type "xModes[firingModes[firingModeIndex].xMode]" every single bloody time I need to reference a set of stats
        fireControls = fireControlModes[firingModes[firingModeIndex].fireControlMode];
        accuracy = accuracyModes[firingModes[firingModeIndex].accuracyMode];
        projectile = projectileModes[firingModes[firingModeIndex].projectileMode];
        optics = GetOpticsStats(firingModeIndex);
        recoil = GetRecoilStats(firingModeIndex);
        ammunition = GetAmmunitionStats(firingModeIndex);
        magazine = GetMagazineStats(firingModeIndex);
        #endregion

        // Checks following criteria to determine if the player should be able to control their weapon:
        // If the player is not currently switching weapon or firing mode
        // If the player's weapon selector is not active

        attackMessageLimitTimer += Time.deltaTime;


        if (isSwitchingWeapon == false && isSwitchingFireMode == false && playerHolding.weaponSelector.MenuIsActive() == false)
        {
            
            #region Firing mode controls
            if (Input.GetAxis("Mouse ScrollWheel") != 0 && firingModes.Length > 1) // Switch firing modes with the scroll wheel
            {
                int i = 0;
                if (Input.GetAxis("Mouse ScrollWheel") < 0)
                {
                    i = 1;
                }
                else if (Input.GetAxis("Mouse ScrollWheel") > 0)
                {
                    i = -1;
                }

                i += firingModeIndex;

                if (i > firingModes.Length - 1)
                {
                    i = 0;
                }
                else if (i < 0)
                {
                    i = firingModes.Length - 1;
                }

                //SwitchWeaponMode(i);
                StartCoroutine(SwitchMode(i));
            }
            #endregion

            fireControls.fireTimer += Time.deltaTime;

            // If player is active
            // If player is pressing fire button
            // If fireTimer has finished
            // If burst count has not exceeded the limit OR burst count is set to zero
            // If ammo is available OR supply is null
            // If magazine is not empty OR null
            if (playerHolding.ph.PlayerState() == GameState.Active && Input.GetButton("Fire") && fireControls.fireTimer >= 60 / fireControls.roundsPerMinute && (fireControls.burstCounter < fireControls.maxBurst || fireControls.maxBurst <= 0) && (ammunition == null || (playerHolding.ph.a.GetStock(ammunition.ammoType) >= ammunition.ammoPerShot)) && (magazine == null || (magazine.data.current >= 1/*ammoPerShot*/ && isReloading == false)))
            {
                // Adjust fire control variables
                fireControls.fireTimer = 0; // Reset fire timer to count up to next shot
                fireControls.burstCounter += 1;

                #region Alter ammunition, magazine and recoil variables if present
                // Consume ammo if supply is present
                if (ammunition != null)
                {
                    playerHolding.ph.a.Spend(ammunition.ammoType, ammunition.ammoPerShot);
                }

                // Deplete magazine if present
                if (magazine != null)
                {
                    if (ammunition != null) // If ammunition supply is present, consume appropriate amount of 
                    {
                        magazine.data.current -= ammunition.ammoPerShot;
                    }
                    else
                    {
                        magazine.data.current -= 1;
                    }
                }

                // Apply recoil if recoil stat exists
                if (recoil != null)
                {
                    recoilToApply += recoil.recoil;
                }
                #endregion

                #region Trigger cosmetic effects

                /*
                firingModes[firingModeIndex].muzzleFlash.Play();
                weaponSoundSource.PlayOneShot(firingModes[firingModeIndex].firingNoise);
                firingModes[firingModeIndex].shellEjection.Play();
                */

                MuzzleFlashEffect m = firingModes[firingModeIndex].muzzleFlash;
                if (m != null)
                {
                    m.Play();
                }

                AudioClip a = firingModes[firingModeIndex].firingNoise;
                if (a != null)
                {
                    weaponSoundSource.PlayOneShot(a);
                }

                ParticleSystem s = firingModes[firingModeIndex].shellEjection;
                if (s != null)
                {
                    s.Play();
                }
                #endregion

                #region Calculate accuracy
                // Calculate direction to shoot in
                Vector3 aimDirection = transform.forward;
                if (optics == null || isAiming == false)
                {
                    float accuracy = playerHolding.standingAccuracy.Calculate();
                    aimDirection = Quaternion.Euler(Random.Range(-accuracy, accuracy), Random.Range(-accuracy, accuracy), Random.Range(-accuracy, accuracy)) * aimDirection;
                }
                #endregion

                #region Send attack message
                if (attackMessageLimitTimer >= attackMessageLimitDelay) // Sends attack message
                {
                    AttackMessage am = AttackMessage.Ranged(playerHolding.ph, transform.position, transform.forward, accuracy.range, projectile.diameter, playerHolding.standingAccuracy.Calculate() + accuracy.projectileSpread, projectile.velocity, projectile.hitDetection);
                    EventObserver.TransmitAttack(am);
                    attackMessageLimitTimer = 0;
                }
                #endregion

                #region Shoot projectiles
                Damage.ShootProjectile(projectile, accuracy.projectileSpread, accuracy.range, playerHolding.ph, transform, projectile.muzzle.position, aimDirection);
                #endregion
            }
            else if (!Input.GetButton("Fire"))
            {
                fireControls.burstCounter = 0;
            }







            if (optics != null)
            {
                AimHandler(optics, firingModes[firingModeIndex].heldPosition, playerHolding.toggleAim);
            }

            if (magazine != null)
            {
                if (ammunition != null)
                {
                    ReloadHandler(magazine.reloadTime, fireControls.fireTimer, fireControls.roundsPerMinute, magazine.roundsReloaded, magazine.data, playerHolding, ammunition.ammoType);
                }
                else
                {
                    ReloadHandler(magazine.reloadTime, fireControls.fireTimer, fireControls.roundsPerMinute, magazine.roundsReloaded, magazine.data);
                }
            }
        }

        if (recoil != null)
        {
            RecoilHandler(recoil.recoilApplyRate, playerHolding);
        }
    }
    
    #region Switching functions
    public IEnumerator SwitchMode(int index)
    {
        if (index != firingModeIndex) // Checks if the firing mode is actually changing, otherwise code is not unnecessarily run
        {
            isSwitchingFireMode = true;

            // Check if the weapon being switched to has different optics, and cancel out if so.
            OpticsStats newOptics = GetOpticsStats(index);
            if (optics != null && (newOptics == null || newOptics != optics))
            {
                isAiming = false;
                zoomTimer = 0;
                LerpSights(optics, 0, firingModes[firingModeIndex].heldPosition);
            }

            if (newOptics != null)
            {
                sensitivityWhileAiming.percentageValue = 100 / newOptics.magnification;
                speedWhileAiming.percentageValue = newOptics.moveSpeedReductionPercentage;
                //Debug.Log("Sensitivity modifier = " + sensitivityWhileAiming.percentageValue + ", final sensitivity value = " + playerHolding.ph.pc.sensitivityModifier.Calculate());
            }

            if (magazine != null)
            {
                reloadTimer = 0;
                isReloading = false;
                print("Reload sequence cancelled");
            }

            yield return new WaitForSeconds(firingModes[firingModeIndex].switchSpeed);

            firingModeIndex = index;

            isSwitchingFireMode = false;
        }
    }
    
    public IEnumerator Draw()
    {
        isSwitchingWeapon = true;

        gameObject.SetActive(true);
        // Draw weapon animation
        weaponModel.transform.SetPositionAndRotation(holsterPosition.position, holsterPosition.rotation);

        Transform newMoveTransform = firingModes[firingModeIndex].heldPosition;
        StartCoroutine(MoveWeaponModel(newMoveTransform.localPosition, newMoveTransform.localRotation, switchSpeed));
        yield return new WaitForSeconds(switchSpeed);
        isSwitchingWeapon = false;
    }

    public IEnumerator Holster()
    {
        isSwitchingWeapon = true;

        #region Cancel running weapon functions
        if (optics != null) // DOES NOT WORK PROPERLY
        {
            isAiming = false;
            zoomTimer = 0;
            LerpSights(optics, 0, firingModes[firingModeIndex].heldPosition);
        }

        if (magazine != null)
        {
            CancelReload();
        }
        #endregion

        // Begin holster animation
        weaponModel.transform.SetPositionAndRotation(firingModes[firingModeIndex].heldPosition.position, firingModes[firingModeIndex].heldPosition.rotation);
        StartCoroutine(MoveWeaponModel(holsterPosition.localPosition, holsterPosition.localRotation, switchSpeed));

        yield return new WaitForSeconds(switchSpeed);

        gameObject.SetActive(false);
        isSwitchingWeapon = false;
    }
    #endregion




    IEnumerator MoveWeaponModel(Vector3 newLocalPosition, Quaternion newLocalRotation, float time, AnimationCurve curve = null, bool interruptsOtherAnimations = true)
    {
        #region Check for existing animations and cancel/override
        if (isAnimating == true)
        {
            if (interruptsOtherAnimations == false)
            {
                // Abort coroutine if another animation is already running
                yield break;
            }
            else
            {
                // Unless interruptsOtherAnimations is enabled, in which case it cancels the currently running animation.
                // It disables isAnimating, then other coroutines will notice and end prematurely.
                isAnimating = false;
                yield return new WaitForEndOfFrame();
            }
        }
        #endregion

        #region Setup
        isAnimating = true;
        if (curve == null)
        {
            curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        }
        Vector3 oldPosition = weaponModel.localPosition;
        Quaternion oldRotation = weaponModel.localRotation;
        float timer = 0;
        #endregion

        #region Loop
        while (timer < 1)
        {
            // End loop prematurely if isAnimating is remotely declared false
            if (isAnimating == false)
            {
                yield break;
            }

            float percentage = curve.Evaluate(timer);

            weaponModel.localPosition = Vector3.Lerp(oldPosition, newLocalPosition, percentage);
            weaponModel.localRotation = Quaternion.Lerp(oldRotation, newLocalRotation, percentage);

            timer += Time.deltaTime / time;
            yield return new WaitForEndOfFrame();
        }
        #endregion

        #region End
        weaponModel.localPosition = Vector3.Lerp(oldPosition, newLocalPosition, curve.Evaluate(1));
        weaponModel.localRotation = Quaternion.Lerp(oldRotation, newLocalRotation, curve.Evaluate(1));

        isAnimating = false;
        #endregion
    }

    #region Firing functions
    public void RecoilHandler(float recoilApplyRate, WeaponHandler playerHolding)
    {
        if (recoilToApply > 0)
        {
            float r = recoilToApply * recoilApplyRate * Time.deltaTime;
            Vector2 rd = new Vector2(Random.Range(-1f, 1f), 1);
            if (rd.magnitude > 1)
            {
                rd.Normalize();
            }
            playerHolding.ph.pc.LookAngle(r * rd); // Add recoil
            recoilToApply -= r;
        }
        else if (!Input.GetButton("Fire")) // Return recoil using recoil recovery float
        {
            //print("Recovering from recoil");
        }

        // Figure out how to have visual gun recoil
        // 1: Use lerping and an animation curve to move gun between standard and fully recoiled position
        // 2: Just give it a spring joint and use Physics.AddForce or AddForceAtPosition to make it jump back
    }
    
    public void ADSHandler()
    {
        if (playerHolding.toggleAim == true)
        {
            if (Input.GetButtonDown("MouseRight"))
            {
                isAiming = !isAiming;
            }
        }
        else
        {
            if (Input.GetButton("MouseRight"))
            {
                isAiming = true;
            }
            else
            {
                isAiming = false;
            }
        }


        Vector3 sightLine;
        Vector3 aimPosition;
        Vector3 holsterPosition;


        Vector3 sightlineDifference = Vector3.zero - optics.sightLine.localPosition;


        


    }


    public void AimHandler(OpticsStats os, Transform hipPosition, bool toggleAim)
    {
        if (toggleAim == true)
        {
            if (Input.GetButtonDown("MouseRight"))
            {
                isAiming = !isAiming;
            }
        }
        else
        {
            if (Input.GetButton("MouseRight"))
            {
                isAiming = true;
            }
            else
            {
                isAiming = false;
            }
        }

        if (isAiming) //Sets timer value to specify lerping of variables
        {
            zoomTimer += Time.deltaTime / os.transitionTime;
            LerpSights(os, zoomTimer, os.aimPosition);
        }
        else
        {
            zoomTimer -= Time.deltaTime / os.transitionTime;
            LerpSights(os, zoomTimer, hipPosition);
        }
        zoomTimer = Mathf.Clamp01(zoomTimer);
    }

    void LerpSights(OpticsStats os, float timer, Transform newWeaponPosition)
    {
        
        // Reduces FOV to zoom in camera
        zoomVariable = Mathf.Lerp(1, 1 / os.magnification, timer);
        playerHolding.ph.pc.playerCamera.fieldOfView = playerHolding.ph.pc.fieldOfView.defaultValue * zoomVariable;

        // Reduce sensitivity and movement speed
        sensitivityWhileAiming.SetIntensity(timer);
        speedWhileAiming.SetIntensity(timer);
        // Alter accuracy if specified
        
        // Move weapon model. THIS DOES NOT WORK FOR SOME REASON
        float moveTime = Vector3.Distance(weaponModel.transform.position, newWeaponPosition.position) * os.transitionTime;
        MoveWeaponModel(newWeaponPosition.position, newWeaponPosition.rotation, moveTime);

        // Toggle overlay
        playerHolding.ph.hud.ADSTransition(timer, optics.scopeGraphic);
    }

    #endregion

    #region Reloading functions
    public void ReloadHandler(float reloadTime, float fireTimer, float roundsPerMinute, int roundsReloaded, Resource magazine, WeaponHandler playerHolding, AmmunitionType caliber)
    {
        int remainingAmmo = playerHolding.ph.a.GetStock(caliber) - magazine.current; // Checks how much spare ammunition the player has

        // If reload button is pressed and weapon's magazine is not full OR if magazine is empty and gun is finished firing, PLUS if ammunition remains and the player is not already reloading
        if (((Input.GetButtonDown("Reload") && magazine.current < magazine.max) || (magazine.current <= 0 && fireTimer >= 60 / roundsPerMinute)) && isReloading == false && remainingAmmo > 0)
        {
            reloadTimer = 0;
            isReloading = true;
            print("Reload sequence started");
        }
        if (isReloading == true)
        {
            reloadTimer += Time.deltaTime / reloadTime;

            // If magazine is full, there is no more ammunition, or reload is interrupted by another action
            if (isReloading == true && (magazine.current >= magazine.max || remainingAmmo <= 0 || (Input.GetButtonDown("Fire") && magazine.current > 0))) // Also include button options for melee attacking and any other functions that would cancel out the reload function
            {
                CancelReload();
            }

            if (reloadTimer >= 1) // If reload time has been reached, reload ammunition into magazine
            {
                print("Ammo reloaded");
                if (remainingAmmo < roundsReloaded) // If there is not enough ammunition to reload the usual amount
                {
                    magazine.current += remainingAmmo; // Reload all remaining ammunition
                }
                else
                {
                    magazine.current += roundsReloaded; // Reload standard amount of ammunition per reload cycle
                }
                magazine.current = Mathf.Clamp(magazine.current, 0, magazine.max); // Ensure magazine is not overloaded

                // If magazine is full, there is no more ammunition, or reload is interrupted by another action
                if (magazine.current >= magazine.max || remainingAmmo <= 0) // Also include button options for melee attacking and any other functions that would cancel out the reload function
                {
                    CancelReload();
                }
                else
                {
                    reloadTimer = 0; // Reset reload timer
                    print("Reload sequence continued");
                }
            }
        }
    }

    public void ReloadHandler(float reloadTime, float fireTimer, float roundsPerMinute, int roundsReloaded, Resource magazine)
    {
        // If reload button is pressed and weapon's magazine is not full OR if magazine is empty and gun is finished firing, PLUS if ammunition remains and the player is not already reloading
        if (((Input.GetButtonDown("Reload") && magazine.current < magazine.max) || (magazine.current <= 0 && fireTimer >= 60 / roundsPerMinute)) && isReloading == false)
        {
            reloadTimer = 0;
            isReloading = true;
            print("Reload sequence started");
        }
        if (isReloading == true)
        {
            reloadTimer += Time.deltaTime / reloadTime;

            // If magazine is full, there is no more ammunition, or reload is interrupted by another action
            if (isReloading == true && (magazine.current >= magazine.max || (Input.GetButtonDown("Fire") && magazine.current > 0))) // Also include button options for melee attacking and any other functions that would cancel out the reload function
            {
                CancelReload();
            }

            if (reloadTimer >= 1) // If reload time has been reached, reload ammunition into magazine
            {
                print("Ammo reloaded");
                magazine.current += roundsReloaded; // Reload standard amount of ammunition per reload cycle
                magazine.current = Mathf.Clamp(magazine.current, 0, magazine.max); // Ensure magazine is not overloaded

                // If magazine is full, there is no more ammunition, or reload is interrupted by another action
                if (magazine.current >= magazine.max) // Also include button options for melee attacking and any other functions that would cancel out the reload function
                {
                    CancelReload();
                }
                else
                {
                    reloadTimer = 0; // Reset reload timer
                    print("Reload sequence continued");
                }
            }
        }
    }

    public void CancelReload()
    {
        reloadTimer = 0;
        isReloading = false;
        //print("Reload sequence ended");
    }



    /*
    IEnumerator ReloadSequence()
    {
        float timer = 0;
        
    }
    */




    #endregion
}
