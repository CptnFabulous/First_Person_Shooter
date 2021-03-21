using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EntityAttack : MonoBehaviour
{
    public FightingAI user;





    [Header("Attack stats")]

    public float attacksPerMinute = 500;
    public float minRange = 0;
    public float maxRange = 400;
    [Tooltip("If set to zero, there is no burst - the enemy will continue attacking until the target is out of their aim.")]
    public int attacksInBurst = 3;
    public LayerMask hitDetection = ~0;
    

    [Header("Aiming and telegraphing")]
    public bool areaOfEffectAndNotDirectional;
    public float aimConfirmThreshold;
    public float fireBreakThreshold;

    public PercentageModifier aimSpeedWhileTelegraphing;
    public PercentageModifier aimSpeedWhileAttacking;
    public float telegraphDelay = 0.5f;
    public float cooldownTime = 1;

    [Header("Cosmetics")]
    public UnityEvent telegraphCosmeticEffects;
    public UnityEvent attackCosmeticEffects;
    public UnityEvent endCosmeticEffects;
    public UnityEvent cancelCosmeticEffects;




    public virtual void TheAttackItself()
    {
        attackCosmeticEffects.Invoke();
    }

    


    // Attacks continuously until the player cannot be hit anymore
    public IEnumerator AttackContinuously(float delay, float breakThreshold, float cooldown)
    {
        // Create telegraph
        telegraphCosmeticEffects.Invoke();
        // Modify aim sensitivity while telegraphing

        yield return new WaitForSeconds(delay);

        // Creates an int to count the amount of bursts, and a float to count the time between attacks
        float attackTimer = 0;
        // Modify aim sensitivity while attacking


        // We've made a miniature update loop inside the IEnumerator. This will keep the loop running until burstCounter runs out.
        while (CanHit(breakThreshold))
        {
            attackTimer += Time.deltaTime;
            if (attackTimer > attacksPerMinute / 60)
            {
                TheAttackItself();
                attackTimer = 0;
            }

            yield return new WaitForEndOfFrame();
        }

        EndAttack();
    }

    // Attacks a set number of times
    public IEnumerator BurstAttack(int burstAmount, float delay, float cooldown)
    {
        // Create telegraph;
        telegraphCosmeticEffects.Invoke();
        // Modify aim sensitivity while telegraphing

        yield return new WaitForSeconds(delay);

        // Creates an int to count the amount of bursts, and a float to count the time between attacks
        int burstCounter = burstAmount;
        float attackTimer = 0;
        // Modify aim sensitivity while attacking


        // We've made a miniature update loop inside the IEnumerator. This will keep the loop running until burstCounter runs out.
        while (burstCounter > 0)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer > attacksPerMinute / 60)
            {
                TheAttackItself();
                attackTimer = 0;
                burstCounter -= 1;
            }

            yield return new WaitForEndOfFrame();
        }

        EndAttack();
    }

    void EndAttack()
    {
        StopCoroutine(user.attackInProgress);
        user.attackInProgress = null;
        // Start cooldown timer
        endCosmeticEffects.Invoke();
    }

    void CancelAttack()
    {
        StopCoroutine(user.attackInProgress);
        user.attackInProgress = null;
        // Start cooldown timer
        cancelCosmeticEffects.Invoke();
    }







    // Checks if the attack will hit (aimed in the right direction and not blocked by line of sight)
    public bool CanHit(float threshold)
    {
        // Checks if the target is within an appropriate range
        float distanceToTarget = Vector3.Distance(user.head.position, user.target.transform.position);
        if (distanceToTarget >= minRange && distanceToTarget <= maxRange)
        {
            // Checks that the attack will not be blocked (I might need to make this more substantial)
            if (AIFunction.SimpleLineOfSightCheck(user.head.position, user.target.transform.position, hitDetection))
            {
                bool checkDistanceNotAngle = areaOfEffectAndNotDirectional;
                if (!checkDistanceNotAngle) // If checking angle (e.g. for a conventional directional attack such as a gunshot or melee attack)
                {
                    Vector3 aimDirection = user.head.forward;
                    Vector3 playerDirection = user.target.transform.position - user.head.position;
                    if (Vector3.Angle(aimDirection, playerDirection) <= threshold)
                    {
                        return true;
                    }
                }
                else // if checking the attack point's distance from the target (e.g. for an area of effect attack)
                {
                    // Figures out how far away the target is.
                    //float distanceToTarget = Vector3.Distance(user.head.position, target.transform.position);
                    // Creates a new position that's an equivalent distance away, but in the direction the user is looking
                    Vector3 aimPoint = user.head.forward * distanceToTarget;

                    if (Vector3.Distance(aimPoint, user.target.transform.position) <= threshold)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }


    /*
    // Checks if the 
    bool WithinAngleThreshold(float angle)
    {
        Vector3 aimDirection = user.head.forward;
        Vector3 playerDirection = target.transform.position - user.head.position;

        if (Vector3.Angle(aimDirection, playerDirection) <= angle)
        {
            return true;
        }

        return false;
    }

    // Produces a position to initiate an area of effect attack, that is to the side of the target.
    Vector3 AimPositionForAreaOfEffect()
    {
        // Figures out how far away the target is.
        float distanceToTarget = Vector3.Distance(user.head.position, target.transform.position);
        // Creates a new position that's an equivalent distance away, but in the direction the user is looking
        Vector3 aimPoint = user.head.forward * distanceToTarget;

        return aimPoint;
    }

    bool WithinDistanceThreshold(Vector3 aimPoint, float distance)
    {
        if (Vector3.Distance(aimPoint, target.transform.position) <= distance)
        {
            return true;
        }
        return false;
    }
    */
}
