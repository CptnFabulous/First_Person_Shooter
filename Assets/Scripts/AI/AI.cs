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
    public Animator stateMachine;
    [HideInInspector] public NpcHealth hp;
    [HideInInspector] public NavMeshAgent na;
    [HideInInspector] public Character c;
    [HideInInspector] public AudioSource audioOutput;

    


    [Header("Detection")]
    public Transform head;
    public float viewRange;
    [Range(0, 180)]
    public float xFOV;
    [Range(0, 180)]
    public float yFOV;
    public LayerMask viewDetection = ~0;
    [HideInInspector] public List<GameObject> fieldOfVision; // { get; private set; } // Object FOV. This is used for other scripts to easily reference what the enemy can currently see

    public float pursueRange;

    public Character target;
    


    private void Awake()
    {
        hp = GetComponent<NpcHealth>();
        na = GetComponent<NavMeshAgent>();
        c = GetComponent<Character>();
        audioOutput = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*
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
        */

        bool targetAcquired = target != null;
        if (target == null) // Checks for targets
        {
            target = AcquireTarget();
        }
        else
        {
            if (Vector3.Distance(transform.position, target.transform.position) > pursueRange)
            {
                print("Target out of range");
                target = null;
            }

            Health h = target.GetComponent<Health>();
            if (h != null && h.IsAlive() == false)
            {
                target = null;
            }
        }

        























        
        stateMachine.SetBool("targetAcquired", target != null);

        if (target != null)
        {
            stateMachine.SetFloat("targetDistance", Vector3.Distance(transform.position, target.transform.position));
        }

        stateMachine.SetFloat("targetNavMeshDistance", na.remainingDistance);
        stateMachine.SetInteger("health", hp.health.current);
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

    Character AcquireTarget()
    {
        Collider[] thingsInEnvironment = Physics.OverlapSphere(head.transform.position, viewRange);
        foreach (Collider thing in thingsInEnvironment)
        {
            if (LineOfSight(head.position, thing.transform, viewDetection))
            {
                print("Line of sight established between agent and " + thing.name);
                Character targetCharacter = thing.transform.root.GetComponent<Character>();
                if (targetCharacter != null && c.faction.Affiliation(targetCharacter.faction) == FactionState.Hostile)
                {
                    return targetCharacter;
                }
            }
        }
        print("No target found");
        return null;
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

    #region LineOfSight variants
    public static bool LineOfSight(Vector3 origin, Transform target, Transform[] exceptions, LayerMask coverCriteria, float overlap = 0.01f)
    {
        RaycastHit[] objectsBetween = Physics.RaycastAll(origin, target.position - origin, Vector3.Distance(origin, target.position) + overlap, coverCriteria);
        foreach(RaycastHit lineOfSightCheck in objectsBetween) // Checks if line of sight is established between the attacker and the cover position. If not, the agent can take cover there.
        {
            Transform t = lineOfSightCheck.collider.transform; // Gets transform of object

            // If object is a DamageHitbox, find the root object, which is the actual thing being tracked if it's an enemy
            DamageHitbox dh = t.GetComponent<DamageHitbox>();
            if (dh != null)
            {
                if (dh.healthScript != null)
                {
                    t = dh.healthScript.transform;
                }
            }
            
            if (t == target) // Checks if the object hit is the target
            {
                return true;
            }

            // If t is not the target, it is checked against the exception objects.
            bool isException = false;
            foreach(Transform g in exceptions)
            {
                if (t == g)
                {
                    isException = true;
                }
            }
            if (isException == false)
            {
                return false;
            }
        }

        return false; // If the raycast somehow doesn't hit anything, the enemy has disappeared, so it cannot establish line of sight with anything
    }

    public static bool LineOfSight(Vector3 origin, Transform target, Transform exception, LayerMask coverCriteria, float overlap = 0.01f)
    {
        RaycastHit[] objectsBetween = Physics.RaycastAll(origin, target.position - origin, Vector3.Distance(origin, target.position) + overlap, coverCriteria);
        foreach (RaycastHit lineOfSightCheck in objectsBetween) // Checks if line of sight is established between the attacker and the cover position. If not, the agent can take cover there.
        {
            Transform t = lineOfSightCheck.collider.transform; // Gets transform of object

            // If object is a DamageHitbox, find the root object, which is the actual thing being tracked if it's an enemy
            DamageHitbox dh = t.GetComponent<DamageHitbox>();
            if (dh != null)
            {
                if (dh.healthScript != null)
                {
                    t = dh.healthScript.transform;
                }
            }

            print(t.name);

            if (t == target) // Checks if the object hit is the target
            {
                return true;
            }

            // Compares t and the exception object. If they do not match, t is not the target or any exceptions, meaning line of sight is not established.
            if (t != exception)
            {
                return false;
            }
        }

        return false; // If the raycast somehow doesn't hit anything, the enemy has disappeared, so it cannot establish line of sight with anything
    }

    public static bool LineOfSight(Vector3 origin, Transform target, LayerMask coverCriteria, float overlap = 0.01f)
    {
        // Launches a raycast between the cover position and the attacker
        RaycastHit lineOfSightCheck;
        if (Physics.Raycast(origin, target.position - origin, out lineOfSightCheck, Vector3.Distance(origin, target.position) + overlap, coverCriteria))
        {
            Transform t = lineOfSightCheck.collider.transform; // Gets transform of object

            
            DamageHitbox dh = t.GetComponent<DamageHitbox>(); // If object is a DamageHitbox, find the root object, which is the actual thing being tracked if it's an enemy
            if (dh != null)
            {
                if (dh.healthScript != null)
                {
                    if (dh.healthScript.transform == target)
                    {
                        return true;
                    }
                }
            }

            if (t == target)
            {
                return true;
            }
        }

        return false;
    }
    #endregion
}