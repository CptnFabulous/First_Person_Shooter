using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof (NavMeshAgent))]
public class EnemyMelee : NPC
{
    [Header("General stats")]
    public float moveSpeed;

    [Header("Targeting")]
    public GameObject head;
    public GameObject target;

    public float targetRange = 25;
    public float pursueRange = 50; // Max distance NPC will pursue target before running out of patience
    //public float patience = 5; // Amount of time NPC will p
    public LayerMask viewDetection;

    RaycastHit lookingAt;

    [Header("Attacks")]
    public MeleeAttack attack;

    // Update is called once per frame
    void Update()
    {
        if (target == null)
        {
            Character t = AcquireTarget();
            if (t != null)
            {
                target = t.gameObject;
            }
        }

        if (target != null)
        {
            if (Vector3.Distance(transform.position, target.transform.position) > attack.range || LineOfSight(target, pursueRange) == false)
            {
                Seek(target, pursueRange);
            }
            else
            {
                StandStill();
            }


            attack.TargetEnemy(target, gameObject, na, moveSpeed, ch.faction, head.transform, lookingAt, audioSource);

            Health h = target.GetComponent<Health>();
            if (h != null && h.health.current <= 0)
            {
                target = null;
            }

            if (Vector3.Distance(transform.position, target.transform.position) > pursueRange && attack.isAttacking == false)
            {
                target = null;
            }
        }
        else
        {
            // Perform idle behaviour
            StandStill();
        }
    }

    Character AcquireTarget()
    {
        Collider[] thingsInEnvironment = Physics.OverlapSphere(head.transform.position, targetRange);
        foreach (Collider c in thingsInEnvironment)
        {
            if (LineOfSight(c.gameObject, targetRange))
            {
                Character targetCharacter = c.transform.root.GetComponent<Character>();
                if (targetCharacter != null && ch.faction.Affiliation(targetCharacter.faction) == FactionState.Hostile)
                {
                    return targetCharacter;
                }
            }

            /*
            RaycastHit lineOfSight;
            if (Physics.Raycast(head.transform.position, c.transform.position - head.transform.position, out lineOfSight, pursueRange, viewDetection) && lineOfSight.collider == c)
            {
                Character targetCharacter = c.transform.root.GetComponent<Character>();
                if (targetCharacter != null && ch.faction.Affiliation(targetCharacter.faction) == FactionState.Hostile)
                {
                    return targetCharacter;
                }
            }
            */
        }
        return null;
    }

    bool LineOfSight(GameObject target, float range)
    {
        RaycastHit lineOfSight;
        if (Physics.Raycast(head.transform.position, target.transform.position - head.transform.position, out lineOfSight, range, viewDetection))
        {
            if (lineOfSight.collider.gameObject == target)
            {
                return true;
            }
        }
        return false;
    }

}




/*
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
        //meleeCooldownTimer += Time.deltaTime;

        //MeleeAttack();
    }

    /*
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
    */

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
    
}
*/