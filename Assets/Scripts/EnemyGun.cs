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
    [Header("Projectile")]
    public Projectile projectile;
    public float velocity;
    public float diameter;
    public float gravityMultiplier;
    public float range;
    public LayerMask hitDetection;

    [Header("Timers")]
    public float fireRate;
    public float delay;
    public float cooldown;

    // MAKE PRIVATE LATER
    public float fireTimer;
    public float delayTimer;
    public float cooldownTimer;
}

[RequireComponent(typeof (NavMeshAgent))]
public class EnemyGun : MonoBehaviour
{
    NavMeshAgent na;

    public float movementSpeed;

    [Header("Targeting")]
    public GameObject head;
    public GameObject target;
    public Transform aimMarker;
    public float aimSpeed;
    public float targetThreshold = 0.1f;
    RaycastHit lookingAt;
    bool isAttacking;

    [Header("Attacks")]
    public ProjectileAttack attack;

    


    private void Awake()
    {
        na = GetComponent<NavMeshAgent>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        TargetEnemy(attack, target);
    }

    void TargetEnemy(ProjectileAttack p, GameObject target)
    {
        if (Physics.Raycast(head.transform.position, target.transform.position - head.transform.position, out lookingAt, p.range, p.hitDetection) && lookingAt.collider.gameObject == target)
        {
            if (Vector3.Distance(aimMarker.position, target.transform.position) <= targetThreshold)
            {
                isAttacking = true;
            }
            else
            {
                aimMarker.position = Vector3.MoveTowards(aimMarker.position, target.transform.position, aimSpeed * Time.deltaTime);
            }
        }
        else
        {
            aimMarker.position = transform.position;
        }

        /*
        if (isAttacking == true)
        {
            p.delayTimer += Time.deltaTime;
            if (p.delayTimer >= p.delay)
            {
                for (int i = 0; i < p.projectileCount; i++)
                {

                }


                isAttacking = false;
                p.delayTimer = 0;
            }
        }
        print(p.delayTimer);
        */

    }
}
