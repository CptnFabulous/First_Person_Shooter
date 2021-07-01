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
    float cooldownTimer = float.MaxValue;

    [Header("Accuracy stats")]
    // Replace with variable value floats.
    public float aimDegreesPerSecond = 120;
    public float aimAngleThreshold = 0.2f;
    public float aimMultiplierWhileTelgraphing = 0;
    public float aimMultiplierWhileAttacking = 0;
    float currentAimDegreesPerSecond; // The speed the enemy is currently aiming at. 

    [Header("Attack")]
    public LayerMask hitDetection = ~0;
    //public GunGeneralStats stats;
    //public Transform projectileOrigin;

    [Header("Cosmetics")]
    public UnityEvent onTelegraph;
    public UnityEvent onAttack;
    public UnityEvent onAttackEnd;
    public UnityEvent onCooldownFinished;

    public void AttackUpdate()
    {
        if (wielder.currentAttack == null)
        {
            cooldownTimer += Time.deltaTime;
        }

        // If the enemy has found a target, and is able to attack them
        if (wielder.currentTarget != null && AIFunction.LineOfSight(wielder.head.position, wielder.currentTarget.transform, wielder.viewDetection))
        {
            Vector3 targetPosition = wielder.currentTarget.transform.position;
            currentAimDegreesPerSecond = aimDegreesPerSecond;
            wielder.LookTowards(targetPosition, currentAimDegreesPerSecond);

            // If the AI is successfully aiming at the target and their attack is off cooldown
            if (wielder.IsLookingAt(targetPosition, aimAngleThreshold) && cooldownTimer >= cooldown)
            {
                ExecuteAttack();
            }
        }
        else
        {
            // If the enemy is unable to directly attack the target, cancel it
            if (wielder.currentAttack != null)
            {
                CancelAttack();
            }

            wielder.head.localRotation = Quaternion.Euler(0, 0, 0); // Head position is reset
        }
    }

    void ExecuteAttack()
    {
        if (wielder.currentAttack != null)
        {
            return;
        }

        cooldownTimer = 0;
        wielder.currentAttack = AttackSequence();
        StartCoroutine(wielder.currentAttack);
    }

    IEnumerator AttackSequence()
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
