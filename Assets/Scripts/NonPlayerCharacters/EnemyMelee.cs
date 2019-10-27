using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof (NavMeshAgent))]

public class EnemyMelee : MonoBehaviour
{
    NavMeshAgent na;

    public GameObject head;

    public GameObject targetedCharacter;

    //public float movementSpeed = 5;

    public Faction faction;

    [Header("Melee Attack")]
    public int meleeDamage = 10;
    public float meleeExecuteRange = 2;
    public float meleeAttackRange = 3;
    public float meleeAttackDelay = 0.5f;
    public float meleeCooldown = 1;
    public DamageType meleeDamageType;
    float meleeDelayTimer;
    float meleeCooldownTimer = 9999999;
    bool isMeleeAttacking;
    Vector3 enemyAttackDirection;
    RaycastHit meleeHitDetection;


    

    
    // Start is called before the first frame update
    void Start()
    {
        na = GetComponent<NavMeshAgent>();
        //na.speed = movementSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        meleeCooldownTimer += Time.deltaTime;

        SeekEnemy();

        MeleeAttack();

        /*
        if (isMeleeAttacking == true)
        {
            meleeDelayTimer += Time.deltaTime;
            print(meleeDelayTimer);
            if (meleeDelayTimer >= meleeAttackDelay)
            {
                ExecuteMeleeAttack();
                print("Melee attack executed");
                isMeleeAttacking = false;
                meleeCooldownTimer = 0;
            }
        }
        */
    }

    void SeekEnemy() // Enemy will seek out a character it is hostile towards and considers an targetedCharacter
    {
        na.destination = targetedCharacter.transform.position;
        /*
        if (Vector3.Distance(transform.position, targetedCharacter.transform.position) <= meleeExecuteRange && isMeleeAttacking == false && meleeCooldownTimer >= meleeCooldown)
        {
            InitiateMeleeAttack();
        }
        */
    }

    void MeleeAttack() // DOES NOT WORK PROPERLY, THE PROBLEM SEEMS TO BE WITH THE RAYCAST
    {
        if (Vector3.Distance(transform.position, targetedCharacter.transform.position) <= meleeExecuteRange && isMeleeAttacking == false && meleeCooldownTimer >= meleeCooldown)
        {
            //na.destination = transform.position;
            
            na.isStopped = true;
            na.enabled = false;
            enemyAttackDirection = new Vector3(targetedCharacter.transform.position.x, transform.position.y, targetedCharacter.transform.position.z);
            transform.LookAt(enemyAttackDirection);
            enemyAttackDirection = targetedCharacter.transform.position - transform.position;
            isMeleeAttacking = true;
            meleeDelayTimer = 0;
        }

        if (isMeleeAttacking == true)
        {
            meleeDelayTimer += Time.deltaTime;
            //print(meleeDelayTimer);
            if (meleeDelayTimer >= meleeAttackDelay)
            {
                print("Raycast launched");

                if (Physics.Raycast(transform.position, enemyAttackDirection, out meleeHitDetection, meleeAttackRange))
                {
                    print("Raycast hit");
                    DamageHitbox enemyHitbox = meleeHitDetection.collider.GetComponent<DamageHitbox>();
                    if (enemyHitbox != null)
                    {
                        print("Enemy struck");
                        enemyHitbox.Damage(meleeDamage, gameObject, faction, meleeDamageType, false);
                    }
                }

                print("Melee attack executed");
                isMeleeAttacking = false;
                meleeCooldownTimer = 0;

                na.enabled = true;
                na.isStopped = false;

            }
        }
    }


    /*
    void InitiateMeleeAttack()
    {
        na.destination = transform.position;
        enemyAttackDirection = new Vector3(targetedCharacter.transform.position.x, transform.position.y, targetedCharacter.transform.position.z);
        transform.LookAt(enemyAttackDirection);
        enemyAttackDirection = targetedCharacter.transform.position - transform.position;
        isMeleeAttacking = true;
        meleeDelayTimer = 0;
    }

    void ExecuteMeleeAttack()
    {
        print("Raycast launched");
        if (Physics.Raycast(transform.position, enemyAttackDirection, out meleeHitDetection, meleeAttackRange))
        {
            print("Raycast hit");
            DamageHitbox enemyHitbox = meleeHitDetection.collider.GetComponent<DamageHitbox>();
            print("Enemy struck");
            if (enemyHitbox != null)
            {
                enemyHitbox.Damage(meleeDamage, DamageType.KnockedOut);
            }
        }
    }
    */
}
