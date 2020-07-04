using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : Entity
{
    public Faction faction;

    public static Character FromHit(GameObject g)
    {
        DamageHitbox d = g.GetComponent<DamageHitbox>();
        if (d != null)
        {
            return FromHitbox(d);
        }
        return null;
    }

    public static Character FromHitbox(DamageHitbox d)
    {
        GameObject g = d.GetRootObject();
        return g.GetComponent<Character>();
    }
}
