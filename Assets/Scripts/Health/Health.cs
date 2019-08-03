using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth = 100;
    [Tooltip("The point where the player's health is considered critical. Percentage or HP value?")]
    public int criticalPercentage = 20;
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

        // DO ADDITIONAL STUFF HERE IN DERIVED CLASSES, e.g. pain/death animations
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
