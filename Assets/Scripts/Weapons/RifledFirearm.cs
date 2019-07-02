using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RifledFirearm : MonoBehaviour
{
    /*
    A weapon script for all guns that shoot regular bullets, e.g. rifles, handguns and machine guns.
    */


    [Header("Damage")]
    public int damage = 10;
    public float criticalModifier = 2;

    [Header("Accuracy")]
    [Range(0, 180)] public float projectileSpread = 5;
    public float recoil = 10;
    public float recoilRecovery = 10;
    Ray targetRay;
    RaycastHit targetFound;
    public float rayRange;
    public LayerMask rayDetection;

    [Header("Fire Rate")]
    public float roundsPerMinute = 600;
    [Tooltip("Amount of shots that can be fired before needing to re-press the trigger. Set to 1 for semi-automatic, or more for burst-fire weapons. Set to zero to enable full-auto fire.")]
    public int burstCount;
    float fireTimer;
    float shotsInBurst;

    [Header("Projectile")]
    public RaycastBullet projectile;
    public Transform weaponMuzzle;
    public float projectileDiameter = 0.1f;
    public float gravityMultiplier; // Determines gravity effect on projectile for bullet drop, set to zero to disable bullet drop
    public float projectileVelocity = 100;
    //public bool gravityAffected;

    [Header("Ammunition")]
    public Ammunition ammoSource; // The 'ammunition supply' e.g. the player shooting the gun
    public AmmunitionType caliber;
    [Min(1)] public int magazineCapacity;
    public int roundsInMagazine = 30;
    public float reloadTime = 2;
    bool isReloading;
    float reloadTimer;

    [Header("Cosmetics")]
    public ParticleSystem muzzleFlashEffect;
    public ParticleSystem impactEffect;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        fireTimer += Time.deltaTime;


        if (Input.GetButton("MouseLeft") && fireTimer >= 60 / roundsPerMinute && roundsInMagazine > 0 && isReloading == false && (shotsInBurst < burstCount || burstCount <= 0)) // If fire button is pressed, previous shot has finished firing, ammo remains in magazine and maximum burst count has not been exceeded
        {
            LaunchProjectile(); // Initiate shooting. This may change depending on the kind of weapon, e.g. with a shotgun this would run multiple times in a for loop.

            //muzzleFlashEffect.Play();
            //roundsInMagazine -= 1;

            fireTimer = 0;

            if (burstCount > 0)
            {
                shotsInBurst += 1;
            }
        }
        else if (Input.GetButtonUp("MouseLeft") && burstCount > 0)
        {
            shotsInBurst = 0;
        }

        if ((Input.GetButtonDown("Reload") && roundsInMagazine < magazineCapacity) || (roundsInMagazine <= 0 && fireTimer >= 60 / roundsPerMinute && isReloading == false))
        {
            //StartCoroutine(Reload()); // Perform reload function

            reloadTimer = 0;
            isReloading = true;
        }
        if (isReloading == true)
        {
            Reload();
        }
    }

    void Reload() // Untested
    {
        reloadTimer += Time.deltaTime;
        if (reloadTimer >= reloadTime)
        {
            roundsInMagazine = magazineCapacity; // This simply sets the magzine capacity to the maximum, meaning the player has infinite ammo. Substitute appropriate code for subtracting ammo from reserve 
            isReloading = false;
        }
    }

    void LaunchProjectile()
    {
        Vector3 target = new Vector3();

        targetRay.origin = transform.position;
        /*
        Vector3 raySpread = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1));
        raySpread.Normalize();
        targetRay.direction = Quaternion.Euler(raySpread.x * projectileSpread, raySpread.y * projectileSpread, raySpread.z * projectileSpread) * transform.forward;
        */
        targetRay.direction = Quaternion.Euler(Random.Range(-projectileSpread, projectileSpread), Random.Range(-projectileSpread, projectileSpread), Random.Range(-projectileSpread, projectileSpread)) * transform.forward;

        if (Physics.Raycast(targetRay, out targetFound, rayRange, rayDetection))
        {
            target = targetFound.point;
            //print(target);
        }
        else
        {
            target = targetRay.direction * 9999999999;
        }

        GameObject bullet = Instantiate(projectile.gameObject, weaponMuzzle.transform.position, Quaternion.LookRotation(target - weaponMuzzle.transform.position, Vector3.up));

        projectile.diameter = projectileDiameter;
        projectile.gravityMultiplier = gravityMultiplier;
        projectile.velocity = projectileVelocity;

        projectile.damage = damage;
        projectile.criticalModifier = criticalModifier;

        projectile.rayDetection = rayDetection;

    }

    /*
    IEnumerator Reload()
    {
        isReloading = true;
        roundsInMagazine = 0;
        yield return new WaitForSeconds(reloadTime);
        roundsInMagazine = magazineCapacity; // This simply sets the magzine capacity to the maximum, meaning the player has infinite ammo. Substitute appropriate code for subtracting ammo from reserve 
        isReloading = false;
    }
    */
}
