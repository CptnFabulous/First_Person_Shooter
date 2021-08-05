using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class Character : Entity
{
    public Faction faction;
    public Health HealthData { get; private set; }

    public virtual void Awake()
    {
        HealthData = GetComponent<Health>();
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
