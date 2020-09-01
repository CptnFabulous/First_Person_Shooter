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

public class AI : MonoBehaviour//, IEventObserver
{
    [Header("References")]
    [HideInInspector] public Animator stateMachine;
    [HideInInspector] public NpcHealth hp;
    [HideInInspector] public NavMeshAgent na;
    [HideInInspector] public Character c;
    [HideInInspector] public AudioSource audioOutput;
    [HideInInspector] public EventObserver eo;

    


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
    public bool selfPreservation = true;
    public AttackMessage attackToDodge;






    [Header("Looking at stuff")]
    public float lookSpeed;
    public float lookThreshold;
    bool inLookIENumerator;
    Vector3 aimMarker;

    public virtual void Awake()
    {
        stateMachine = GetComponent<Animator>();
        hp = GetComponent<NpcHealth>();
        na = GetComponent<NavMeshAgent>();
        c = GetComponent<Character>();
        audioOutput = GetComponent<AudioSource>();

        eo = GetComponent<EventObserver>();
        eo.OnAttack += Dodge;
    }
    /*
    // Start is called before the first frame update
    void Start()
    {
        
    }
    */
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

    #region Looking at stuff
    void LookTowards(Vector3 position, float degreesPerSecond)
    {
        Quaternion correctRotation = Quaternion.LookRotation(position, transform.up);
        head.transform.rotation = Quaternion.RotateTowards(head.transform.rotation, correctRotation, degreesPerSecond * Time.deltaTime);
    }

    bool IsLookingAt(Vector3 position, float threshold)
    {
        if (Vector3.Angle(position - head.transform.position, head.transform.forward) < threshold)
        {
            return true;
        }
        return false;
    }

    public IEnumerator LookAtThing(Vector3 position, float lookTime, float threshold, AnimationCurve lookCurve)
    {
        inLookIENumerator = true;

        float timer = 0;

        Quaternion originalRotation = head.transform.rotation;

        while (IsLookingAt(position, threshold) == false)
        {
            Quaternion lookLerp = Quaternion.Lerp(originalRotation, Quaternion.LookRotation(position, transform.up), lookCurve.Evaluate(timer));
            head.transform.rotation = lookLerp;
            timer += Time.deltaTime / lookTime;
            yield return new WaitForEndOfFrame();
        }

        inLookIENumerator = false;
        print("Agent is now looking at " + position + ".");
    }

    public IEnumerator LookAtThingOld(Vector3 position, float lookTime, AnimationCurve lookCurve)
    {
        inLookIENumerator = true;

        float timer = 0;

        Vector3 originalMarkerPosition = aimMarker;

        while (Vector3.Distance(aimMarker, position) > lookThreshold)
        {
            Vector3 lookLerp = Vector3.Lerp(originalMarkerPosition, position, lookCurve.Evaluate(timer));
            aimMarker = lookLerp;
            timer += Time.deltaTime / lookTime;
            yield return new WaitForEndOfFrame();
        }

        inLookIENumerator = false;
        print("Agent is now looking at " + position + ".");
    }

    #endregion

    public void Dodge(AttackMessage am)
    {
        // If the AI is not already dodging an attack
        // If the attack is by an enemy who will harm them
        // If the AI is in the path of the attack

        Collider[] hitboxes = new Collider[hp.hitboxes.Length];
        for(int r = 0; r < hitboxes.Length; r++)
        {
            hitboxes[r] = hp.hitboxes[r].GetComponent<Collider>();
        }
        /*
        if (selfPreservation == true && attackToDodge == null && am.attacker.faction.Affiliation(c.faction) == FactionState.Hostile && am.AtRisk(hitboxes)) // If the attack is being executed by a character that is hostile to this NPC
        {
            print("kablowie");
            print(c.properName + " is being attacked by " + am.attacker.name + "!");
            //attackToDodge = am; // Specifies attack to dodge from
            //stateMachine.SetBool("mustDodgeAttack", true); // Sets trigger so agent can dodge attack
        }
        */
    }

    public static List<Collider> VisionConeSimple(Vector3 origin, Vector3 direction, float angle, float range, LayerMask viewable)
    {
        List<Collider> objectsInView = new List<Collider>();
        Collider[] objects = Physics.OverlapSphere(origin, range); // Checks for all objects in range
        foreach (Collider c in objects)
        {
            if (Vector3.Angle(c.transform.position - origin, direction) < angle) // Eliminates all objects outside a certain viewing angle
            {
                if (LineOfSight(origin, c.transform, viewable))
                {
                    objectsInView.Add(c); // Add c.gameObject to viewedObjects array
                }
            }
        }

        return objectsInView;
    }

    public static List<Collider> VisionConeSimpleTwoAngles(Vector3 origin, Vector3 direction, Vector2 angles, float range, LayerMask viewable)
    {
        List<Collider> objectsInView = new List<Collider>();
        Collider[] objects = Physics.OverlapSphere(origin, range); // Checks for all objects in range
        foreach (Collider c in objects)
        {
            // Obtains the horizontal and vertical relative position data for the raycast hit point relative to the line of sight's origin.
            Vector3 relativePosition_X = new Vector3(c.transform.position.x, origin.y, c.transform.position.z) - origin;
            Vector3 relativePosition_Y = new Vector3(origin.x, c.transform.position.y, c.transform.position.z) - origin;
            Vector2 visionAngle = new Vector2(Vector3.Angle(relativePosition_X, direction), Vector3.Angle(relativePosition_Y, direction));
            if (visionAngle.x < angles.x && visionAngle.y < angles.y)
            {
                if (LineOfSight(origin, c.transform, viewable))
                {
                    objectsInView.Add(c); // Add c.gameObject to viewedObjects array
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
                //print("Line of sight established between agent and " + thing.name);
                Character targetCharacter = thing.transform.root.GetComponent<Character>();
                if (targetCharacter != null && c.faction.Affiliation(targetCharacter.faction) == FactionState.Hostile)
                {
                    return targetCharacter;
                }
            }
        }
        //print("No target found");
        return null;
    }

    public static float NavMeshPathLength(NavMeshPath path)
    {
        // Calculate path length
        float pathLength = 0;
        for (int r = 1; r < path.corners.Length; r++)
        {
            pathLength += Vector3.Distance(path.corners[r - 1], path.corners[r]);
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

            //print(t.name);

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

    public static bool LineOfSight(Vector3 viewer, Vector3 viewed, LayerMask coverCriteria, GameObject[] exceptions)
    {
        RaycastHit[] objectsBetween = Physics.RaycastAll(viewer, viewed - viewer, Vector3.Distance(viewer, viewed), coverCriteria);
        foreach(RaycastHit lineOfSightCheck in objectsBetween)
        {
            GameObject g = lineOfSightCheck.collider.gameObject; // Gets transform of object

            DamageHitbox dh = g.GetComponent<DamageHitbox>(); // If object is a DamageHitbox, find the root object, which is the actual thing being tracked if it's an enemy
            if (dh != null)
            {
                g = dh.GetRootObject();
            }

            bool notException = true;
            foreach (GameObject go in exceptions)
            {
                if (g == go)
                {
                    notException = false;
                }
            }

            if (notException == true)
            {
                return false;
            }
        }

        return true; // If the raycast did not hit anything, there is nothing inbetween the two objects (except for things that the raycast does not deem important)
    }

    public static bool LineOfSight(Vector3 viewer, Vector3 viewed, LayerMask coverCriteria, GameObject exception)
    {
        GameObject[] array = new GameObject[1];
        array[0] = exception;
        return LineOfSight(viewer, viewed, coverCriteria, array);
    }

    public static bool LineOfSight(Vector3 viewer, Vector3 viewed, LayerMask coverCriteria)
    {
        // Launches a raycast between the cover position and the attacker
        RaycastHit lineOfSightCheck;
        if (Physics.Raycast(viewer, viewed - viewer, out lineOfSightCheck, Vector3.Distance(viewer, viewed), coverCriteria))
        {
            return false;
        }

        return true; // If the raycast did not hit anything, there is nothing inbetween the two objects (except for things that the raycast does not deem important)
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