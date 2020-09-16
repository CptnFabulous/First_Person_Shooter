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
    [HideInInspector] public Character characterData;
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
    public float pursueRange;
    

    public Character target;

    


    [Header("Self-preservation")]
    public SelfPreservation selfPreservationBehaviour;
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
        characterData = GetComponent<Character>();
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
        // If the AI is willing to dodge attacks
        // If the AI is not already dodging an attack
        // If the AI is at risk of being damaged
        if (selfPreservationBehaviour != SelfPreservation.Suicidal && attackToDodge == null && am.AtRisk(characterData))
        {
            print(name + " is in danger!");

            attackToDodge = am; // Specifies attack to dodge from
            stateMachine.SetBool("mustDodgeAttack", true); // Sets trigger so agent can dodge attack
        }
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
                if (targetCharacter != null && characterData.HostileTowards(targetCharacter))
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