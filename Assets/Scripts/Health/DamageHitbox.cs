using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (Collider))]
public class DamageHitbox : MonoBehaviour
{

    public int degreesFromHealth; // The hitbox with this script attached is meant to be a child of the object with the health script attached. 
    // Needs some kind of variable to determine critical hits.
    public float damageMultiplier = 1;
    public bool critical;
    // public bool ricochetsBullets; This currently does not do anything

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
        healthScript.TakeDamage(Mathf.RoundToInt(damage * damageMultiplier), damageSource);
    }

    public void Damage(int damage, float criticalModifier, DamageType damageSource)
    {
        // Checks hitbox's tree to look for the appropriate health script
        GameObject objectWithHealthScript = gameObject;
        if (degreesFromHealth > 0)
        {
            for (int i = 0; i < degreesFromHealth; i++)
            {
                objectWithHealthScript = objectWithHealthScript.transform.parent.gameObject;
            }
        }

        // Calculates appropriate damage to deal
        int d = Mathf.RoundToInt(damage * damageMultiplier);
        if (critical == true)
        {
            d = Mathf.RoundToInt(d * criticalModifier);
        }

        // Finds health script and deals damage
        Health healthScript = objectWithHealthScript.GetComponent<Health>();
        healthScript.TakeDamage(d, damageSource);
    }
}