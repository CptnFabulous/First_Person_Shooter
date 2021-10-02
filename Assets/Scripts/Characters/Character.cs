using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class Character : Entity
{
    public Faction faction;
    public Health health { get; private set; }

    public virtual void Awake()
    {
        health = GetComponent<Health>();
    }


    public static Character FromObject(GameObject g)
    {
        DamageHitbox d = g.GetComponent<DamageHitbox>();
        if (d != null)
        {
            g = d.GetRootObject();
            return g.GetComponent<Character>();
        }
        return g.GetComponentInParent<Character>();
    }

    /// <summary>
    /// Returns true if this character is hostile towards the target character, or if the target is not a character
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    public bool HostileTowards(Character c)
    {
        return c == null || faction.HostileTowards(c.faction);
    }


    public bool CanDamage(Character target, bool friendlyFire, bool selfDamage)
    {
        bool isSafeToDamage = HostileTowards(target);
        bool isSelfButDamageable = target == this && selfDamage;
        bool isAllyButDamageable = !isSafeToDamage && target != this && friendlyFire;
        // If target is safe to attack
        return (isSafeToDamage || isSelfButDamageable || isAllyButDamageable);
    }




}
