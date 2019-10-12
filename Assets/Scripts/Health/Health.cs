using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public Resource health = new Resource { max = 100, current = 100, critical = 20 };
    int prevHealth;

    DamageType lastDamageSource;
    GameObject lastAttacker;

#if UNITY_EDITOR
    void Reset() { OnValidate(); }
    void OnValidate()
    {
        //magazineCapacity = Mathf.Clamp(magazineCapacity, 0, Mathf.Infinity);
        health.current = Mathf.Clamp(health.current, 0, health.max);
    }
#endif

    // Start is called before the first frame update
    void Start()
    {
        prevHealth = health.current;
    }

    // Update is called once per frame
    public virtual void Update()
    {


        if (prevHealth != health.current) // if health has changed
        {
            HealthChanged();
        }
        prevHealth = health.current;
    }

    public virtual void HealthChanged() // Do stuff here when health changes
    {
        if (health.current <= 0)
        {
            Die(lastDamageSource);
        }

        // DO ADDITIONAL STUFF HERE IN DERIVED CLASSES, e.g. pain/death animations
    }

    public virtual void TakeDamage(int damageAmount, GameObject origin, DamageType damageSource)
    {
        health.current -= damageAmount;
        lastAttacker = origin;
        lastDamageSource = damageSource;
    }

    public virtual void Die(DamageType causeOfDeath)
    {
        Destroy(gameObject); // Destroy gameobject upon death, when using inherited classes override this function to implement different code such as death animations etc.)
    }
}
