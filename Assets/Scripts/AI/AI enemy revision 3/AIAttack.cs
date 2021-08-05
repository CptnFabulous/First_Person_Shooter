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
    float currentAimDegreesPerSecond; // The speed the enemy is currently aiming at.
    
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

        // Stores a bool that only needs to be altered if the following if statement returns true
        bool lineOfSight = false;
        // If the AI has an assigned target
        if (wielder.currentTarget != null)
        {
            // Determine the correct position for the AI to be aiming at
            Vector3 targetPosition = DetermineEnemyPosition();

            // Perform a line of sight check, making sure to ignore the AI and target's hitboxes since those obviously aren't obstacles
            List<DamageHitbox> wielderAndTargetHitboxes = new List<DamageHitbox>(wielder.hp.hitboxes);
            wielderAndTargetHitboxes.AddRange(new List<DamageHitbox>(wielder.currentTarget.HealthData.hitboxes));
            lineOfSight = AIFunction.LineOfSightCheckWithExceptions(targetPosition, wielder.head.position, wielder.viewDetection, wielderAndTargetHitboxes.ToArray());
            if (lineOfSight)
            {
                Debug.Log("aiming");
                // Aim for player
                currentAimDegreesPerSecond = aimDegreesPerSecond;
                wielder.RotateLookTowards(targetPosition, currentAimDegreesPerSecond);
                // If the AI is successfully aiming at the target and their attack is off cooldown
                if (wielder.AngleIsLookingAt(targetPosition, aimAngleThreshold) && cooldownTimer >= cooldown)
                {
                    ExecuteAttack();
                }

                /*
                currentAimUnitsPerSecond = aimUnitsPerSecond;
                wielder.TrackLookTowards(targetPosition, currentAimUnitsPerSecond);
                // If the AI is successfully aiming at the target and their attack is off cooldown
                if (wielder.DistanceIsLookingAt(targetPosition, aimDistanceThreshold) && cooldownTimer >= cooldown)
                {
                    ExecuteAttack();
                }
                */

                Debug.DrawLine(wielder.head.transform.position, targetPosition, Color.magenta);
            }
        }

        if (wielder.currentTarget == null || lineOfSight == false)
        {
            if (wielder.currentAttack != null)
            {
                CancelAttack();
            }

            wielder.ResetLookDirection();
        }

        /*
        // If the enemy has found a target, and is able to attack them
        //if (wielder.currentTarget != null && AIFunction.LineOfSight(wielder.head.position, wielder.currentTarget.transform, wielder.viewDetection))
        bool lineOfSight = AIFunction.LineOfSightCheckWithExceptions(wielder.currentTarget.transform.position, wielder.head.position, wielder.viewDetection, wielder.hp.hitboxes);
        Debug.Log("Line of sight = " + lineOfSight);
        if (wielder.currentTarget != null && lineOfSight)
        {
            Vector3 targetPosition = DetermineEnemyPosition();

            currentAimDegreesPerSecond = aimDegreesPerSecond;
            wielder.RotateLookTowards(targetPosition, currentAimDegreesPerSecond);
            // If the AI is successfully aiming at the target and their attack is off cooldown
            if (wielder.AngleIsLookingAt(targetPosition, aimAngleThreshold) && cooldownTimer >= cooldown)
            {
                ExecuteAttack();
            }
        }
        else
        {
            //Debug.Log(wielder.name + " is cancelling their attack on " + wielder.currentTarget.name + " on frame " + Time.frameCount);
            // If the enemy is unable to directly attack the target, cancel it
            if (wielder.currentAttack != null)
            {
                CancelAttack();
            }

            wielder.ResetLookDirection();
        }
        */
    }

    public virtual Vector3 DetermineEnemyPosition()
    {
        return TargetEnemyWholeBody(wielder.currentTarget.GetComponent<Health>());
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

    public virtual void Telegraph()
    {
        onTelegraph.Invoke();
    }

    public virtual void TheAttackItself()
    {
        onAttack.Invoke();
    }

    public void CancelAttack()
    {
        //currentAimDegreesPerSecond = aimDegreesPerSecond;
        currentAimUnitsPerSecond = aimUnitsPerSecond;
        StopCoroutine(wielder.currentAttack);
        wielder.currentAttack = null;
        onAttackEnd.Invoke();
    }


    // Gets the world position of the centre of the enemy's colliders. Ensures that if the target's pivot point is not in the centre of the collider, that the AI will still shoot at the colliders and not the pivot point.
    static Vector3 TargetEnemyWholeBody(Health health)
    {
        // Specifies a default centre in case there aren't any colliders for some reason
        Vector3 centre = health.transform.position;

        // Gets all the target's damageable hitboxes. If there are, start determining collider bounds.
        DamageHitbox[] enemyHitboxes = health.hitboxes;
        if (enemyHitboxes.Length > 0)
        {
            // Get the collider of the first hitbox in the list, and store its bounds.
            Bounds totalTargetableBounds = enemyHitboxes[0].Collider.bounds;
            // A standard for loop, skips zero because there is no existing bounds for the first one to be compared to so it is automatically assigned first.
            for (int i = 1; i < enemyHitboxes.Length; i++)
            {
                // For each new collider, check their minimum and maximum extents against the main bounds, and expand to encompass them if necessary.
                Bounds b = enemyHitboxes[i].Collider.bounds;
                totalTargetableBounds.min = Vector3.Min(totalTargetableBounds.min, b.min);
                totalTargetableBounds.max = Vector3.Max(totalTargetableBounds.max, b.max);
            }

            centre = totalTargetableBounds.center;
        }

        return centre;
    }

    // Gets the centre of a particular collider for the enemy to aim at.
    static Vector3 TargetEnemySpecificCollider(DamageHitbox dh)
    {
        return dh.Collider.bounds.center;
    }
}
