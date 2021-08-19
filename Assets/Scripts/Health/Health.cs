using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Health : MonoBehaviour
{
    public Resource health = new Resource(100, 100, 20);

    DamageType lastDamageSource;
    Entity lastAttacker;
    bool isDead;
    public DamageHitbox[] hitboxes;

#if UNITY_EDITOR
    void Reset() { OnValidate(); }
    void OnValidate()
    {
        health.current = Mathf.Clamp(health.current, 0, health.max);
    }
#endif

    public virtual void Awake()
    {
        hitboxes = GetComponentsInChildren<DamageHitbox>();
        foreach(DamageHitbox dh in hitboxes)
        {
            dh.healthScript = this;
        }
    }


    // Update is called once per frame
    public virtual void Update()
    {
        if (health.current <= 0)
        {
            Die(lastDamageSource, lastAttacker); // Die function runs multiple times when it must only run once, this needs fixing
        }
    }
    






    















    public virtual void TakeDamage(int damageAmount, Entity origin, DamageType damageSource)
    {
        health.current -= damageAmount;
        lastAttacker = origin;
        lastDamageSource = damageSource;
    }

    public virtual void Die(DamageType causeOfDeath, Entity lastAttacker)
    {
        if (isDead == false)
        {
            //print(name + " has died");
            isDead = true;
            EventObserver.TransmitKill(lastAttacker.GetComponent<Character>(), GetComponent<Character>(), causeOfDeath);
        }
        else
        {
            //print(name + "is already dead");
        }
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
