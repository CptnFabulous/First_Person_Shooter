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

    public void Damage(int damage, GameObject origin, Faction originFaction, DamageType damageSource, bool isSevere)
    {
        Character c = Character.FromHitbox(this);
        if (c == null || originFaction == null || originFaction.Affiliation(c.faction) == FactionState.Hostile)
        {
            if (healthScript != null)
            {
                healthScript.TakeDamage(Mathf.RoundToInt(damage * damageMultiplier), origin, damageSource);
            }

            PlayerHandler ph = origin.GetComponent<PlayerHandler>(); // Checks for WeaponHandler script i.e. if the thing that shot the projectile was a player
            if (ph != null)
            {
                ph.hud.PlayHitMarker(isSevere);
            }
        }
    }
    
    public void Damage(int damage, float criticalMultiplier, GameObject origin, Faction originFaction, DamageType normalType, DamageType criticalType)
    {
        Character c = Character.FromHitbox(this);
        if (c == null || originFaction == null || originFaction.Affiliation(c.faction) == FactionState.Hostile)
        {
            if (healthScript != null)
            {
                // Calculates appropriate damage to deal
                DamageType dt = normalType;
                float d = damage * damageMultiplier;
                if (critical == true)
                {
                    d *= criticalMultiplier;
                    dt = criticalType;
                }
                healthScript.TakeDamage(Mathf.RoundToInt(d), origin, dt);
            }

            PlayerHandler ph = origin.GetComponent<PlayerHandler>(); // Checks for WeaponHandler script i.e. if the thing that shot the projectile was a player
            if (ph != null)
            {
                ph.hud.PlayHitMarker(critical);
            }
        }
    }

    public GameObject GetRootObject()
    {
        if (healthScript != null)
        {
            return healthScript.gameObject;
        }
        return transform.root.gameObject;
    }

    
    
}