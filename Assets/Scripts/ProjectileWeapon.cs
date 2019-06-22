/*

using UnityEngine;

public class ProjectileWeapon : MonoBehaviour
{
    
    
    

    [Header("References")]
    public GameObject Inventory;
    public Sprite uiIcon;
    public GameObject head;



    [Header("Ammunition")]
    public AmmunitionType ammo;
    [Tooltip("Amount of ammunition weapon can hold before needing to reload.")]
    public int magazineCapacity;
    [Tooltip("Amount of ammunition currently in the weapon's magazine.")]
    public int ammoInMagazine;
    [Tooltip("Time taken to reload the weapon, in seconds.")]
    public float reloadSpeed;

    public bool loadAmmoIndividually;

    [Header("Firing mode")]
    [Tooltip("Cyclic fire rate, in rounds per minute.")]
    public float roundsPerMinute;

    /*
    [Tooltip("How many shots can be fired per button press. For semi-automatic fire, enter 1. Set to 0 to disable burst function")]
    public int burstCount;

    float burstCurrent;
    bool burstMax;

    
    [Header("Damage")]
    [Tooltip("Damage points dealt upon a successful hit, per projectile.")]
    public int damage;
    [Tooltip("Time taken to reload the weapon, in seconds.")]
    public int damageCritical;
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
    

    /*
    [Header("Optics")]
    public bool hasOptics;
    public Transform hipfirePosition;
    public Transform sightsPosition;
    public float magnification;
    public float aimSpeed;
    

    [Header("Projectile")]
    [Tooltip("Amount of projectiles launched per shot. Set to 1 for regular bullet-shooting weapons, increase for weapons such as shotguns.")]
    public int projectileCount;
    [Tooltip("How fast said projectiles move through the air. Set high for bullets, slower for grenades, arrows and similar projectiles.")]
    public float muzzleVelocity;
    [Tooltip("Whether the projectile is affected by gravity.")]
    public bool gravityAffected;
    [Tooltip("Projectile being fired.")]
    public GameObject projectilePrefab;
    [Tooltip("Point on weapon that projectile is spawned at.")]
    public Transform weaponMuzzle;

    [Header("Visual effects")]
    public ParticleSystem ejectionPort;

    public GameObject impactEffect;

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
                    ShootProjectile();
                    //print("Projectile shot");
                }
                // Do stuff like consume ammo, reset fire timer
                weaponSpread *= recoilMultiplier;
                //print(recoilMultiplier);
                fireTimer = 0;

                ejectionPort.Play();
            }
        }
        else // if fire button is not pressed
        {
            weaponSpread -= recoilRecovery * Time.deltaTime;
        }
        weaponSpread = Mathf.Clamp(weaponSpread, weaponSpreadMin, recoilMax);
        

        


    }

    void ShootProjectile()
    {
        targetRay.origin = transform.position;
        targetRay.direction = Quaternion.Euler(Random.Range(-weaponSpread, weaponSpread), Random.Range(-weaponSpread, weaponSpread), Random.Range(-weaponSpread, weaponSpread)) * transform.forward;
        // Figure out alternate method to have bullet spread occur in a circle

        if (Physics.Raycast(targetRay, out targetFound, 100, ignorePlayer))
        {
            target = targetFound.point;
        }
        else
        {
            target = targetRay.direction * 9999999999;
        }

        Quaternion bulletDirection = Quaternion.Euler(90, 0, 0);
        GameObject projectile = (GameObject)Instantiate(projectilePrefab, weaponMuzzle.transform.position, transform.rotation * bulletDirection);
        Vector3 direction = (target - weaponMuzzle.transform.position).normalized;

        LaunchedProjectile projectileData = projectile.GetComponent<LaunchedProjectile>();
        projectileData.impactEffect = impactEffect;

        Rigidbody projectileRigidbody = projectile.GetComponent<Rigidbody>(); // Obtains rigidbody script for projectile, to alter physics variables
        projectileRigidbody.useGravity = gravityAffected; // Alters projectile gravity based on gravityAffected bool
        projectileRigidbody.AddForce(direction * muzzleVelocity); // Launches projectile in the direction of where the target raycast hits, based on the muzzleVelocity variable.

        
    }
}
*/

using UnityEngine;

public class ProjectileWeapon : MonoBehaviour
{

    [Header("References")]
    public GameObject Inventory;
    public Sprite uiIcon;
    public GameObject head;



    [Header("Ammunition")]
    public AmmunitionType ammo;
    [Tooltip("Amount of ammunition weapon can hold before needing to reload.")]
    public int magazineCapacity;
    [Tooltip("Amount of ammunition currently in the weapon's magazine.")]
    public int ammoInMagazine;
    [Tooltip("Time taken to reload the weapon, in seconds.")]
    public float reloadSpeed;
    public bool loadAmmoIndividually;

    [Header("Firing mode")]
    [Tooltip("Cyclic fire rate, in rounds per minute.")]
    public float roundsPerMinute;
    public float burstCount;

    [Header("Accuracy")]

    [Tooltip("Level of deviation in weapon accuracy from centre of reticle, in degrees.")]
    public float weaponSpread;
    [Tooltip("Amount of recoil applied per shot.")]
    public float recoil;
    [Tooltip("Speed at which camera returns to starting position.")]
    public float recoilRecovery;

    [Header("Projectile")]
    [Tooltip("Amount of projectiles launched per shot. Set to 1 for regular bullet-shooting weapons, increase for weapons such as shotguns.")]
    public int projectileCount;
    [Tooltip("How fast said projectiles move through the air. Set high for bullets, slower for grenades, arrows and similar projectiles.")]
    public float muzzleVelocity;
    [Tooltip("Whether the projectile is affected by gravity.")]
    public bool gravityAffected;
    [Tooltip("Projectile being fired.")]
    public GameObject projectilePrefab;
    [Tooltip("Point on weapon that projectile is spawned at.")]
    public Transform weaponMuzzle;

    [Header("Visual effects")]
    public ParticleSystem ejectionPort;

    public GameObject impactEffect;

    Ray targetRay; // Raycast launched to determine shot direction
    RaycastHit targetFound; // Point where raycast hits target
    LayerMask ignorePlayer = (1 << 8); // LayerMask ensuring raycast does not hit player's own body
    Vector3 target; // Vector3 used to represent where the raycast landed, and where the projectile must travel towards

    float fireTimer;




    // Use this for initialization
    void Start()
    {
        ignorePlayer = ~ignorePlayer;
    }

    // Update is called once per frame
    void Update()
    {
        fireTimer += Time.deltaTime; // Timer counts up to determine when weapon is ready to fire

        float Fire1 = Input.GetAxis("RightTrigger"); // Obtains input from right trigger
        if ((Input.GetButton("MouseLeft") || Fire1 > 0) && fireTimer >= 60 / roundsPerMinute && ammoInMagazine > 0) // If the following conditions are met: Fire button is pressed, the previous round has finished firing and there is ammunition remaining in the gun's magazine
        {
            for (int i = 0; i < projectileCount; i++) // Shoot multiple projectiles according to the integer 'projectileCount'
            {
                ShootProjectile();
            }
            fireTimer = 0; // Resets delay before next shot can be fired

            // Consume ammunition
            // Play firing noise

            // RECOIL CODE IS BROKEN
            Vector2 headRecoil = new Vector2(0, -1);
            headRecoil.Normalize();
            head.transform.Rotate(headRecoil.y * recoil, headRecoil.x * recoil, 0, Space.Self);
            

            ejectionPort.Play(); // Animation for ejecting shell casing

        }

    }

    void ShootProjectile()
    {
        targetRay.origin = transform.position;
        targetRay.direction = Quaternion.Euler(Random.Range(-weaponSpread, weaponSpread), Random.Range(-weaponSpread, weaponSpread), Random.Range(-weaponSpread, weaponSpread)) * transform.forward;
        // Figure out alternate method to have bullet spread occur in a circle

        if (Physics.Raycast(targetRay, out targetFound, 100, ignorePlayer))
        {
            target = targetFound.point;
        }
        else
        {
            target = targetRay.direction * 9999999999;
        }

        Quaternion bulletDirection = Quaternion.Euler(90, 0, 0);
        GameObject projectile = (GameObject)Instantiate(projectilePrefab, weaponMuzzle.transform.position, transform.rotation * bulletDirection);
        Vector3 direction = (target - weaponMuzzle.transform.position).normalized;

        LaunchedProjectile projectileData = projectile.GetComponent<LaunchedProjectile>();
        projectileData.impactEffect = impactEffect;

        Rigidbody projectileRigidbody = projectile.GetComponent<Rigidbody>(); // Obtains rigidbody script for projectile, to alter physics variables
        projectileRigidbody.useGravity = gravityAffected; // Alters projectile gravity based on gravityAffected bool
        projectileRigidbody.AddForce(direction * muzzleVelocity); // Launches projectile in the direction of where the target raycast hits, based on the muzzleVelocity variable.

        
    }
}

