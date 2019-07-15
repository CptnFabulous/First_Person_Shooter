using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodyProjectileWeapon : Weapon
{
    public float criticalModifier;

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
    float fireTimer;

    [Header("Projectile")]
    public GameObject projectilePrefab;
    public Transform weaponMuzzle;
    public int projectileCount;
    public float projectileDiameter;
    public float projectileVelocity;
    public bool gravityAffected;

    [Header("Ammunition")]
    //public Ammunition ammoType;
    public int magazineCapacity;
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
        Vector3 raySpread = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1));
        raySpread.Normalize();
        targetRay.direction = Quaternion.Euler(raySpread.x * weaponSpread, raySpread.y * weaponSpread, raySpread.z * weaponSpread) * transform.forward;
        //targetRay.direction = Quaternion.Euler(Random.Range(-weaponSpread, weaponSpread), Random.Range(-weaponSpread, weaponSpread), Random.Range(-weaponSpread, weaponSpread)) * transform.forward;

        if (Physics.Raycast(targetRay, out targetFound, rayRange, rayDetection))
        {
            target = targetFound.point;
        }
        else
        {
            target = targetRay.direction * 9999999999;
        }


        #region Raycast-based projectiles
        GameObject projectile = Instantiate(projectilePrefab, weaponMuzzle.transform.position, Quaternion.LookRotation(target, Vector3.up));
        RaycastBullet projectileData = GetComponent<RaycastBullet>();

        projectileData.diameter = projectileDiameter;
        projectileData.velocity = projectileVelocity;
        #endregion
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
