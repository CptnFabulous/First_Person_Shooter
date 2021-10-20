using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GunGeneralStats : MonoBehaviour
{
    

    [Header("Accuracy")]
    [Range(0, 180)] public float projectileSpread;
    public float range;

    [Header("Recoil")]
    public float recoil;
    public float recoilApplyRate;
    //public float recoilRecovery;

    //[Header("Spread")]
    //public float spreadMultiplier;
    //public float timeToReachMaxSpread;
    //public AnimationCurve spreadRampSpeed;

    [Header("Ammunition")]
    public bool consumesAmmo = true;
    public AmmunitionType ammoType;
    public int ammoPerShot = 1;

    

    [Header("Projectile")]
    public Projectile projectilePrefab;
    public Transform muzzle;
    public int projectileCount = 1;


    [Header("Cosmetics")]
    public Transform heldPosition;
    public UnityEvent effectsOnFire;



    public void Shoot(Character origin, Vector3 aimOrigin, Vector3 forward, Vector3 up)
    {
        for (int i = 0; i < projectileCount; i++)
        {
            RaycastHit targetFound; // Declare RaycastHit
            // Declare direction in which to fire projectile
            Vector3 direction = new Vector3(Random.Range(-projectileSpread, projectileSpread), Random.Range(-projectileSpread, projectileSpread), Random.Range(-projectileSpread, projectileSpread));
            direction = Misc.AngledDirection(direction, forward, up);
            // To reduce the amount of superfluous variables, I re-used the 'target' Vector3 in the same function as it is now unneeded for its original purpose
            if (Physics.Raycast(aimOrigin, direction, out targetFound, range, projectilePrefab.hitDetection))
            {
                // If the raycast hits, store the point where it hit.
                direction = targetFound.point;
                Debug.DrawLine(aimOrigin, direction, Color.green, 2f);
            }
            else
            {
                // If the raycast didn't hit, store the point at the raycast's maximum reach.
                direction = aimOrigin + direction.normalized * range;
            }

            Projectile p =  Instantiate(projectilePrefab);
            p.gameObject.SetActive(true);
            p.origin = origin;

            // Checks that the position 'processedDirection' is actually further away than the muzzle and that the bullets will not travel in the complete wrong direction
            if (Vector3.Angle(forward, direction - muzzle.position) < 90)
            {
                p.transform.position = muzzle.position;
                p.transform.rotation = Quaternion.LookRotation(direction - muzzle.position, up);
            }
            else
            {
                // Otherwise, the gun barrel is probably clipping into a wall. Directly spawn the projectiles at the appropriate hit points.
                p.transform.position = direction;
                p.transform.rotation = Quaternion.LookRotation(direction - aimOrigin, up);
                p.OnHit(targetFound);
            }
        }

        effectsOnFire.Invoke();
    }



    /*
    public void CosmeticRecoil(float force)
    {
        weaponVisualRigidbody.AddForceAtPosition(-muzzle.forward * force, muzzle.position, ForceMode.Impulse);
    }
    */
}
