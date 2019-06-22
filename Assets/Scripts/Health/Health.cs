using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    
    public int maxHealth = 100;
    public int currentHealth = 100;
    int prevHealth;

    DamageType lastDamageSource;

    
    // Start is called before the first frame update
    void Start()
    {
        prevHealth = currentHealth;
    }

    // Update is called once per frame
    public virtual void Update()
    {


        if (prevHealth != currentHealth) // if health has changed
        {
            HealthChanged();
        }
        prevHealth = currentHealth;
    }

    public virtual void HealthChanged() // Do stuff here when health changes
    {
        if (currentHealth <= 0)
        {
            Die(lastDamageSource);
        }

        // When inheriting from this script, override this function and use the base, then add additional command such as pain animations
    }

    public virtual void TakeDamage(int damageAmount, DamageType damageSource)
    {
        currentHealth -= damageAmount;
        lastDamageSource = damageSource;
    }

    public virtual void Die(DamageType causeOfDeath)
    {
        Destroy(gameObject); // Destroy gameobject upon death, when using inherited classes override this function to implement different code such as death animations etc.)
    }
}
