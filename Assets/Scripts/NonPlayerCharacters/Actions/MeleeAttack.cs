using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class MeleeAttack
{
    [Header("Attack")]
    public int damage = 10;
    //public int attackCount = 1;
    public Transform hitDetectOrigin;
    public LayerMask hitDetection = 1;
    public DamageType damageType;

    [Header("Accuracy")]
    public float range = 50;
    public float aimSpeed = 50;
    public float telegraphAimSpeed = 10;
    public float targetThreshold = 0.2f;

    [Header("Timers")]
    //public float attackSpeedPerMinute = 600;
    public float delay = 0.5f;
    public float cooldown = 0.5f;

    [Header("Other stats")]
    public float telegraphMoveSpeed;

    //float attackTimer = float.MaxValue;
    float delayTimer;
    float cooldownTimer = float.MaxValue;
    //int attackCounter;
    Vector3 aimMarker;
    [HideInInspector] public bool isAttacking;
    
    [Header("Cosmetics")]
    public AudioClip delayNoise;
    public AudioClip swingNoise;
    public AudioClip hitNoise;
    
    public void TargetEnemy(GameObject target, GameObject characterAttacking, NavMeshAgent na, float standardMoveSpeed, Faction characterFaction, Transform head, RaycastHit lookingAt, AudioSource audioSource)
    {
        if (isAttacking == false) // If attack has not been initiated, aim at target to start attacking
        {
            na.speed = standardMoveSpeed;

            cooldownTimer += Time.deltaTime;

            if (Physics.Raycast(head.position, target.transform.position - head.position, out lookingAt, range, hitDetection) && lookingAt.collider.gameObject == target) // Checks for line of sight between enemy and object
            {
                if (Vector3.Distance(aimMarker, target.transform.position) <= targetThreshold && cooldownTimer >= cooldown) // If aimMarker has reached target (i.e. NPC has aimed at target) and attack cooldown has finished
                {
                    // Initiate attack sequence
                    isAttacking = true;
                    delayTimer = 0;
                    //attackTimer = 60 / attackSpeedPerMinute;
                    //attackCounter = 0;

                    // Do cosmetic stuff for telegraph
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
                //audioSource.PlayOneShot(swingNoise);

                if (Physics.Raycast(hitDetectOrigin.position, head.forward, out lookingAt, range, hitDetection))
                {
                    Damage.PointDamage(characterAttacking, characterFaction, lookingAt.collider.gameObject, damage, damageType, false);
                }

                EndAttack(); // End attack

                /*
                if (attackCounter < attackCount || attackCount <= 0)
                {
                    attackTimer += Time.deltaTime;
                    if (attackTimer >= 60 / attackSpeedPerMinute)
                    {
                        audioSource.PlayOneShot(swingNoise);



                        attackTimer = 0;
                        attackCounter += 1;
                    }
                }
                else // If all shots have fired
                {
                    EndAttack(); // End attack
                }
                */
            }
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