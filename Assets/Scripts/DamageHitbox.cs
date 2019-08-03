using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageHitbox : MonoBehaviour
{

    public int degreesFromHealth; // The hitbox with this script attached is meant to be a child of the object with the health script attached. 
    // Needs some kind of variable to determine critical hits.
    public float damageModifier = 1;
    public bool critical;
    public bool ricochetsBullets;

    public void Damage(int damage, DamageType damageSource)
    {
        GameObject objectWithHealthScript = gameObject;

        if (degreesFromHealth > 0)
        {
            for (int i = 0; i < degreesFromHealth; i++)
            {
                objectWithHealthScript = objectWithHealthScript.transform.parent.gameObject;
            }
        }

        Health healthScript = objectWithHealthScript.GetComponent<Health>();
        healthScript.TakeDamage(Mathf.RoundToInt(damage * damageModifier), damageSource);
    }

    public void ExplosiveDamage()
    {

    }
}
