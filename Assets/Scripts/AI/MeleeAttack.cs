using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class MeleeAttack : NPCAction
{
    [Header("Character")]
    [HideInInspector] public Combatant c;
    public float telegraphMoveSpeed;
    public float attackMoveSpeed;
    float previousMoveSpeed;

    [Header("Attack")]
    public int damage = 10;
    public int attackCount = 1;
    public DamageType damageType;

    [Header("Hit detection")]
    public LayerMask hitDetection = ~0;
    public Vector2 attackAngles;


    [Header("Aiming")]
    public Transform hitDetectOrigin;
    public float range = 3;
    public float aimSpeed = 50;
    public float telegraphAimSpeed = 10;
    public float targetThreshold = 0.2f;
    Vector3 aimMarker;
    [HideInInspector] public bool isAttacking;

    [Header("Timers")]
    //public float attackSpeedPerMinute = 600;
    public float attacksPerMinute = 100;
    public float delay = 0.5f;
    public float cooldown = 0.5f;
    public float attackTimer = float.MaxValue;
    float delayTimer;
    float cooldownTimer = float.MaxValue;
    int attackCounter;
    

    

    public void Update()
    {
        if (isAttacking == false) // If attack has not been initiated, aim at target to start attacking
        {
            cooldownTimer += Time.deltaTime;

            if (AI.LineOfSight(c.head.position, c.target.transform, hitDetection))
            {
                // aim 
                if (Vector3.Distance(aimMarker, c.target.transform.position) <= targetThreshold && cooldownTimer >= cooldown) // If aimMarker has reached target (i.e. NPC has aimed at target) and attack cooldown has finished
                {
                    #region Initiate attack
                    // Initiate attack sequence
                    isAttacking = true;
                    delayTimer = 0;
                    attackTimer = 60 / attacksPerMinute;
                    attackCounter = 0;

                    previousMoveSpeed = c.na.speed; // Stores the move speed of the agent prior to executing the attack
                    c.na.speed = telegraphMoveSpeed; // The agent's speed is adjusted while it telegraphs.
                    //AttackMessage m = AttackMessage.Melee(c.c, c.head.position, aimMarker - hitDetectOrigin.position, range, projectileStats.diameter, spread, projectileStats.velocity, projectileStats.hitDetection);
                    EventObserver.TransmitAttack(m); // Transmits a message of the attack the player is about to perform
                    #endregion
                }
                else
                {
                    aimMarker = Vector3.MoveTowards(aimMarker, c.target.transform.position, aimSpeed * Time.deltaTime); // If enemy has not acquired target, move aimMarker towards target
                }

                c.head.LookAt(aimMarker);
            }
            else
            {
                aimMarker = c.transform.position; // If line of sight has not been acquired, NPC cannot aim at enemy, aim is at ease
                c.head.localRotation = Quaternion.Euler(0, 0, 0);
            }
        }
        else // If attack sequence is initiated (isAttacking == true), execute telegraph and attack
        {
            //c.na.speed = telegraphMoveSpeed;

            aimMarker = Vector3.MoveTowards(aimMarker, c.target.transform.position, telegraphAimSpeed * Time.deltaTime);
            c.head.LookAt(aimMarker);


            delayTimer += Time.deltaTime;
            if (delayTimer >= delay) // If delay is finished
            {
                c.na.speed = attackMoveSpeed;

                if (attackCounter < attackCount || attackCounter <= 0)
                {
                    attackTimer += Time.deltaTime;
                    if (attackTimer >= 60 / attacksPerMinute)
                    {
                        

                        //Damage.ShootProjectile(projectile, projectileCount, spread, range, c.c, c.head, projectileOrigin.position, aimMarker - c.head.position);
                        Damage.ShootProjectile(projectileStats, spread, range, c.c, c.head, projectileOrigin.position, aimMarker - c.head.position);

                        fireTimer = 0;
                        burstCounter += 1;
                    }
                }
                else // If all shots have fired
                {
                    EndAttack(); // End attack
                }
            }



            /*
            //c.na.speed = telegraphMoveSpeed;
            aimMarker = Vector3.MoveTowards(aimMarker, c.target.transform.position, telegraphAimSpeed * Time.deltaTime);
            c.head.LookAt(aimMarker);


            delayTimer += Time.deltaTime;
            if (delayTimer >= delay) // If delay is finished
            {
                //audioSource.PlayOneShot(swingNoise);

                if (Physics.Raycast(hitDetectOrigin.position, head.forward, out lookingAt, range, hitDetection))
                {
                    Damage.PointDamage(characterAttacking, characterFaction, lookingAt.collider.gameObject, damage, damageType, false);
                }

                EndAttack(); // End attack
            }
            */
        }

        if (Vector3.Distance(characterAttacking.transform.position, target.transform.position) > range)
        {
            EndAttack();
        }
    }

    public void EndAttack()
    {
        isAttacking = false;
        //attackCounter = 0;
        cooldownTimer = 0;
    }
}
