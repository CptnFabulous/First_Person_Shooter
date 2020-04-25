using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class ProjectileAttack : NPCAction
{
    [Header("Character")]
    [HideInInspector] public Combatant c;
    public float telegraphMoveSpeed;
    public float attackMoveSpeed;
    float previousMoveSpeed;

    [Header("Projectile")]
    public ProjectileData projectile;
    public Transform projectileOrigin;
    public int projectileCount = 1;
    public int burstAmount = 1;

    [Header("Accuracy")]
    public float spread;
    public float range = 50;
    public float aimSpeed = 50;
    public float telegraphAimSpeed = 10;
    public float targetThreshold = 0.2f;

    [Header("Timers")]
    public float roundsPerMinute = 600;
    public float delay = 0.5f;
    public float cooldown = 0.5f;

    float fireTimer = float.MaxValue;
    float delayTimer;
    float cooldownTimer = float.MaxValue;
    int burstCounter;
    Vector3 aimMarker;
    [HideInInspector] public bool isAttacking;

    [Header("Cosmetics")]
    public LineRenderer laserSight;
    public TimedVisualEffect muzzleFlash;
    public float flashRelativeDuration = 2;
    public AudioClip delayNoise;
    public AudioClip firingNoise;

    public void Update()
    {
        if (isAttacking == false) // If attack has not been initiated, aim at target to start attacking
        {
            cooldownTimer += Time.deltaTime;

            if (AI.LineOfSight(c.head.position, c.target.transform, projectile.hitDetection))
            {
                Debug.Log("NPC has line of sight");
                if (Vector3.Distance(aimMarker, c.target.transform.position) <= targetThreshold && cooldownTimer >= cooldown) // If aimMarker has reached target (i.e. NPC has aimed at target) and attack cooldown has finished
                {
                    #region Initiate attack
                    // Initiate attack sequence
                    isAttacking = true;
                    delayTimer = 0;
                    fireTimer = 60 / roundsPerMinute;
                    burstCounter = 0;

                    previousMoveSpeed = c.na.speed; // Stores the move speed of the agent prior to executing the attack
                    c.na.speed = telegraphMoveSpeed; // The agent's speed is adjusted while it telegraphs.
                    EventObserver.TransmitAttack(c.c, c.target, range, projectile.velocity); // Transmits a message of the attack the player is about to perform

                    //telegraphNoise.Play();
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

                if (burstCounter < burstAmount || burstAmount <= 0)
                {
                    fireTimer += Time.deltaTime;
                    if (fireTimer >= 60 / roundsPerMinute)
                    {
                        muzzleFlash.Play();
                        c.audioOutput.PlayOneShot(firingNoise);

                        for (int _p = 0; _p < projectileCount; _p++)
                        {
                            Damage.ShootProjectile(projectile, spread, range, c.gameObject, c.c.faction, c.head, projectileOrigin, c.head.forward);
                        }

                        fireTimer = 0;
                        burstCounter += 1;
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

        laserSight.enabled = isAttacking;
    }

    public void EndAttack()
    {
        isAttacking = false;
        burstCounter = 0;
        cooldownTimer = 0;

        c.na.speed = previousMoveSpeed;
    }
}