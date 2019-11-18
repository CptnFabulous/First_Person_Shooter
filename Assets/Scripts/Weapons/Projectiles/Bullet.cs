using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : KineticProjectile
{
    
    public ParticleSystem impactEffect;
    /*
    public override void OnHit()
    {
        //GameObject ie = 
        Quaternion normalDirection = Quaternion.LookRotation(-projectileHit.normal, Vector3.up);
        Instantiate(impactEffect.gameObject, projectileHit.point + normalDirection * transform.forward, normalDirection);
        impactEffect.GetComponent<ParticleSystemRenderer>().material = projectileHit.collider.GetComponent<Renderer>().material;
        base.OnHit();
    }
    */
}