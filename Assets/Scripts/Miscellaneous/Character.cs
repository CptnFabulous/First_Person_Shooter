using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : Entity
{
    public Faction faction;

    public static Character FromObject(GameObject g)
    {
        DamageHitbox d = g.GetComponent<DamageHitbox>();
        if (d != null)
        {
            g = d.GetRootObject();
            return g.GetComponent<Character>();
        }
        return g.GetComponentInParent<Character>();

        //return g.transform.root.GetComponent<Character>();
    }

    public bool HostileTowards(Character c)
    {
        return faction.HostileTowards(c.faction);
    }
}
