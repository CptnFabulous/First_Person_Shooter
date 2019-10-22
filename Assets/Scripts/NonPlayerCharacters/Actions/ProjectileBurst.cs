﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProjectileBurst
{
    [Header("Damage")]
    public int damage = 10;
    public int projectileCount = 1;
    public int burstAmount = 1;

    [Header("Accuracy")]
    public float spread;
    public float range = 500;

    [Header("Projectile")]
    public Projectile projectile;
    public float velocity = 100;
    public float diameter = 0.1f;
    public float gravityMultiplier;
    public LayerMask hitDetection = 1;
    public Transform projectileOrigin;

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

    public void TargetEnemy(GameObject target, GameObject characterAttacking, GameObject head, float targetThreshold, float aimSpeed, RaycastHit lookingAt)
    {
        if (isAttacking == false) // If attack has not been initiated, aim at target to start attacking
        {
            cooldownTimer += Time.deltaTime;
            if (Physics.Raycast(head.transform.position, target.transform.position - head.transform.position, out lookingAt, range, hitDetection) && lookingAt.collider.gameObject == target) // Checks for line of sight between enemy and object
            {
                if (Vector3.Distance(aimMarker, target.transform.position) <= targetThreshold && cooldownTimer >= cooldown) // If aimMarker has reached target (i.e. NPC has aimed at target) and attack cooldown has finished
                {
                    // Initiate attack sequence
                    isAttacking = true;
                    delayTimer = 0;
                    fireTimer = 60 / roundsPerMinute;
                    burstCounter = 0;
                    Debug.Log("Attack sequence initiated");


                    //laserSight.SetPosition(1, (target.transform.position - head.transform.position) * Vector3.Distance(head.transform.position, target.transform.position));
                }
                else
                {
                    // This line of code should not be running after the aimMarker has reached the target, and before the attack has finished.
                    aimMarker = Vector3.MoveTowards(aimMarker, target.transform.position, aimSpeed * Time.deltaTime); // If enemy has not acquired target, move aimMarker towards target
                }

                head.transform.LookAt(aimMarker);
            }
            else
            {
                aimMarker = characterAttacking.transform.position; // If line of sight has not been acquired, NPC cannot aim at enemy, aim is at ease
                head.transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
        }
        else // If attack sequence is initiated (isAttacking == true), execute telegraph and attack
        {
            delayTimer += Time.deltaTime;
            if (delayTimer >= delay) // If delay is finished
            {
                if (burstCounter < burstAmount)
                {
                    fireTimer += Time.deltaTime;
                    if (fireTimer >= 60 / roundsPerMinute)
                    {
                        muzzleFlash.Restart(60 / roundsPerMinute * flashRelativeDuration);
                        AudioSource.PlayClipAtPoint(firingNoise, projectileOrigin.position);

                        for (int _p = 0; _p < projectileCount; _p++)
                        {
                            #region Shoot projectile
                            Vector3 destination = Quaternion.Euler(Random.Range(-spread, spread), Random.Range(-spread, spread), Random.Range(-spread, spread)) * (aimMarker - head.transform.position);
                            RaycastHit rh;
                            if (Physics.Raycast(head.transform.position, destination, out rh, range, hitDetection)) // To reduce the amount of superfluous variables, I re-used the 'target' Vector3 in the same function as it is now unneeded for its original purpose
                            {
                                destination = rh.point;
                            }
                            else
                            {
                                destination *= range;
                            }

                            GameObject launchedProjectile = Object.Instantiate(projectile.gameObject, projectileOrigin.position, Quaternion.LookRotation(destination - projectileOrigin.position, Vector3.up));
                            Projectile p = launchedProjectile.GetComponent<Projectile>();
                            p.velocity = velocity;
                            p.gravityMultiplier = gravityMultiplier;
                            p.diameter = diameter;
                            p.targetDetection = hitDetection;
                            p.origin = characterAttacking;
                            #endregion
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

        laserSight.enabled = isAttacking;
    }

    public void EndAttack()
    {
        isAttacking = false;
        burstCounter = 0;
        cooldownTimer = 0;
    }
}