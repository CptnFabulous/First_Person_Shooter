using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]

public class BulletEnemy : MonoBehaviour
{

    public GameObject head;
    NavMeshAgent na;



    [Header("Enemy Detection")]
    public GameObject[] targets;
    public GameObject priorityTarget;
    [Min(0)] public float viewRange;
    [Range(0, 180)]
    public float horizontalFOV;
    [Range(0, 180)]
    public float verticalFOV;
    RaycastHit fovLineOfSight;


    [Header("Targeting")]
    public int damage;
    public int minRange;
    public int maxRange;

    /*
    [Header("Field of Vision")]
    
    */


    // Use this for initialization
    void Awake()
    {
        na = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        TargetEnemy();
    }

    void TargetEnemy()
    {
        float distanceFromTarget = Vector3.Distance(transform.position, priorityTarget.transform.position);
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



    }

    void Attack()
    {

    }



    void FieldOfVision()
    {
        Collider[] viewedObjects = Physics.OverlapSphere(head.transform.position, viewRange);
        foreach (Collider c in viewedObjects)
        {
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

                        // Do stuff with object in FOV
                    }
                }
            }
        }
    }
}
