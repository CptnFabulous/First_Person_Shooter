using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : KineticProjectile
{
    
    public ParticleSystem impactEffect;
    
    public override void OnHit()
    {
        Quaternion normalDirection = Quaternion.FromToRotation(Vector3.forward, projectileHit.normal);
        Instantiate(impactEffect.gameObject, projectileHit.point + normalDirection * Vector3.forward * 0.1f, normalDirection);
        //Instantiate(impactEffect.gameObject, projectileHit.point + normalDirection * Vector3.forward * 0.001f, normalDirection, projectileHit.collider.transform);
        base.OnHit();
    }
}