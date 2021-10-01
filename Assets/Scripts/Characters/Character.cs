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

    public bool HostileTowards(Character c)
    {
        return faction.HostileTowards(c.faction);
    }







}
