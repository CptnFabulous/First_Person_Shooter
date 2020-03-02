using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


/*

Current AI behaviours to make:
* Seek cover
* Dodge attack
* Evade target
* Pursue target
* Patrol along route
* Wander randomly

Current AI action behaviours to make:
* Ranged projectile attack
* Ranged throwable attack (calculate arcs)
* Melee attack
*/

public class AI : MonoBehaviour
{
    [Header("References")]
    public Animator movementStateMachine;
    public Animator actionStateMachine;
    [HideInInspector] public NpcHealth hp;
    [HideInInspector] public NavMeshAgent na;

    public Character target;


    [Header("Detection")]



    float t;
    bool b;

    private void Awake()
    {
        hp = GetComponent<NpcHealth>();
        na = GetComponent<NavMeshAgent>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bool targetAcquired = (target != null);
        movementStateMachine.SetBool("targetAcquired", targetAcquired);

        if (target != null)
        {
            movementStateMachine.SetFloat("targetDistance", Vector3.Distance(transform.position, target.transform.position));
        }

        movementStateMachine.SetFloat("targetNavMeshDistance", na.remainingDistance);
        movementStateMachine.SetInteger("health", hp.health.current);
        
        /*
        Appropriate variables for state machine
        
        bool targetAcquired
        float targetDistance
        float targetNavMeshDistance
        int health

        */
    }
}
