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
    public Collider Collider { get; private set; }

    private void Awake()
    {
        Collider = GetComponent<Collider>();
        Collider.isTrigger = false;
    }

    /// <summary>
    /// Damages the object whose health meter this component is attached to.
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="origin"></param>
    /// <param name="typeOfDamage"></param>
    public void Damage(int damage, Entity origin, DamageType typeOfDamage)
    {
        int finalDamage = Mathf.RoundToInt(damage * damageMultiplier);
        healthScript.Damage(finalDamage, origin, typeOfDamage);
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