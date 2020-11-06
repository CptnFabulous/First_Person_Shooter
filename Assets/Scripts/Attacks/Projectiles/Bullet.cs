using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : Projectile
{
    [Header("Damage")]
    public int damage;
    public float knockback;
    public float criticalMultiplier;
    public bool friendlyFire = false;

    [Header("Other")]
    public bool passThroughFriendlies = true;
    public ParticleSystem bulletDecal;
    
    public override void OnHit(RaycastHit rh)
    {
        Character c = Character.FromObject(rh.collider.gameObject);

        if (c != null && (origin.HostileTowards(c) || friendlyFire == true))
        {
            GameObject o = rh.collider.gameObject;
            Damage.PointDamage(origin, o, damage, criticalMultiplier, DamageType.Shot, DamageType.CriticalShot);
            Damage.Knockback(o, knockback, transform.forward);
        }

        if (c == null || (origin.HostileTowards(c) == false && passThroughFriendlies == false))
        {
            InstantiateOnImpact(rh, bulletDecal.gameObject, true, true);
            base.OnHit(rh);
        }



        /*
        if (origin.HostileTowards(c) || friendlyFire == true)   
        {
            GameObject o = rh.collider.gameObject;
            Damage.PointDamage(origin, o, damage, criticalMultiplier, DamageType.Shot, DamageType.CriticalShot);
            Damage.Knockback(o, knockback, transform.forward);

            Impact(rh);
        }
        
        if (c == null || (origin.HostileTowards(c) == false && passThroughFriendlies == false))
        {
            Impact(rh);
        }
        */
    }
}