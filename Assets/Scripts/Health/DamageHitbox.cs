using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (Collider))]
public class DamageHitbox : MonoBehaviour
{
    public Health healthScript;
    public float damageMultiplier = 1;
    public bool critical;
    // public bool ricochetsBullets; This currently does not do anything

    public void Damage(int damage, GameObject origin, DamageType damageSource)
    {
        if (healthScript != null)
        {
            healthScript.TakeDamage(Mathf.RoundToInt(damage * damageMultiplier), origin, damageSource);
        }
        
    }

    /*
    public void Damage(int damage, float criticalModifier, GameObject origin, DamageType damageSource)
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
        healthScript.TakeDamage(d, origin, damageSource);

        WeaponHandler wh = origin.GetComponent<WeaponHandler>(); // Checks for WeaponHandler script i.e. if the thing that shot the projectile was a player
        if (wh != null)
        {
            wh.ph.hud.PlayHitMarker(critical);
        }
    }
    */
}