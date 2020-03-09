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
    [HideInInspector] public Character c;

    public Character target;


    [Header("Detection")]
    public Transform head;
    public float viewRange;
    [Range(0, 180)]
    public float xFOV;
    [Range(0, 180)]
    public float yFOV;
    [HideInInspector] public List<GameObject> fieldOfVision;// { get; private set; } // Object FOV. This is used for other scripts to easily reference what the enemy can currently see


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
        fieldOfVision = FieldOfView(head, viewRange, xFOV, yFOV);
        if (target == null)
        {
            Character bestTarget = null;
            foreach (GameObject g in fieldOfVision)
            {
                Character ch = g.GetComponent<Character>();
                if (ch != null && c.faction.Affiliation(ch.faction) == FactionState.Hostile)
                {
                    // Check other potential attributes for character
                    bestTarget = ch;
                }
            }
            target = bestTarget;
        }

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

    public static List<GameObject> FieldOfView(Transform viewOrigin, float viewRange, float horizontalFOV, float verticalFOV)
    {
        List<GameObject> objectsInView = new List<GameObject>();
        Collider[] objects = Physics.OverlapSphere(viewOrigin.position, viewRange); // Checks for all objects in range
        foreach (Collider c in objects)
        {
            RaycastHit lineOfSight;
            if (Physics.Raycast(viewOrigin.position, c.transform.position - viewOrigin.position, out lineOfSight, viewRange)) // Launch a raycast to check if the object is actually in the NPC's line of sight.
            {
                if (lineOfSight.collider == c) // If raycast hits object being checked for line of sight.
                {
                    /*
                    Vector3 relativePosition_X = new Vector3(c.transform.position.x, viewOrigin.position.y, c.transform.position.z) - viewOrigin.position;
                    Vector3 relativePosition_Y = new Vector3(viewOrigin.position.x, c.transform.position.y, c.transform.position.z) - viewOrigin.position;
                    */

                    // Obtains the horizontal and vertical relative position data for the raycast hit point relative to the line of sight's origin.
                    Vector3 relativePosition_X = new Vector3(lineOfSight.point.x, viewOrigin.position.y, lineOfSight.point.z) - viewOrigin.position;
                    Vector3 relativePosition_Y = new Vector3(viewOrigin.position.x, lineOfSight.point.y, lineOfSight.point.z) - viewOrigin.position;

                    Vector2 visionAngle = new Vector2(Vector3.Angle(relativePosition_X, viewOrigin.forward), Vector3.Angle(relativePosition_Y, viewOrigin.forward));
                    if (visionAngle.x < horizontalFOV && visionAngle.y < verticalFOV)
                    {
                        objectsInView.Add(c.gameObject); // Add c.gameObject to viewedObjects array

                    }
                }
            }
        }

        return objectsInView; // Returns list of objects the player is looking at
    }

    public static float NavMeshPathLength(NavMeshPath path)
    {
        // Calculate path length
        float pathLength = 0;
        for (int i = 1; i < path.corners.Length; i++)
        {
            pathLength += Vector3.Distance(path.corners[i - 1], path.corners[i]);
        }

        return pathLength;
    }

    public static bool LineOfSight(Vector3 origin, Transform target, LayerMask coverCriteria, float overlap = 0.01f)
    {
        // Launches a raycast between the cover position and the attacker
        RaycastHit lineOfSightCheck;
        if (Physics.Raycast(origin, target.position - origin, out lineOfSightCheck, Vector3.Distance(origin, target.position) + overlap, coverCriteria))
        {
            // Checks if line of sight is established between the attacker and the cover position. If not, the agent can take cover there.
            if (lineOfSightCheck.collider.transform == target)
            {
                return true;
            }
        }

        return false;
    }
}
