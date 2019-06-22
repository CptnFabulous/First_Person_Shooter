using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    //public Animator a;

    public int maxHealth = 100;
    public int currentHealth = 100;
    int prevHealth;

    int lastDamageIndex; // Used to calculate source of damage/death. This might need to be changed to an enum



    
    // Start is called before the first frame update
    void Start()
    {
        prevHealth = currentHealth;
    }

    // Update is called once per frame
    void Update()
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
            Die(lastDamageIndex);
        }

        // When inheriting from this script, override this function and use the base, then add additional command such as pain animations
    }

    public virtual void TakeDamage(int damageAmount, int damageIndex)
    {
        currentHealth -= damageAmount;
        lastDamageIndex = damageIndex;
    }

    public virtual void Die(int deathIndex)
    {
        Destroy(gameObject); // Destroy gameobject upon death, when using inherited classes override this function to implement different code such as death animations etc.)
        /*
        a.SetInteger("DeathType",deathIndex);

        Upon death, 
        */
    }
}
