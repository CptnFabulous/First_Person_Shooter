using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : KineticProjectile
{
    
    public ParticleSystem impactEffect;
    
    public override void OnHit()
    {
        InstantiateOnImpact(impactEffect.gameObject, true);
        base.OnHit();
    }
}