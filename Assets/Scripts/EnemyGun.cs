using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class ProjectileAttack
{
    [Header("Damage")]
    public int damage = 10;
    public int projectileCount = 1;
    public int burstAmount = 3;

    [Header("Accuracy")]
    public float spread;
    public float range;

    [Header("Projectile")]
    public Projectile projectile;
    public float velocity;
    public float diameter;
    public float gravityMultiplier;
    public LayerMask hitDetection;
    public Transform projectileOrigin;

    [Header("Timers")]
    public float roundsPerMinute;
    public float delay;
    public float cooldown;

    float fireTimer = float.MaxValue;
    float delayTimer;
    float cooldownTimer = float.MaxValue;
    int burstCounter;
    Vector3 aimMarker;
    [HideInInspector] public bool isAttacking;

    

    public void TargetEnemy(GameObject target, GameObject characterAttacking, GameObject head, Transform a, float targetThreshold, float aimSpeed, RaycastHit lookingAt)
    {
        if (isAttacking == false) // If attack has not been initiated, aim at target to start attacking
        {
            cooldownTimer += Time.deltaTime;
            if (Physics.Raycast(head.transform.position, target.transform.position - head.transform.position, out lookingAt, range, hitDetection) && lookingAt.collider.gameObject == target) // Checks for line of sight between enemy and object
            {
                if (Vector3.Distance(aimMarker, target.transform.position) <= targetThreshold && cooldownTimer >= cooldown) // If aimMarker has reached target (i.e. NPC has aimed at target) and attack cooldown has finished
                {
                    // Initiate attack sequence
                    #region Initiate attack
                    Vector3 characterAimDirection = new Vector3(aimMarker.x, characterAttacking.transform.position.y, aimMarker.z);
                    characterAttacking.transform.LookAt(characterAimDirection);

                    //aimMarker = target.transform.position;

                    isAttacking = true;
                    delayTimer = 0;
                    fireTimer = 60 / roundsPerMinute;
                    burstCounter = 0;
                    Debug.Log("Attack sequence initiated");
                    #endregion
                }
                else
                {
                    // This line of code should not be running after the aimMarker has reached the target, and before the attack has finished.
                    aimMarker = Vector3.MoveTowards(aimMarker, target.transform.position, aimSpeed * Time.deltaTime); // If enemy has not acquired target, move aimMarker towards target
                }
            }
            else
            {
                aimMarker = characterAttacking.transform.position; // If line of sight has not been acquired, NPC cannot aim at enemy, aim is at ease
            }
        }
        else // If attack sequence is initiated (isAttacking == true), execute telegraph and attack
        {
            #region Attack sequence
            delayTimer += Time.deltaTime;
            if (delayTimer >= delay) // If delay is finished
            {
                #region Execute burst
                if (burstCounter < burstAmount)
                {
                    fireTimer += Time.deltaTime;
                    if (fireTimer >= 60 / roundsPerMinute)
                    {
                        #region Fire weapon
                        for (int _p = 0; _p < projectileCount; _p++)
                        {
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
                        }

                        fireTimer = 0;
                        #endregion

                        burstCounter += 1;
                    }
                }
                else // If all shots have fired
                {
                    EndAttack(); // End attack
                }
                #endregion
            }
            #endregion
        }

        a.position = aimMarker;
    }

    public void EndAttack()
    {
        isAttacking = false;
        burstCounter = 0;
        cooldownTimer = 0;
    }
}

[RequireComponent(typeof (NavMeshAgent))]
public class EnemyGun : MonoBehaviour
{
    NavMeshAgent na;

    public float movementSpeed;

    [Header("Targeting")]
    public GameObject head;
    public GameObject target;
    public Transform a;
    public float aimSpeed;
    public float targetThreshold = 0.1f;
    RaycastHit lookingAt;

    [Header("Attacks")]
    public ProjectileAttack attack;

    private void Awake()
    {
        na = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {

        if (target != null)
        {
            if (Vector3.Distance(gameObject.transform.position, target.transform.position) > attack.range)
            {
                na.destination = target.transform.position;
            }

            if (attack.isAttacking == true)
            {
                na.enabled = false;
            }
            else
            {
                na.enabled = true;
            }

            attack.TargetEnemy(target, gameObject, head, a, targetThreshold, aimSpeed, lookingAt);
        }
    }
}
