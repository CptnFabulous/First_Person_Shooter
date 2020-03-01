using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI : MonoBehaviour
{
    public Animator movementStateMachine;
    public Animator actionStateMachine;

    NpcHealth hp;
    NavMeshAgent na;

    public Character target;

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
        t += Time.deltaTime;
        if (t >= 5)
        {
            b = !b;
            //print("State changed");
        }
        movementStateMachine.SetBool("targetAcquired", b);


        //movementStateMachine.SetBool("targetAcquired", target != null);
        if (target != null)
        {
            movementStateMachine.SetFloat("targetDistance", Vector3.Distance(transform.position, target.transform.position));
            movementStateMachine.SetFloat("targetNavMeshDistance", na.remainingDistance);
        }
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
