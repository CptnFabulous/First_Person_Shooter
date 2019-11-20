using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class ProjectileBurst
{
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

    [Header("Other stats")]
    public float telegraphMoveSpeed;

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

    public void TargetEnemy(GameObject target, GameObject characterAttacking, NavMeshAgent na, float standardMoveSpeed, Faction characterFaction, Transform head, RaycastHit lookingAt, AudioSource audioSource)
    {
        if (isAttacking == false) // If attack has not been initiated, aim at target to start attacking
        {
            na.speed = standardMoveSpeed;

            cooldownTimer += Time.deltaTime;

            if (Physics.Raycast(head.position, target.transform.position - head.position, out lookingAt, range, projectile.hitDetection) && lookingAt.collider.gameObject == target) // Checks for line of sight between enemy and object
            {
                if (Vector3.Distance(aimMarker, target.transform.position) <= targetThreshold && cooldownTimer >= cooldown) // If aimMarker has reached target (i.e. NPC has aimed at target) and attack cooldown has finished
                {
                    // Initiate attack sequence
                    isAttacking = true;
                    delayTimer = 0;
                    fireTimer = 60 / roundsPerMinute;
                    burstCounter = 0;

                    //telegraphNoise.Play();
                }
                else
                {
                    aimMarker = Vector3.MoveTowards(aimMarker, target.transform.position, aimSpeed * Time.deltaTime); // If enemy has not acquired target, move aimMarker towards target
                }

                head.LookAt(aimMarker);
            }
            else
            {
                aimMarker = characterAttacking.transform.position; // If line of sight has not been acquired, NPC cannot aim at enemy, aim is at ease
                head.localRotation = Quaternion.Euler(0, 0, 0);
            }
        }
        else // If attack sequence is initiated (isAttacking == true), execute telegraph and attack
        {
            na.speed = telegraphMoveSpeed;

            aimMarker = Vector3.MoveTowards(aimMarker, target.transform.position, telegraphAimSpeed * Time.deltaTime);
            head.LookAt(aimMarker);


            delayTimer += Time.deltaTime;
            if (delayTimer >= delay) // If delay is finished
            {
                if (burstCounter < burstAmount || burstAmount <= 0)
                {
                    fireTimer += Time.deltaTime;
                    if (fireTimer >= 60 / roundsPerMinute)
                    {
                        muzzleFlash.Play();
                        audioSource.PlayOneShot(firingNoise);

                        for (int _p = 0; _p < projectileCount; _p++)
                        {
                            Damage.ShootProjectile(projectile, spread, range, characterAttacking, characterFaction, head, projectileOrigin, head.forward);
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

        if (Vector3.Distance(characterAttacking.transform.position, target.transform.position) > range)
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
    }
}