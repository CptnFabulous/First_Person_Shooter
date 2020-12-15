using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GunGeneralStats : MonoBehaviour
{
    [Header("Projectile")]
    public Projectile projectilePrefab;
    public Transform muzzle;
    public int projectileCount = 1;

    [Header("Accuracy")]
    [Range(0, 180)] public float projectileSpread;
    public float range;

    [Header("Recoil")]
    public float recoil;
    public float recoilApplyRate;
    public float recoilRecovery;

    //[Header("Spread")]
    //public float spreadMultiplier;
    //public float timeToReachMaxSpread;
    //public AnimationCurve spreadRampSpeed;

    [Header("Ammunition")]
    public bool consumesAmmo = true;
    public AmmunitionType ammoType;
    public int ammoPerShot = 1;

    [Header("Cosmetics")]
    public Transform heldPosition;
    public UnityEvent effectsOnFire;

    public void Shoot(Character origin, Vector3 aimOrigin, Vector3 forward, Vector3 up)
    {
        for (int i = 0; i < projectileCount; i++)
        {
            #region Calculate initial variables
            // Declare RaycastHit
            RaycastHit targetFound;
            // Declare direction in which to fire projectile
            Vector3 direction = new Vector3(Random.Range(-projectileSpread, projectileSpread), Random.Range(-projectileSpread, projectileSpread), Random.Range(-projectileSpread, projectileSpread));
            direction = Misc.AngledDirection(direction, forward, up);
            #endregion

            #region Launch raycast to determine where to shoot projectile
            if (Physics.Raycast(aimOrigin, direction, out targetFound, range, projectilePrefab.hitDetection)) // To reduce the amount of superfluous variables, I re-used the 'target' Vector3 in the same function as it is now unneeded for its original purpose
            {
                direction = targetFound.point;
            }
            else
            {
                direction = aimOrigin + direction.normalized * range;
            }
            #endregion

            #region Instantiate projectile
            Projectile p = projectilePrefab;

            p.origin = origin;

            // Checks that the position 'processedDirection' is actually further away than the muzzle and that the bullets will not travel in the complete wrong direction
            if (Vector3.Angle(forward, direction - muzzle.position) < 90)
            {
                Instantiate(p, muzzle.position, Quaternion.LookRotation(direction - muzzle.position, up));
            }
            else
            {
                // Otherwise, the gun barrel is probably clipping into a wall. Directly spawn the projectiles at the appropriate hit points.
                Instantiate(p, direction, Quaternion.LookRotation(direction - aimOrigin, up));
                p.OnHit(targetFound);
            }
            #endregion
        }
    }
}
