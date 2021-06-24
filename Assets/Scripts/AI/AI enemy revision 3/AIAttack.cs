using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;

public abstract class AIAttack : MonoBehaviour
{
    public AICombatant wielder;

    [Header("Attack timings")]
    public int attacksInBurst = 3;
    public float attacksPerMinute = 600;
    public float telegraphLength = 0.5f;
    public float cooldown = 1;

    [Header("Accuracy stats")]
    // Replace with variable value floats.
    public float aimDegreesPerSecond = 120;
    public float aimAngleThreshold = 0.2f;
    public float aimMultiplierWhileTelgraphing = 0;
    public float aimMultiplierWhileAttacking = 0;
    float currentAimDegreesPerSecond; // The speed the enemy is currently aiming at. 

    [Header("Attack")]
    //public GunGeneralStats stats;
    //public Transform projectileOrigin;

    [Header("Cosmetics")]
    public UnityEvent onTelegraph;
    public UnityEvent onAttack;
    public UnityEvent onAttackEnd;
    public UnityEvent onCooldownFinished;

    void Target(Entity e)
    {
        // If the enemy has found a target, and is able to attack them
        if (e != null && AIFunction.SimpleLineOfSightCheck(wielder.transform.position, e.transform.position, wielder.viewDetection))
        {
            Vector3 targetPosition = e.transform.position;

            currentAimDegreesPerSecond = aimDegreesPerSecond;
            wielder.LookTowards(targetPosition, currentAimDegreesPerSecond);

            if (wielder.IsLookingAt(targetPosition, aimAngleThreshold))
            {
                ExecuteAttack();
            }
        }
        else
        {
            if (wielder.currentAttack != null)
            {
                CancelAttack();
            }
        }
    }

    public void ExecuteAttack()
    {
        
        if (wielder.currentAttack != null)
        {
            return;
        }
        
        wielder.currentAttack = AttackSequence();
        StartCoroutine(wielder.currentAttack);
    }

    public IEnumerator AttackSequence()
    {
        Telegraph();
        
        currentAimDegreesPerSecond = aimDegreesPerSecond * aimMultiplierWhileTelgraphing;

        yield return new WaitForSeconds(telegraphLength);

        currentAimDegreesPerSecond = aimDegreesPerSecond * aimMultiplierWhileAttacking;

        for (int i = 0; i < attacksInBurst; i++)
        {
            TheAttackItself();
            
            if (i < attacksInBurst - 1)
            {
                yield return new WaitForSeconds(60 / attacksPerMinute);
            }
        }

        CancelAttack();
    }

    public virtual void TheAttackItself()
    {
        onAttack.Invoke();
    }

    public virtual void Telegraph()
    {
        onTelegraph.Invoke();
        
    }

    public void CancelAttack()
    {
        currentAimDegreesPerSecond = aimDegreesPerSecond;
        StopCoroutine(wielder.currentAttack);
        wielder.currentAttack = null;
        onAttackEnd.Invoke();
    }
}
