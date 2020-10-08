using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[System.Serializable]
public class FiringModeData
{
    [Header("General")]
    public string name;
    

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
    public LayerMask rayDetection;

    [Header("Recoil - Spread")]
    public float maxSpreadMultiplier = 2;
    public int shotsToReachMaxSpread = 5;
    public AnimationCurve spreadCurve;
    public float spreadApplyTimePerShot = 0.25f;
    public float spreadRecoveryTimePerShot = 0.5f;
    [HideInInspector] public float spreadTimer = 0;
    [HideInInspector] public float spreadToApply = 0;

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
    public int magazineIndex;
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
    public Resource magazine;
    public int roundsReloaded;
    public float reloadTime;
}

[System.Serializable]
public class WeaponOpticsData
{
    public float name;
    public float magnification;
    public float transitionTime;
    [Range(-1, 0)] public float moveSpeedReduction;
    public Transform aimPosition;
    public Sprite scopeGraphic;
    public bool disableReticle;
}




public class NewRangedWeapon : MonoBehaviour
{
    public WeaponHandler playerHolding;
    
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

    [Header("Switching weapon")]
    public float drawSpeed;
    public float holsterSpeed;
    bool isSwitchingWeapon;

    [Header("Attack messages")]
    public float attackMessageDelay;
    float attackMessageDelayTimer;

    [Header("Cosmetics")]
    public Animator animationHandler;
    
    // Start is called before the first frame update
    void Start()
    {
        SwitchFiringMode(0);
    }

    // Update is called once per frame
    void Update()
    {
        // Checks following criteria to determine if the player should be able to control their weapon:
        // If the player is not currently switching weapon or firing mode
        // If the player's weapon selector is not active

        attackMessageDelayTimer += Time.deltaTime / attackMessageDelay;


        if (isSwitchingWeapon == false && isSwitchingFiringMode == false && playerHolding.weaponSelector.MenuIsActive() == false)
        {
            // Firing mode controls

            mode.fireTimer += Time.deltaTime;

            // If player is active
            // If player is pressing fire button
            if (playerHolding.ph.PlayerState() == GameState.Active && Input.GetButton("Fire"))
            {
                // If fireTimer has finished
                // If burst count has not exceeded the limit OR burst count is set to zero
                if (mode.fireTimer >= 60 / mode.roundsPerMinute && (mode.burstCounter < mode.maxBurst || mode.maxBurst <= 0))
                {
                    // If ammunition is available, or the firing mode does not consume any
                    if (mode.consumesAmmo == false || playerHolding.ph.a.GetStock(mode.ammoType) >= mode.ammoPerShot)
                    {
                        // If the magazine has ammunition and is not currently reloading, or the weapon does not use a magazine
                        if (magazine == null || (magazine.magazine.current >= 1/*ammoPerShot*/ && isReloading == false))
                        {
                            #region Fire weapon
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

                            if (magazine != null)
                            {
                                magazine.magazine.current -= mode.ammoPerShot;
                            }

                            // Obtain initial direction to fire weapon in
                            Transform aimOrigin = playerHolding.ph.pc.head.transform;
                            Vector3 aimDirection = aimOrigin.forward;

                            if (optics == null || isAiming == false)
                            {
                                float accuracy = playerHolding.standingAccuracy.Calculate();
                                Vector3 angles = new Vector3(Random.Range(-accuracy, accuracy), Random.Range(-accuracy, accuracy), Random.Range(-accuracy, accuracy));
                                Vector3 processedDirection = Misc.AngledDirection(angles, aimDirection, transform.up);
                            }

                            // Shoots projectiles. ACCOUNT FOR RECOIL SPREAD
                            projectile.Shoot(playerHolding.ph, aimOrigin.position, aimDirection, aimOrigin.up, mode.projectileSpread, mode.range);

                            if (attackMessageDelayTimer >= 1) // Sends attack message
                            {
                                AttackMessage am = AttackMessage.Ranged(playerHolding.ph, transform.position, transform.forward, mode.range, projectile.diameter, playerHolding.standingAccuracy.Calculate() + mode.projectileSpread, projectile.velocity, projectile.hitDetection);
                                EventObserver.TransmitAttack(am);
                                attackMessageDelayTimer = 0;
                            }

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

            if (magazine != null)
            {
                if (ammunition != null)
                {
                    ReloadHandler(magazine.reloadTime, fireControls.fireTimer, fireControls.roundsPerMinute, magazine.roundsReloaded, magazine.magazine, playerHolding, ammunition.ammoType);
                }
                else
                {
                    ReloadHandler(magazine.reloadTime, fireControls.fireTimer, fireControls.roundsPerMinute, magazine.roundsReloaded, magazine.magazine);
                }
            }

            if (optics != null)
            {
                AimHandler(optics, firingModes[firingModeIndex].heldPosition, playerHolding.toggleAim);
            }

            RecoilHandler();
        }
    }

    void SwitchFiringMode(int index)
    {
        mode = firingModes[index];
        projectile = projectileTypes[mode.projectileIndex];
        magazine = magazineTypes[mode.magazineIndex];
        optics = opticsTypes[mode.opticsIndex];
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

        // Reuses variable s to figure out how wide the spread should be, based on spreadCurve and spreadTimer
        s = mode.spreadCurve.Evaluate(mode.spreadTimer);

        // Calculate current spread
        float currentSpread = Mathf.Lerp(mode.projectileSpread, mode.projectileSpread * mode.maxSpreadMultiplier, s);

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
    }
}
