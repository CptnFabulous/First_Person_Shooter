using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public string properName;
    public string description;
    public Faction faction;

    /*
    public static Character FromHitbox(GameObject g)
    {
        DamageHitbox d = g.GetComponent<DamageHitbox>();
        if (d != null)
        {
            Health h = d.healthScript;
            if (h != null)
            {
                Character ch = h.h;
                if (ch != null)
                {
                    return ch;
                }
            }
        }
        return null;
    }
    */

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
        Health h = d.healthScript;
        if (h != null)
        {
            return FromHealth(h);
        }
        return null;
    }

    public static Character FromHealth(Health h)
    {
        Character ch = h.h;
        if (ch != null)
        {
            return ch;
        }
        return null;
    }
}
