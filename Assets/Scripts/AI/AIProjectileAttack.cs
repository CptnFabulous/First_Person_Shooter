using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;

public class AIProjectileAttack : NPCAttack
{
    [Header("Projectile")]
    public GunGeneralStats stats;

    [Header("Burst")]
    public int burstAmount;
    public float delayTelegraph;
    public float delayBetweenAttacks;
    
    [Header("Accuracy")]
    // Replace with variable value floats.
    public float aimSpeed;
    public float aimThreshold;
    public float telegraphAimModifier;

    float currentAimSpeed; // The speed the enemy is currently aiming at. 

    IEnumerator attackBeingExecuted;
    public Transform projectileOrigin;


    [Header("Cosmetics")]
    public UnityEvent effectsOnTelegraph;



    void Target(Entity e)
    {
        // If the enemy has found a target, and is able to attack them
        if (e != null && AIFunction.SimpleLineOfSightCheck(c.transform.position, e.transform.position, c.viewDetection))
        {
            Vector3 targetPosition = e.transform.position;
            
            c.LookTowards(targetPosition, currentAimSpeed);

            if (c.IsLookingAt(targetPosition, aimThreshold))
            {
                ExecuteAttack();
            }
        }
        else
        {
            if (attackBeingExecuted != null)
            {
                CancelAttack();
            }
        }
    }

    public override void ExecuteAttack()
    {
        if (attackBeingExecuted != null)
        {
            return;
        }
        
        currentAimSpeed = aimSpeed * telegraphAimModifier;
        attackBeingExecuted = AttackSequence();
        stats.StartCoroutine(attackBeingExecuted);
    }

    public IEnumerator AttackSequence()
    {
        // Play telegraph effects
        effectsOnTelegraph.Invoke();

        Vector3 direction = c.target.transform.position - c.transform.position;
        AttackMessage m = AttackMessage.Ranged(c.characterData, c.head.position, direction, range, stats.projectilePrefab.diameter, stats.projectileSpread, stats.projectilePrefab.velocity, stats.projectilePrefab.hitDetection);
        EventObserver.TransmitAttack(m); // Transmits a message of the attack the player is about to perform

        float delayTime = delay;

        

        for (int i = 0; i < burstAmount; i++)
        {
            yield return new WaitForSeconds(delayTime);

            // Attack!

            delayTime = delayBetweenAttacks;
        }

        CancelAttack();
    }

    public void CancelAttack()
    {
        currentAimSpeed = aimSpeed;
        stats.StopCoroutine(attackBeingExecuted);
        attackBeingExecuted = null;
    }


}
