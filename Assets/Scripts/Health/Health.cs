using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public Resource health = new Resource { max = 100, current = 100, critical = 20 };

    DamageType lastDamageSource;
    GameObject lastAttacker;
    bool isDead;

#if UNITY_EDITOR
    void Reset() { OnValidate(); }
    void OnValidate()
    {
        health.current = Mathf.Clamp(health.current, 0, health.max);
    }
#endif

    /*
    // Start is called before the first frame update
    void Start()
    {
        
    }
    */

    // Update is called once per frame
    public virtual void Update()
    {
        if (health.current <= 0)
        {
            Die(lastDamageSource, lastAttacker); // Die function runs multiple times when it must only run once, this needs fixing
        }
    }

    public virtual void TakeDamage(int damageAmount, GameObject origin, DamageType damageSource)
    {
        health.current -= damageAmount;
        lastAttacker = origin;
        lastDamageSource = damageSource;
    }

    public virtual void Die(DamageType causeOfDeath, GameObject lastAttacker)
    {
        print(name + " has died");
        isDead = true;
        //GameEvent.TransmitKill(lastAttacker.GetComponent<Character>(), GetComponent<Character>(), causeOfDeath);
        EventObserver.TransmitKill(lastAttacker.GetComponent<Character>(), GetComponent<Character>(), causeOfDeath);
    }

    public virtual bool IsAlive()
    {
        if (health.current > 0)
        {
            return true;
        }
        return false;
    }
}
