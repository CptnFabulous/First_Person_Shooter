using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof (NavMeshAgent))]
public class EnemyMelee : MonoBehaviour
{
    NavMeshAgent na;

    public GameObject head;

    public float movementSpeed;

    public int damage;
    public float executeRange;
    public float attackRange;

    public GameObject targetedCharacter;

    
    // Start is called before the first frame update
    void Start()
    {
        na = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SeekEnemy(GameObject enemy) // Enemy will seek out a character it is hostile towards and considers an enemy
    {
        na.destination = enemy.transform.position;

        if (Vector3.Distance(transform.position, enemy.transform.position) <= executeRange)
        {

        }
    }

    void MeleeAttack()
    {

    }
}
