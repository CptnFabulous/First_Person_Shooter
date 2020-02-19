using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : KineticProjectile
{
    
    public ParticleSystem impactEffect;
    
    public override void OnHit()
    {
        //GameObject ie = 
        //Quaternion normalDirection = Quaternion.LookRotation(-projectileHit.normal, Vector3.up);


        //Quaternion normalDirection = Quaternion.FromToRotation(Vector3.back, projectileHit.normal);


        //Quaternion.FromToRotation(Vector3.back, surfaceFound.normal)


        //Instantiate(impactEffect.gameObject, projectileHit.point + normalDirection * transform.forward, normalDirection);

        Quaternion normalDirection = Quaternion.FromToRotation(Vector3.forward, projectileHit.normal);
        Instantiate(impactEffect.gameObject, projectileHit.point + normalDirection * Vector3.forward * 0.1f, normalDirection);
        //impactEffect.GetComponent<ParticleSystemRenderer>().material = projectileHit.collider.GetComponent<Renderer>().material;
        base.OnHit();
    }
    
}