using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]

public class EnemyAI : MonoBehaviour
{

    public GameObject head;
    NavMeshAgent na;



    [Header("Enemy Detection")]
    [Min(0)] public float viewRange = 50;
    [Range(0, 180)]
    public float horizontalFOV = 60;
    [Range(0, 180)]
    public float verticalFOV = 30;
    RaycastHit fovLineOfSight;
    public GameObject[] viewedObjects;
    public GameObject[] targets;

    [Header("Targeting")]
    public GameObject priorityTarget;
    RaycastHit targetLineOfSight;
    public float maxPursueRange;
    public float pursueTime; // Enemy will continue pursuing after line of sight has been lost for this amount of time before giving up
    float pursueTimer;


    public int minRange;
    public int maxRange;


    // Use this for initialization
    void Awake()
    {
        na = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        FieldOfVision();

        

        if (priorityTarget != null)
        {
            TargetEnemy();
        }
        else
        {
            // Have priorities for selecting a target. Criteria could include nearest target, target with the lowest health, etc.
        }

    }

    void TargetEnemy()
    {
        Vector3 newDestination = new Vector3();

        if (Physics.Raycast(transform.position, priorityTarget.transform.position - transform.position, out targetLineOfSight, maxPursueRange))
        {

        }
        
        if (targetLineOfSight.collider == priorityTarget) // If line of sight is acquired
        {
            print("Line of sight acquired");
            pursueTimer = 0;

            /*
            float distanceFromTarget = Vector3.Distance(transform.position, priorityTarget.transform.position);
            if (distanceFromTarget > maxPursueRange)
            {

            }
            */




        }
        else // If line of sight is not acquired
        {
            /*
            pursueTimer += Time.deltaTime;
            if (pursueTimer >= pursueTime)
            {
                priorityTarget = null;
            }
            */
        }


        /*
        if (Physics.Raycast(transform.position, priorityTarget.transform.position - transform.position, out targetLineOfSight, maxPursueRange)) // Shoots raycast in direction of player
        {
            
        }
        else
        {
            
        }
        */

        /*

        //float distanceFromTarget = Vector3.Distance(transform.position, priorityTarget.transform.position);
        if (distanceFromTarget > maxRange)
        {
            na.SetDestination(priorityTarget.transform.position);
        }
        else if (distanceFromTarget < minRange)
        {
            //na.SetDestination(Vector3.MoveTowards(transform.position, priorityTarget.transform.position, -na.speed * Time.deltaTime));

            //Vector3 backpedalDestination = priorityTarget.transform.position + transform.position;
            Vector3 backpedalDestination = (transform.position - priorityTarget.transform.position).normalized * minRange;
            na.SetDestination(backpedalDestination);

            // This code needs work. The enemy is meant to backpedal if it is too close to the player.
        }

        if (distanceFromTarget >= minRange && distanceFromTarget <= maxRange)
        {
            na.SetDestination(transform.position);
        }

        */

    }

    void Attack()
    {

    }



    void FieldOfVision()
    {
        Collider[] objects = Physics.OverlapSphere(head.transform.position, viewRange);
        foreach (Collider c in objects)
        {
            if (Physics.Raycast(head.transform.position, c.transform.position - head.transform.position, out fovLineOfSight, viewRange)) // Launch a raycast to check if the thing being viewed is actually in the NPC's line of sight and not behind a wall.
            {
                if (fovLineOfSight.collider == c) // If raycast hits object being checked for line of sight.
                {
                    Vector3 relativePosition_X = new Vector3(c.transform.position.x, head.transform.position.y, c.transform.position.z) - head.transform.position;
                    Vector3 relativePosition_Y = new Vector3(head.transform.position.x, c.transform.position.y, c.transform.position.z) - head.transform.position;
                    Vector2 visionAngle = new Vector2(Vector3.Angle(relativePosition_X, head.transform.forward), Vector3.Angle(relativePosition_Y, head.transform.forward));
                    if (visionAngle.x < horizontalFOV && visionAngle.y < verticalFOV)
                    {
                        print("NPC " + gameObject.name + "has spotted " + c.gameObject.name + ".");

                        // Add c.gameObject to viewedObjects array, I need to figure out how to do this!
                    }
                }
            }

            /*
            Vector3 relativePosition_X = new Vector3(c.transform.position.x, head.transform.position.y, c.transform.position.z) - head.transform.position;
            Vector3 relativePosition_Y = new Vector3(head.transform.position.x, c.transform.position.y, c.transform.position.z) - head.transform.position;
            Vector2 visionAngle = new Vector2(Vector3.Angle(relativePosition_X, head.transform.forward), Vector3.Angle(relativePosition_Y, head.transform.forward));
            if (visionAngle.x < horizontalFOV && visionAngle.y < verticalFOV)
            {
                if (Physics.Raycast(head.transform.position, c.transform.position - head.transform.position, out fovLineOfSight, viewRange)) // Launch a raycast to check if the thing being viewed is actually in the NPC's line of sight and not behind a wall.
                {
                    if (fovLineOfSight.collider == c) // If raycast hits object being checked for line of sight.
                    {
                        print("NPC " + gameObject.name + "has spotted " + c.gameObject.name + ".");

                        // Add c.gameObject to viewedObjects array, I need to figure out how to do this!
                    }
                }
            }
            */
        }
    }
}
