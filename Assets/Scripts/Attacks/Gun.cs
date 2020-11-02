using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class FiringModeData
{
    [Header("General")]
    public string name;
    public float switchSpeed;

    [Header("Ammunition")]
    public bool consumesAmmo = true;
    public AmmunitionType ammoType;
    public int ammoPerShot = 1;

    [Header("Fire rate")]
    public float roundsPerMinute = 900;
    public int maxBurst;
    [HideInInspector] public float fireTimer;
    [HideInInspector] public float burstCounter;

    [Header("Accuracy")]
    [Range(0, 180)] public float projectileSpread;
    public float range = 400;
    public LayerMask rayDetection = ~0;

    [Header("Recoil - Spread")]
    public float maxSpreadMultiplier = 2;
    public int shotsToReachMaxSpread = 5;
    public AnimationCurve spreadCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public float spreadApplyTimePerShot = 0.25f;
    public float spreadRecoveryTimePerShot = 0.5f;
    [HideInInspector] public float spreadTimer = 0;
    [HideInInspector] public float spreadToApply = 0;

    public float CalculatedSpread(float initialAccuracy)
    {
        return Mathf.Lerp(initialAccuracy, initialAccuracy * maxSpreadMultiplier, spreadCurve.Evaluate(spreadTimer));
    }

    [Header("Recoil - Kick")]
    public float kickPerShot;
    public int shotsToReachMaxKick;
    public AnimationCurve kickCurve;
    public float kickApplyTimePerShot;
    public float kickRecoveryTimePerShot;
    [HideInInspector] public float kickTimer = 0;
    [HideInInspector] public float kickToApply = 0;

    [Header("Other stats")]
    public int projectileIndex;
    public bool feedsFromMagazine;
    public int magazineIndex;
    public bool hasOptics;
    public int opticsIndex;

    [Header("Cosmetic")]
    public UnityEvent effectsOnFire;
    public Sprite hudIcon;
}

[System.Serializable]
public class WeaponProjectileData
{
    public string name;
    [Header("Generic stats")]
    public Projectile prefab;
    public Vector3 muzzle;
    public int count;
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

    public void Shoot(Character origin, Vector3 aimOrigin, Vector3 forward, Vector3 up, float spread, float range)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 angles = new Vector3(Random.Range(-spread, spread), Random.Range(-spread, spread), Random.Range(-spread, spread));
            Vector3 processedDirection = Misc.AngledDirection(angles, forward, up);

            RaycastHit targetFound;
            if (Physics.Raycast(aimOrigin, processedDirection, out targetFound, range, hitDetection)) // To reduce the amount of superfluous variables, I re-used the 'target' Vector3 in the same function as it is now unneeded for its original purpose
            {
                processedDirection = targetFound.point;
            }
            else
            {
                processedDirection = aimOrigin + processedDirection.normalized * range;
            }

            Projectile p = NewProjectileClass(origin);

            if (Vector3.Angle(forward, processedDirection - muzzle) < 90) // Checks that the position 'processedDirection' is actually further away than the muzzle and that the bullets will not travel in the complete wrong direction
            {
                Object.Instantiate(p.gameObject, muzzle, Quaternion.LookRotation(processedDirection - muzzle, up));
            }
            else // Otherwise, the gun barrel is probably clipping into a wall. Directly spawn the projectiles at the appropriate hit points.
            {
                // Spawn the projectile directly at the location where it is supposed to hit, with the correct rotation, and activate its OnHit function.
                Object.Instantiate(p.gameObject, processedDirection, Quaternion.LookRotation(processedDirection - aimOrigin, up));
                p.OnHit(targetFound);
            }
        }
    }
    Projectile NewProjectileClass(Character origin)
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

    /*
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
    */
}

[System.Serializable]
public class WeaponMagazineData
{
    public string name;
    public Resource data;
    public int roundsReloadedPerCycle;
    public float reloadTime;
}

[System.Serializable]
public class WeaponOpticsData
{
    public float name;
    public float magnification;
    public float transitionTime;
    [Range(-1, 0)] public float moveSpeedReduction;
    public Transform playerSightline;
    public bool disableReticle;
}




public class Gun : MonoBehaviour
{
    [HideInInspector] public WeaponHandler playerHolding;

    [Header("Firing modes")]
    public FiringModeData[] firingModes;
    public WeaponProjectileData[] projectileTypes;
    public WeaponMagazineData[] magazineTypes;
    public WeaponOpticsData[] opticsTypes;
    FiringModeData mode;
    WeaponProjectileData projectile;
    WeaponMagazineData magazine;
    WeaponOpticsData optics;
    bool isSwitchingFiringMode;

    float reloadTimer;
    bool isReloading;

    bool isAiming;
    [HideInInspector] PercentageModifier sensitivityWhileAiming = new PercentageModifier { multiplicative = true };
    [HideInInspector] PercentageModifier speedWhileAiming = new PercentageModifier { };


    [Header("Switching weapon")]
    public float drawSpeed;
    public float holsterSpeed;
    bool isSwitchingWeapon;

    [Header("Attack messages")]
    public float attackMessageDelay;
    float attackMessageDelayTimer;

    [Header("Cosmetics")]
    Transform weaponModel;
    public Animator animationHandler;


    
    
    // Start is called before the first frame update
    void Start()
    {
        SwitchFiringMode(0);
    }

    // Update is called once per frame
    void Update()
    {
        attackMessageDelayTimer += Time.deltaTime / attackMessageDelay;

        // If player is active
        // Checks following criteria to determine if the player should be able to control their weapon:
        // If the player is not currently switching weapon or firing mode
        // If the player's weapon selector is not active
        if (playerHolding.ph.PlayerState() == GameState.Active && isSwitchingWeapon == false && isSwitchingFiringMode == false && isReloading == false && playerHolding.weaponSelector.MenuIsActive() == false)
        {
            mode.fireTimer += Time.deltaTime;

            #region Fire controls
            // If player is pressing fire button
            if (Input.GetButton("Fire"))
            {
                // If fireTimer has finished
                // If burst count has not exceeded the limit OR burst count is set to zero
                if (mode.fireTimer >= 60 / mode.roundsPerMinute && (mode.burstCounter < mode.maxBurst || mode.maxBurst <= 0))
                {
                    // If ammunition is available, or the firing mode does not consume any
                    if (mode.consumesAmmo == false || playerHolding.ph.a.GetStock(mode.ammoType) >= mode.ammoPerShot)
                    {
                        // If the magazine has ammunition and is not currently reloading, or the weapon does not use a magazine
                        if (magazine == null || (magazine.data.current >= mode.ammoPerShot && isReloading == false))
                        {
                            #region Calculate variables for other gun stats
                            // Reset fire timer for next shot and update burst counter to ensure burst limit is not exceeded
                            mode.fireTimer = 0;
                            mode.burstCounter += 1;

                            // Apply recoil
                            mode.spreadToApply += 1;
                            mode.kickToApply += 1;

                            // Consume ammo if supply is present
                            if (mode.consumesAmmo == true)
                            {
                                playerHolding.ph.a.Spend(mode.ammoType, mode.ammoPerShot);
                            }

                            // Deplete magazine if the weapon feeds from one
                            if (magazine != null)
                            {
                                magazine.data.current -= mode.ammoPerShot;
                            }

                            #endregion

                            #region Calculate shot

                            // Obtain initial direction to fire weapon in
                            Transform aimOrigin = playerHolding.ph.pc.head.transform;
                            Vector3 aimDirection = aimOrigin.forward;

                            // Creates a cone of fire based on the player's accuracy and current recoil spread
                            float accuracy = playerHolding.standingAccuracy.Calculate();
                            accuracy = mode.CalculatedSpread(accuracy);

                            // While aiming normally, calculate player accuacy/recoil the same way as projectile spread, but beforehand.
                            // While aiming down sights, substitute some kind of system where the player's actual aim wavers around a certain point.
                            if (optics == null || isAiming == false)
                            {
                                Vector3 angles = new Vector3(Random.Range(-accuracy, accuracy), Random.Range(-accuracy, accuracy), Random.Range(-accuracy, accuracy));
                                aimDirection = Misc.AngledDirection(angles, aimDirection, transform.up);
                            }

                            // Shoots projectiles
                            projectile.Shoot(playerHolding.ph, aimOrigin.position, aimDirection, aimOrigin.up, mode.projectileSpread, mode.range);

                            #endregion

                            #region Send attack message

                            if (attackMessageDelayTimer >= 1) // Sends attack message
                            {
                                AttackMessage am = AttackMessage.Ranged(playerHolding.ph, aimOrigin.position, aimOrigin.forward, mode.range, projectile.diameter, accuracy + mode.projectileSpread, projectile.velocity, projectile.hitDetection);
                                EventObserver.TransmitAttack(am);
                                attackMessageDelayTimer = 0;
                            }

                            #endregion

                            #region Cosmetic effects
                            // Play animation
                            animationHandler.SetTrigger("Fire " + mode.name);
                            // Trigger cosmetic effects
                            mode.effectsOnFire.Invoke();
                            #endregion
                        }
                    }
                }
            }
            else if (!Input.GetButton("Fire"))
            {
                mode.burstCounter = 0;
            }
            #endregion

            /*
            RecoilHandler(mode);
            ReloadHandler(magazine, mode);
            OpticsHandler(optics);
            */
        }
    }

    IEnumerator SwitchFiringMode(int index)
    {
        // Obtains old stats
        FiringModeData oldMode = mode;
        WeaponProjectileData oldProjectile = projectile;
        WeaponMagazineData oldMagazine = magazine;
        WeaponOpticsData oldOptics = optics;

        // Obtains new stats
        FiringModeData newMode = firingModes[index];
        WeaponProjectileData newProjectile = projectileTypes[newMode.projectileIndex];
        WeaponMagazineData newMagazine = null;
        if (newMode.feedsFromMagazine == true)
        {
            newMagazine = magazineTypes[newMode.magazineIndex];
        }
        WeaponOpticsData newOptics = null;
        if (newMode.hasOptics == true)
        {
            newOptics = opticsTypes[newMode.opticsIndex];
        }

        yield return new WaitForEndOfFrame();

        // Before switching, exit aiming animation, if the new firing mode has different optics to the old one
        if (oldOptics != newOptics)
        {
            // Cancel ADS. Wait while this is in progress
            //yield return new WaitWhile(() => )
        }

        mode = newMode;
        projectile = newProjectile;
        magazine = newMagazine;
        optics = newOptics;

        // If the new firing mode feeds from the same magazine but uses different ammunition (i.e. if the player is swapping out ammo)
        if (newMagazine == oldMagazine && mode.ammoType != oldMode.ammoType)
        {
            // Reload the weapon's magazine. It will be unrealistic if the player's magazine instantly switches from one ammo type to the other without reloading.
            // I'm doing this by automatically emptying the magazine. This won't remove any of the player's ammunition.
            magazine.data.current = 0;
        }
    }


    void RecoilHandler(FiringModeData mode)
    {
        #region Calculate spread

        // Calculate spread
        float s = Time.deltaTime / mode.shotsToReachMaxSpread;
        if (mode.spreadToApply > 0)
        {
            // If there is spread to apply, increase spreadTimer and reduce spreadToApply accordingly
            s /= mode.spreadApplyTimePerShot;
            mode.spreadTimer += s;
            mode.spreadToApply -= s;
        }
        else
        {
            // If not, just reduce spreadTimer and don's do anything to spreadToApply
            s = s / mode.spreadRecoveryTimePerShot;
            mode.spreadTimer -= s;
        }
        #endregion

        #region Calculate kick
        /*
        
        float k = Time.deltaTime / mode.shotsToReachMaxKick;

        if (kickToApply > 0)
        {
            float r = recoilToApply * recoilApplyRate * Time.deltaTime;

            r = mode.kickCurve.Evaluate(mode.kickTimer);






            Vector2 rd = new Vector2(Random.Range(-1f, 1f), 1);
            if (rd.magnitude > 1)
            {
                rd.Normalize();
            }
            playerHolding.ph.pc.LookAngle(r * rd); // Move player's look position based on recoil
            recoilToApply -= k;
        }
        else if (!Input.GetButton("Fire")) // Return recoil using recoil recovery float
        {
            //print("Recovering from recoil");
        }


        */
        #endregion
    }
    
    void ReloadHandler(WeaponMagazineData magazine, FiringModeData mode)
    {
        if (magazine != null)
        {
            int remainingAmmo = playerHolding.ph.a.GetStock(mode.ammoType) - magazine.data.current; // Checks how much spare ammunition the player has

            // Checks if the gun is being reloaded manually
            bool manualReload = Input.GetButtonDown("Reload") && magazine.data.current < magazine.data.max;
            // Checks if the magazine is not full enough to fire, and the previous shot has finished firing
            bool automaticReload = magazine.data.current < mode.ammoPerShot && mode.fireTimer >= 60 / mode.roundsPerMinute;

            if ((manualReload || automaticReload) && isReloading == false && remainingAmmo > 0)
            {
                StartCoroutine(ReloadSequence(magazine, mode));
            }
        }
    }

    IEnumerator ReloadSequence(WeaponMagazineData magazine, FiringModeData mode)
    {
        #region Set up loop and wait for firing cycle to end
        // Establish variables that need to be present throughout the loop
        isReloading = true;
        reloadTimer = 0;
        int remainingAmmo = playerHolding.ph.a.GetStock(mode.ammoType) - magazine.data.current;

        // Wait until firing cycle has ended, then begin reload sequence for real.
        yield return new WaitUntil(() => mode.fireTimer >= mode.roundsPerMinute / 60 || isReloading == false);
        // If the reload sequence is cancelled prematurely while in this stage, it will detect that isReloading is false and skip to the next step.
        // The next step would be the update loop, but since that requires isReloading to be true, it will skip that.
        #endregion

        #region Start reload sequence for real

        #endregion

        #region 'Update' loop
        // If the reload cycle has not been automatically cancelled, the magazine is not full and there is ammunition remaining, count up and add ammunition to the magazine
        // This is essentially a separate update loop for reloading
        while (isReloading == true && magazine.data.PercentageFull() < 1 && remainingAmmo > 0)
        {
            reloadTimer += Time.deltaTime / magazine.reloadTime;

            // If reload timer has maxed out, add ammunition to magazine. This is a while loop instead of an if, so if the game severely lags out it will 'reload' multiple times if appropriate
            while (reloadTimer >= 1)
            {
                reloadTimer -= 1;

                #region Add ammunition to magazine
                // Checks how much spare ammunition the player has
                remainingAmmo = playerHolding.ph.a.GetStock(mode.ammoType) - magazine.data.current;

                // Reload the appropriate amount per cycle, unless there is not enough, in which case load all remaining ammunition
                magazine.data.current += Mathf.Min(remainingAmmo, magazine.roundsReloadedPerCycle);

                // Ensure magazine is not overloaded
                magazine.data.current = Mathf.Clamp(magazine.data.current, 0, magazine.data.max);
                #endregion
            }

            yield return new WaitForEndOfFrame();
        }
        #endregion

        #region End reload sequence
        isReloading = false;
        #endregion
    }

    public void CancelReload()
    {
        isReloading = false;
    }

    void OpticsHandler(WeaponOpticsData optics)
    {
        if (optics != null)
        {

        }
    }



    bool isAnimating;

    /*
    IEnumerator MoveInWorldSpace(Transform transform, Vector3 position, Quaternion rotation, float time, AnimationCurve curve = null, bool overridesOtherAnimations = false)
    {
        // Add code so the animation can combine its data. Maybe in a different function?

        #region End coroutine prematurely or override others, if the animation is in progress
        if (isAnimating == true)
        {
            // If this coroutine has permission, end and override other animations
            if (overridesOtherAnimations == true)
            {
                isAnimating = false;
                yield return new WaitForEndOfFrame();
            }
            else
            {
                yield break;
            }
        }
        #endregion

        #region Setup
        isAnimating = true;
        Vector3 originalPosition = transform.position;
        Quaternion originalRotation = transform.rotation;
        float timer = 0;
        #endregion

        #region Loop
        while (timer < 1)
        {
            // End function prematurely if remotely disabled
            if (isAnimating == false)
            {
                yield break;
            }

            // Count up timer
            timer += Time.deltaTime / time;

            // Determine percentage value based on how far through the animation is
            float completed = timer;
            if (curve != null)
            {
                completed = curve.Evaluate(timer);
            }

            // Move transform
            transform.position = Vector3.LerpUnclamped(originalPosition, position, completed);
            transform.rotation = Quaternion.LerpUnclamped(originalRotation, rotation, completed);
            // Should I use slerps?

            // End frame
            yield return new WaitForEndOfFrame();
        }
        #endregion

        isAnimating = false;
    }

    IEnumerator MoveInLocalSpace(Transform transform, Vector3 position, Quaternion rotation, float time, AnimationCurve curve = null, bool overridesOtherAnimations = false)
    {
        // Add code so the animation can combine its data. Maybe in a different function?

        #region End coroutine prematurely or override others, if the animation is in progress
        if (isAnimating == true)
        {
            // If this coroutine has permission, end and override other animations
            if (overridesOtherAnimations == true)
            {
                isAnimating = false;
                yield return new WaitForEndOfFrame();
            }
            else
            {
                yield break;
            }
        }
        #endregion

        #region Setup
        isAnimating = true;
        Vector3 originalPosition = transform.localPosition;
        Quaternion originalRotation = transform.localRotation;
        float timer = 0;
        #endregion

        #region Loop
        while (timer < 1)
        {
            // End function prematurely if remotely disabled
            if (isAnimating == false)
            {
                yield break;
            }

            // Count up timer
            timer += Time.deltaTime / time;

            // Determine percentage value based on how far through the animation is
            float completed = timer;
            if (curve != null)
            {
                completed = curve.Evaluate(timer);
            }

            // Move transform
            transform.localPosition = Vector3.LerpUnclamped(originalPosition, position, completed);
            transform.localRotation = Quaternion.LerpUnclamped(originalRotation, rotation, completed);
            // Should I use slerps?

            // End frame
            yield return new WaitForEndOfFrame();
        }
        #endregion

        isAnimating = false;
    }

    */

    IEnumerator MoveWeaponModel(Vector3 newLocalPosition, Quaternion newLocalRotation, float time, AnimationCurve curve = null, bool interruptsOtherAnimations = false)
    {
        isAnimating = true;
        
        if (curve == null)
        {
            curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        }

        Vector3 oldPosition = weaponModel.localPosition;
        Quaternion oldRotation = weaponModel.localRotation;
        
        float timer = 0;
        while (timer < 1)
        {
            float percentage = curve.Evaluate(timer);

            weaponModel.localPosition = Vector3.Lerp(oldPosition, newLocalPosition, percentage);
            weaponModel.localRotation = Quaternion.Lerp(oldRotation, newLocalRotation, percentage);

            timer += Time.deltaTime / time;
            yield return new WaitForEndOfFrame();
        }


        isAnimating = false;
    }
}

[System.Serializable]
public class TransformData
{
    public Vector3 position;
    public Quaternion rotation;

    public TransformData(Vector3 _position, Quaternion _rotation)
    {
        position = _position;
        rotation = _rotation;
    }

    public TransformData(Transform transform)
    {
        position = transform.position;
        rotation = transform.rotation;
    }
}
