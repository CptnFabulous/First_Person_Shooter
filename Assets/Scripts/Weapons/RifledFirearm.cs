using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RifledFirearm : MonoBehaviour
{
    

    [Header("Accuracy")]
    public float weaponSpread;
    public float recoil;
    public float recoilRecovery;
    Ray targetRay;
    RaycastHit targetFound;
    public float rayRange;
    public LayerMask rayDetection;

    [Header("Fire Rate")]
    public float roundsPerMinute;
    public int burstCount;
    float fireTimer;
    float shotsInBurst;

    [Header("Projectile")]
    public RaycastBullet projectile;
    public Transform weaponMuzzle;
    public float projectileDiameter;
    public float projectileMass; // Determines gravity effect on projectile for bullet drop, set to zero to disable bullet drop
    public float projectileVelocity;
    //public bool gravityAffected;

    [Header("Ammunition")]
    public Ammunition ammoSource; // The 'ammunition supply' e.g. the player shooting the gun
    public AmmunitionType caliber;
    [Min(1)] public int magazineCapacity;
    public int roundsInMagazine;
    public float reloadTime;
    bool isReloading;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        fireTimer += Time.deltaTime;


        if (Input.GetButton("MouseLeft") && fireTimer >= 60 / roundsPerMinute && roundsInMagazine > 0 && (shotsInBurst < burstCount || burstCount <= 0)) // If fire button is pressed, previous shot has finished firing, ammo remains in magazine and maximum burst count has not been exceeded
        {
            LaunchProjectile();

            fireTimer = 0;
            if (burstCount > 0)
            {
                shotsInBurst += 1;
            }
        }
        else if (Input.GetButtonUp("MouseLeft"))
        {
            shotsInBurst = 0;
        }

        if ((Input.GetButtonDown("Reload") && roundsInMagazine < magazineCapacity) || (roundsInMagazine <= 0 && fireTimer >= 60 / roundsPerMinute && isReloading == false))
        {
            StartCoroutine(Reload());
            // Perform reload function
        }
    }

    void LaunchProjectile()
    {
        Vector3 target = new Vector3();

        targetRay.origin = transform.position;
        /*
        Vector3 raySpread = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1));
        raySpread.Normalize();
        targetRay.direction = Quaternion.Euler(raySpread.x * weaponSpread, raySpread.y * weaponSpread, raySpread.z * weaponSpread) * transform.forward;
        */
        targetRay.direction = Quaternion.Euler(Random.Range(-weaponSpread, weaponSpread), Random.Range(-weaponSpread, weaponSpread), Random.Range(-weaponSpread, weaponSpread)) * transform.forward;

        if (Physics.Raycast(targetRay, out targetFound, rayRange, rayDetection))
        {
            target = targetFound.point;
        }
        else
        {
            target = targetRay.direction * 9999999999;
        }

        /*
        #region Raycast-based projectiles
        GameObject projectile = Instantiate(projectilePrefab, weaponMuzzle.transform.position, Quaternion.LookRotation(target, Vector3.up));
        RaycastBullet projectileData = GetComponent<RaycastBullet>();

        projectileData.diameter = projectileDiameter;
        projectileData.velocity = projectileVelocity;
        #endregion
        */

        GameObject bullet = Instantiate(projectile.gameObject, weaponMuzzle.transform.position, Quaternion.LookRotation(target, Vector3.up));
        projectile.diameter = projectileDiameter;
        projectile.mass = projectileMass;
        projectile.velocity = projectileVelocity;
        //projectile.gravityAffected = gravityAffected;

    }

    IEnumerator Reload()
    {
        isReloading = true;
        roundsInMagazine = 0;
        yield return new WaitForSeconds(reloadTime);
        roundsInMagazine = magazineCapacity; // This simply sets the magzine capacity to the maximum, meaning the player has infinite ammo. Substitute appropriate code for subtracting ammo from reserve 
        isReloading = false;
    }
}
