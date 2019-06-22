using UnityEngine;

public class RangedWeapon : MonoBehaviour
{
    [Header("References")]
    public GameObject Inventory;
    public Sprite uiIcon;

    [Header("Ammunition")]
    public AmmunitionType ammo;
    [Tooltip("Amount of ammunition weapon can hold before needing to reload.")]
    public int magazineCapacity;
    [Tooltip("Amount of ammunition currently in the weapon's magazine.")]
    public int ammoInMagazine;
    [Tooltip("Time taken to reload the weapon, in seconds.")]
    public float reloadSpeed;
    [Tooltip("If true, ammunition is loaded one at a time instead of all at once. Used for weapons with fixed magazines such as shotguns with tubular magazines.")]
    public bool loadAmmoIndividually;

    [Header("Firing mode")]
    [Tooltip("Cyclic fire rate, in rounds per minute.")]
    public float roundsPerMinute;

    /*
    [Tooltip("How many shots can be fired per button press. For semi-automatic fire, enter 1. Set to 0 to disable burst function")]
    public int burstCount;

    float burstCurrent;
    bool burstMax;
    */

    [Header("Damage")]
    [Tooltip("Damage points dealt upon a successful hit, per projectile.")]
    public int damage;
    [Tooltip("Multiplies damage by this amount if a critical point is struck")]
    public int criticalModifier;




    [Tooltip("Radius in which splash damage is dealt.")]
    public float splashRadius;
    [Tooltip("The way that splash damage decreases the further an enemy is from the explosion. Currently does not work.")]
    public AnimationCurve splashFalloff;
    [Tooltip("The amount of force applied to an enemy struck by the projectile.")]
    public float hitKnockback;
    [Tooltip("The amount of force applied to enemies caught in the weapon's splash radius.")]
    public float splashKnockback;
    

    [Header("Accuracy")]
    [Tooltip("Level of deviation in weapon accuracy from centre of reticle, in degrees.")]
    public float weaponSpreadMin;
    [Tooltip("Maximum amount that recoil can increase the weapon's spread, in degrees.")]
    public float recoilMax;
    [Tooltip("Amount that weapon spread multiplies by, per shot.")] //Maybe 1.5 or something, for exponentially increasing recoil
    public float recoilMultiplier;
    [Tooltip("Amount that weapon spread divides by when not firing.")]
    public float recoilRecovery;
    [HideInInspector]
    public float weaponSpread;


    /*
    [Tooltip("Amount that weapon spread increases by, per shot.")]
    public float recoilRate; //Flat increase
    */

    /*
    [Header("Optics")]
    public bool hasOptics;
    public Transform hipfirePosition;
    public Transform sightsPosition;
    public float magnification;
    public float aimSpeed;
    */

    [Header("Projectile")]
    [Tooltip("Weight of projectile, determines effect of gravity")]
    public float projectileMass;
    [Tooltip("The bullet's size radius")]
    public float projectileRadius;
    [Tooltip("How fast said projectiles move through the air. Set high for bullets, slower for grenades, arrows and similar projectiles.")]
    public float muzzleVelocity;
    [Tooltip("Whether the projectile is affected by gravity.")]
    public bool gravityAffected;
    [Tooltip("Amount of projectiles launched per shot. Set to 1 for regular bullet-shooting weapons, increase for weapons such as shotguns.")]
    public int projectileCount;
    [Tooltip("Projectile being fired.")]
    public GameObject bulletPrefab;
    [Tooltip("Point on weapon that projectile is spawned at.")]
    public Transform weaponMuzzle;

    [Header("Visual effects")]
    public GameObject weaponObject;
    public ParticleSystem ejectionPort;

    public GameObject impactEffect;

    public Transform hipfirePosition;
    public Transform ADSPosition;
    Transform gunLocation;

    Ray targetRay; // Raycast launched to determine shot direction
    RaycastHit targetFound; // Point where raycast hits target
    LayerMask ignorePlayer = (1 << 8); // LayerMask ensuring raycast does not hit player's own body
    Vector3 target; // Vector3 used to represent where the raycast landed, and where the projectile must travel towards

    float fireTimer;




    // Use this for initialization
    void Start()
    {
        ignorePlayer = ~ignorePlayer;
        weaponSpread = weaponSpreadMin;
    }

    // Update is called once per frame
    void Update()
    {
        fireTimer += Time.deltaTime;
        //Mathf.Clamp(ammoInMagazine, 0, magazineCapacity);

        float Fire1 = Input.GetAxis("RightTrigger");
        if (Input.GetButton("MouseLeft") || Fire1 > 0)
        {
            //Calculate stuff like burst counts
            if (fireTimer >= 60 / roundsPerMinute && ammoInMagazine > 0)
            {
                for (int i = 0; i < projectileCount; i++) // Perform commands inside brackets an amount of times equal to int 'projectileCount'
                {
                    //ShootProjectile();
                    ShootBullet();
                    //print("Projectile shot " + Time.time);
                }
                // Do stuff like consume ammo, reset fire timer
                weaponSpread *= recoilMultiplier;
                ejectionPort.Play();
                fireTimer = 0;
            }
        }
        else // if fire button is not pressed
        {
            //burstCurrent = 0;

            weaponSpread -= recoilRecovery * Time.deltaTime;
        }
        weaponSpread = Mathf.Clamp(weaponSpread, weaponSpreadMin, recoilMax);
    }

    void ShootBullet()
    {
        /*
        targetRay.origin = transform.position;
        targetRay.direction = Quaternion.Euler(Random.Range(-weaponSpread, weaponSpread), Random.Range(-weaponSpread, weaponSpread), 0) * transform.forward; // Figure out alternate method to have bullet spread occur in a circle
        if (Physics.Raycast(targetRay, out targetFound, 10000, ignorePlayer))
        {
            target = targetFound.point;
        }
        else
        {
            target = targetRay.direction * 9999999999;
        }
        */

        GameObject bullet = bulletPrefab; // 'Creates' an instance of a bullet
        BulletManager bulletData = bullet.GetComponent<BulletManager>(); // Accesses the bullet's script

        // Applies appropriate physical stats to the bullet
        bulletData.mass = projectileMass;
        bulletData.radius = projectileRadius;
        bulletData.velocity = muzzleVelocity;
        bulletData.gravityAffected = gravityAffected;

        // Applies appropriate damage stats to the bullet
        bulletData.damage = damage;
        bulletData.criticalModifier = criticalModifier;

        // Applies appropriate visual effects to the bullet
        bulletData.impactPrefab = impactEffect;

        Quaternion bulletDirection = Quaternion.Euler(Random.Range(-weaponSpread, weaponSpread), Random.Range(-weaponSpread, weaponSpread), Random.Range(-weaponSpread, weaponSpread)); // Calculates weapon spread
        bulletDirection = Quaternion.Euler(transform.eulerAngles + bulletDirection.eulerAngles); // Creates a quaternion that combines the direction the player is looking in and the spread modifier
        Instantiate(bullet, transform.position, bulletDirection); // Spawns the bullet with appropriate statistics, in the correct direction.
    }
}
