using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;

public abstract class AIAttack : MonoBehaviour
{
    public AICombatant wielder;

    [Header("Timings")]
    public int attacksInBurst = 3;
    public float attacksPerMinute = 600;
    public float telegraphLength = 0.5f;
    public float cooldown = 1;
    float cooldownTimer = float.MaxValue;

    [Header("Accuracy")] // Some of these variables might need to be replaced with variable value floats.
    public float aimDegreesPerSecond = 120;
    public float aimAngleThreshold = 0.2f;
    float currentAimDegreesPerSecond; // The speed the enemy is currently aiming at.
    public float aimMultiplierWhileTelgraphing = 0;
    public float aimMultiplierWhileAttacking = 0;
    public bool lockOntoTargetWorldPosition = true;
    Vector3 targetPosition;

    List<DamageHitbox> wielderAndTargetHitboxes;

    [Header("Movement")]
    public float movementMultiplierWhileTelegraphing = 0.5f;
    public float movementMultiplierWhileAttacking = 0.5f;
    [HideInInspector] public AIMovementBehaviour currentMovementBehaviour;

    [Header("Cosmetics")]
    public UnityEvent onTelegraph;
    public UnityEvent onAttackEnd;
    public UnityEvent onCooldownFinished;

    [Header("Attack")]
    public UnityEvent onAttack;
    public LayerMask hitDetection = ~0;

    public void StateStart()
    {
        wielderAndTargetHitboxes = new List<DamageHitbox>(wielder.hp.hitboxes);
        wielderAndTargetHitboxes.AddRange(new List<DamageHitbox>(wielder.currentTarget.HealthData.hitboxes));

        currentAimDegreesPerSecond = aimDegreesPerSecond;
        wielder.na.speed = currentMovementBehaviour.movementSpeed;
    }

    public void AttackUpdate()
    {
        // If the attack has ended, cool down
        if (wielder.currentAttack == null)
        {
            cooldownTimer += Time.deltaTime;
        }

        // Stores a bool that only needs to be altered if the following if statement returns true
        bool lineOfSight = false;
        // If the AI has an assigned target
        if (wielder.currentTarget != null)
        {
            // If the AI has not started telegraphing yet, or it is but has not locked a specific position to aim at
            if (wielder.currentAttack == null || lockOntoTargetWorldPosition == false)
            {
                targetPosition = DetermineEnemyPosition(); // Determine the correct position for the AI to be aiming at
            }

            // Perform a line of sight check, making sure to ignore the AI and target's hitboxes since those obviously aren't obstacles
            lineOfSight = AIFunction.LineOfSightCheckWithExceptions(targetPosition, wielder.LookOrigin, wielder.viewDetection, wielderAndTargetHitboxes.ToArray());
            if (lineOfSight)
            {
                // Aim for player
                wielder.RotateLookTowards(targetPosition, currentAimDegreesPerSecond);
                // If the AI is successfully aiming at the target and their attack is off cooldown
                if (wielder.IsLookingAt(targetPosition, aimAngleThreshold) && cooldownTimer >= cooldown)
                {
                    ExecuteAttack();
                }
            }
        }

        if (wielder.currentTarget == null || lineOfSight == false)
        {
            if (wielder.currentAttack != null)
            {
                CancelAttack();
            }
            wielder.ReturnToNeutralLookPosition();
        }
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

        //currentAimUnitsPerSecond = aimUnitsPerSecond * aimMultiplierWhileTelgraphing;
        currentAimDegreesPerSecond = aimDegreesPerSecond * aimMultiplierWhileTelgraphing;
        wielder.na.speed = currentMovementBehaviour.movementSpeed * movementMultiplierWhileTelegraphing;

        yield return new WaitForSeconds(telegraphLength);

        //currentAimUnitsPerSecond = aimUnitsPerSecond * aimMultiplierWhileAttacking;
        currentAimDegreesPerSecond = aimDegreesPerSecond * aimMultiplierWhileAttacking;
        wielder.na.speed = currentMovementBehaviour.movementSpeed * movementMultiplierWhileAttacking;

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

        //currentAimUnitsPerSecond = aimUnitsPerSecond;
        currentAimDegreesPerSecond = aimDegreesPerSecond;
        wielder.na.speed = currentMovementBehaviour.movementSpeed;
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
