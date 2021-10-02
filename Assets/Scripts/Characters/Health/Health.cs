using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;



public class Health : MonoBehaviour
{
    public Resource values = new Resource(100, 100, 20);
    public UnityEvent onDamage;
    public UnityEvent onDeath;
    public UnityEvent onHeal;
    public DamageHitbox[] hitboxes;


    public bool IsDead { get; private set; }

    DamageType lastDamageSource;
    Entity lastAttacker;

#if UNITY_EDITOR
    void Reset() { OnValidate(); }
    void OnValidate()
    {
        values.current = Mathf.Clamp(values.current, 0, values.max);
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

    

    public virtual void Damage(int damageAmount, Entity origin, DamageType damageSource)
    {
        values.current -= damageAmount;
        lastAttacker = origin;
        lastDamageSource = damageSource;

        // Play appropriate event
        if (damageAmount >= 0)
        {
            onDamage.Invoke();
        }
        else
        {
            onHeal.Invoke();
        }

        if (values.current <= 0)
        {
            Die(lastDamageSource, lastAttacker);
        }
    }


    public virtual void Die(DamageType causeOfDeath, Entity lastAttacker)
    {
        if (IsDead)
        {
            return; // Character is already dead, no need to do anything
        }

        IsDead = true;
        onDeath.Invoke();
        EventObserver.TransmitKill(lastAttacker.GetComponent<Character>(), GetComponent<Character>(), causeOfDeath);

    }

    string DeathMessage(Entity sourceOfDeath, DamageType causeOfDeath)
    {
        string deathMessage = name + " was ";

        switch (causeOfDeath)
        {
            case DamageType.Shot:
                deathMessage += "shot to death";
                break;
            case DamageType.CriticalShot:
                deathMessage += "shot in the head";
                break;
            case DamageType.BlownUp:
                deathMessage += "blown up";
                break;
            case DamageType.Gibbed:
                deathMessage += "splattered to giblets";
                break;
            case DamageType.Burned:
                deathMessage += "burned to a crisp";
                break;
            case DamageType.Bludgeoned:
                deathMessage += "bludgeoned to a pulp";
                break;
            default:
                deathMessage += "killed";
                break;
        }

        deathMessage += " by ";


        if (sourceOfDeath == null)
        {
            deathMessage += " unknown circumstances!";
            return deathMessage;
        }

        if (sourceOfDeath.isUniquelyNamed == false)
        {
            string[] vowels = new string[]
            {
                "A", "E", "I", "O", "U", "a", "e", "i", "o", "u"
            };
            // Checks if the first letter of the name is a vowel
            bool startsWithVowel = false;
            for (int i = 0; i < vowels.Length; i++)
            {
                if (sourceOfDeath.name.StartsWith(vowels[i]))
                {
                    startsWithVowel = true;
                }
            }

            // Changes 
            if (startsWithVowel)
            {
                deathMessage += "an ";
            }
            else
            {
                deathMessage += "a ";
            }
        }

        deathMessage += sourceOfDeath.name + "!";

        return deathMessage;
    }
}
