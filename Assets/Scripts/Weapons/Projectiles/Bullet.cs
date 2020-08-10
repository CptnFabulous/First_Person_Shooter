using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : KineticProjectile
{
    
    public ParticleSystem impactEffect;
    
    public override void OnHit(RaycastHit rh)
    {
        InstantiateOnImpact(rh, impactEffect.gameObject, true);
        base.OnHit(rh);
    }
}