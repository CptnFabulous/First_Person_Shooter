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
    /*
    public float aimDegreesPerSecond = 120;
    public float aimAngleThreshold = 0.2f;
    
    float currentAimDegreesPerSecond; // The speed the enemy is currently aiming at.
    */

    public float aimUnitsPerSecond = 50;
    public float aimDistanceThreshold = 0.1f;
    float currentAimUnitsPerSecond;
    public float aimMultiplierWhileTelgraphing = 0;
    public float aimMultiplierWhileAttacking = 0;

    [Header("Attack")]
    public LayerMask hitDetection = ~0;

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

            //currentAimDegreesPerSecond = aimDegreesPerSecond;
            //wielder.RotateLookTowards(targetPosition, currentAimDegreesPerSecond);
            currentAimUnitsPerSecond = aimUnitsPerSecond;
            wielder.TrackLookTowards(targetPosition, currentAimUnitsPerSecond);

            // If the AI is successfully aiming at the target and their attack is off cooldown
            //if (wielder.AngleIsLookingAt(targetPosition, aimAngleThreshold) && cooldownTimer >= cooldown)
            if (wielder.DistanceIsLookingAt(targetPosition, aimDistanceThreshold) && cooldownTimer >= cooldown)
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

            wielder.ResetLookDirection();
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

        //currentAimDegreesPerSecond = aimDegreesPerSecond * aimMultiplierWhileTelgraphing;
        currentAimUnitsPerSecond = aimUnitsPerSecond * aimMultiplierWhileTelgraphing;

        yield return new WaitForSeconds(telegraphLength);

        //currentAimDegreesPerSecond = aimDegreesPerSecond * aimMultiplierWhileAttacking;
        currentAimUnitsPerSecond = aimUnitsPerSecond * aimMultiplierWhileAttacking;

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
        //currentAimDegreesPerSecond = aimDegreesPerSecond;
        currentAimUnitsPerSecond = aimUnitsPerSecond;
        StopCoroutine(wielder.currentAttack);
        wielder.currentAttack = null;
        onAttackEnd.Invoke();
    }
}
