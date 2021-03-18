using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[System.Serializable]
public class NPCAttack : NPCAction
{
    [Header("Character")]
    [HideInInspector] public Combatant c;
    public float telegraphMoveSpeed;
    public float attackMoveSpeed;
    float previousMoveSpeed;

    [Header("Hit detection")]
    public LayerMask hitDetection = ~0;
    //public Vector2 attackAngles;

    [Header("Aiming")]
    public float range = 50;
    public float aimSpeed = 50;
    public float telegraphAimSpeed = 10;
    public float targetThreshold = 0.2f;
    [HideInInspector] public Vector3 aimMarker;
    [HideInInspector] public bool isAttacking;

    [Header("Timers")]
    public float attacksPerMinute = 100;
    public float delay = 0.5f;
    public float cooldown = 0.5f;
    float attackTimer = float.MaxValue;
    float delayTimer;
    float cooldownTimer = float.MaxValue;
    int attackCounter;

    [Header("Telegraph")]
    public UnityEvent onTelegraph;

    [Header("Attack")]
    public UnityEvent onAttack;
    public DamageType damageType;
    public int attackCount = 1;

    public void Update()
    {
        if (isAttacking == false) // If attack has not been initiated, aim at target to start attacking
        {
            cooldownTimer += Time.deltaTime;

            if (AIFunction.LineOfSight(c.head.position, c.target.transform, hitDetection)) // If the NPC can see the target
            {
                if (Vector3.Distance(aimMarker, c.target.transform.position) <= targetThreshold && cooldownTimer >= cooldown) // If aimMarker has reached target (i.e. NPC has aimed at target) and attack cooldown has finished
                {
                    #region Begin telegraph
                    isAttacking = true; // Enables isAttacking, to start the telegraph/attacking code on the next frame.
                    delayTimer = 0; // Resets timer for the telegraph duration
                    attackTimer = 60 / attacksPerMinute; // Resets timer for each individual attack
                    attackCounter = 0; // Resets counter for the amount of attacks
                    previousMoveSpeed = c.na.speed; // Stores the move speed of the agent prior to executing the attack
                    c.na.speed = telegraphMoveSpeed; // The agent's speed is adjusted while it telegraphs.
                    onTelegraph.Invoke(); // Invokes events that occur on telegraph, e.g. telegraph animation
                    TelegraphAttack(); // Runs additional code for telegraph
                    #endregion
                }
                else
                {
                    aimMarker = Vector3.MoveTowards(aimMarker, c.target.transform.position, aimSpeed * Time.deltaTime); // If enemy has not acquired target, move aimMarker towards target
                }

                c.head.LookAt(aimMarker); // Aims head towards aimMarker
            }
            else
            {
                aimMarker = c.transform.position; // If line of sight has not been acquired, NPC cannot aim at enemy, aim is at ease
                c.head.localRotation = Quaternion.Euler(0, 0, 0); // Head position is reset
            }
        }
        else // If attack sequence is initiated (isAttacking == true), execute telegraph and attack
        {
            aimMarker = Vector3.MoveTowards(aimMarker, c.target.transform.position, telegraphAimSpeed * Time.deltaTime); // Continue moving aimMarker at different speed for when the enemy is telegraphing
            c.head.LookAt(aimMarker); // Aims head towards aimMarker
            delayTimer += Time.deltaTime; // Delay timer counts up

            if (delayTimer >= delay) // When delay is finished, start attack
            {
                c.na.speed = attackMoveSpeed; // Change move speed again

                if (attackCounter < attackCount || attackCounter <= 0)
                {
                    attackTimer += Time.deltaTime;
                    if (attackTimer >= 60 / attacksPerMinute)
                    {
                        onAttack.Invoke();
                        attackTimer = 0;
                        attackCounter += 1;
                        ExecuteAttack();
                    }
                }
                else // If all shots have fired
                {
                    EndAttack(); // End attack
                }
            }
        }

        if (Vector3.Distance(c.transform.position, c.target.transform.position) > range)
        {
            EndAttack();
        }
    }

    public virtual void TelegraphAttack() // Initiate attack sequence
    {
        
    }

    public virtual void ExecuteAttack()
    {
        
    }

    public void EndAttack()
    {
        isAttacking = false;
        attackCounter = 0;
        cooldownTimer = 0;
        c.na.speed = previousMoveSpeed;
    }
}
