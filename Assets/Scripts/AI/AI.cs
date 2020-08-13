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

    void AdjustLook(Vector3 newLookPoint)
    {
        if (inLookIENumerator == false)
        {
            aimMarker = Vector3.MoveTowards(aimMarker, newLookPoint, lookSpeed * Time.deltaTime);
            head.transform.LookAt(aimMarker, transform.up);
        }
    }

    public IEnumerator LookAtThing(Vector3 position, float lookTime, AnimationCurve lookCurve)
    {
        inLookIENumerator = true;

        float timer = 0;

        while (Vector3.Distance(aimMarker, position) > lookThreshold)
        {
            Vector3 lookLerp = Vector3.Lerp(aimMarker, position, lookCurve.Evaluate(timer));
            head.transform.LookAt(lookLerp, transform.up);
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

        if (selfPreservation == true && attackToDodge == null && am.attacker.faction.Affiliation(c.faction) == FactionState.Hostile && am.AtRisk(hitboxes)) // If the attack is being executed by a character that is hostile to this NPC
        {
            print("kablowie");
            print(c.properName + " is being attacked by " + am.attacker.name + "!");
            //attackToDodge = am; // Specifies attack to dodge from
            //stateMachine.SetBool("mustDodgeAttack", true); // Sets trigger so agent can dodge attack
        }
    }

    public static List<Collider> FieldOfView(Vector3 origin, Vector3 direction, float angle, float range, LayerMask viewable)
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


    public static RaycastHit[] RaycastVisionCone(Transform origin, float angle, float range, float raycastDiameter, LayerMask viewable)
    {
        #region Figure out amount of raycast rings required
        float circumference = Mathf.PI * (range * 2); // Creates a circumference of an imaginary circle
        float coneWidth = circumference / 360 * angle; // Gets the 'length' from travelling across the 'surface' of the end of the cone (and the max width of the distance that needs to be covered with raycasts)
        float notActualAmountOfRaycastRings = coneWidth / raycastDiameter; // Produces amount of rings needed to fill the whole cone
        int amountOfRaycastRings = Mathf.RoundToInt(coneWidth / raycastDiameter); // Produces actual amount that can fit in the cone
        #region Adjusts size of spherecasts to form a uniform covering of the cone's edge
        if (amountOfRaycastRings > notActualAmountOfRaycastRings) // Ensures that the actual amount is lower than the decimal amount, so there is always not enough space
        {
            amountOfRaycastRings -= 1;
        }
        float remainder = notActualAmountOfRaycastRings - amountOfRaycastRings; // Obtains the remainder
        remainder /= amountOfRaycastRings; // Divides the remainder by amountOfRaycastRings
        raycastDiameter *= (1 + remainder); // Multiplies the width of the raycast by one times the remainder, essentially expanding the existing raycasts to fill the extra space.
        #endregion
        float ringAngleOutIncrement = angle / amountOfRaycastRings;
        #endregion

        for (int r = 0; r < amountOfRaycastRings; r++) // Produce several rings of raycasts
        {
            float ringAngleOut = ringAngleOutIncrement * r; // Gets the angle of a hypothetical cone with a max radius of the ring
            float ringRadiusIncrement = coneWidth / amountOfRaycastRings;
            float ringRadius = ringRadiusIncrement * r; // Figures out the radius of the current ring
            float ringCircumference = Mathf.PI * (ringRadius * 2); // Obtains the circumference of the current ring

            // Divide ringCircumference by raycastDiameter to get the required amount of raycasts to fill the ring
            float notActualAmountOfRingSegments = ringCircumference / raycastDiameter;
            int amountOfRingSegments = Mathf.RoundToInt(ringCircumference / raycastDiameter);
            if (amountOfRingSegments < notActualAmountOfRingSegments)
            {
                amountOfRingSegments += 1;
            }

            if (amountOfRingSegments <= 0)
            {
                amountOfRingSegments = 1;
            }

            float ringAngleAroundIncrement = 360 / amountOfRingSegments;
            // Calculate the required number of spherecasts (of diameter raycastDiameter) to form a ring

            for (float s = 0; s < amountOfRingSegments; s++)
            {
                
                //Vector3 raycastDirection = Quaternion.AngleAxis(ringAngleAround * s, origin.forward) * Quaternion.AngleAxis(ringAngleOutwards, origin.right) * origin.forward;
                Vector3 raycastDirection = Misc.DirectionFromTransform(origin, new Vector3(ringAngleOut, 0, ringAngleAroundIncrement * s));
                Debug.DrawRay(origin.position, origin.position + raycastDirection * range);
            }
        }

        return null;
    }

    public static RaycastHit[] RaycastVisionCone(Vector3 origin, Vector3 direction, float angle, float range, float raycastDiameter, LayerMask viewable)
    {
        float circumference = Mathf.PI * (range * 2); // Creates a circumference of an imaginary circle
        float coneWidth = circumference / 360 * angle; // Divides by 360 then multiplies by the angle value to get the distance from travelling that angle around the circle (and the max width of the distance that needs to be covered with raycasts)
        float notActualAmountOfRaycastRings = coneWidth / raycastDiameter; // Produces amount of rings needed to fill the whole cone
        int amountOfRaycastRings = Mathf.RoundToInt(coneWidth / raycastDiameter); // Produces actual amount that can fit in the cone

        if (amountOfRaycastRings > notActualAmountOfRaycastRings) // Ensures that the actual amount is lower than the decimal amount, so there is always not enough space
        {
            amountOfRaycastRings -= 1;
        }
        float remainder = notActualAmountOfRaycastRings - amountOfRaycastRings; // Obtains the remainder
        remainder /= amountOfRaycastRings; // Divides the remainder by amountOfRaycastRings
        raycastDiameter *= (1 + remainder); // Multiplies the width of the raycast by one times the remainder, essentially expanding the existing raycasts to fill the extra space.

        float angleIncrement = angle / amountOfRaycastRings;

        for (int r = 0; r < amountOfRaycastRings; r++)
        {
            float rotationAroundRing = 0;
            //Vector3 raycastDirection = Quaternion.AngleAxis(angleIncrement * r, Quaternion.Euler(0, 90, rotationAmount) * direction) * direction; // FIGURE THIS OUT

            // How to produce a Vector3 perpendicular to the direction value? I need the direction, an 'up' orientation and perhaps the origin? Vector3.Cross seems to work, but I don't know how it calculates which specific angle


            Vector3 left = Vector3.Cross(direction, Vector3.up).normalized;
            //left = Vector3.
            // Multiplies a Quaternion around the cone by the angle rotationAroundRing, plus a Quaternion representing how angled out the ring is

            Vector3 raycastDirection = Quaternion.AngleAxis(rotationAroundRing, direction) * Quaternion.AngleAxis(angleIncrement * r, left) * direction;

            Color c = Color.white;
            float cv = (c.r / amountOfRaycastRings) * r;
            c = new Color(cv, cv, cv);

            Debug.DrawRay(origin, raycastDirection * range, c);

        }

        return null;
    }
    
    public static RaycastHit[] RaycastVisionField(Vector3 origin, Vector3 direction, Vector2 angles, float range, float raycastDiameter, LayerMask viewable)
    {
        List<RaycastHit> hits = new List<RaycastHit>();
        
        float angleCircumferenceX = Mathf.PI * (range * 2);
        angleCircumferenceX /= 360;
        angleCircumferenceX *= angles.x;
        int raycastArrayDiameterX = Mathf.RoundToInt(angleCircumferenceX / raycastDiameter);
        float angleIncrementX = angles.x * 2 / raycastArrayDiameterX;

        float angleCircumferenceY = Mathf.PI * (range * 2);
        angleCircumferenceY /= 360;
        angleCircumferenceY *= angles.y;
        int raycastArrayDiameterY = Mathf.RoundToInt(angleCircumferenceY / raycastDiameter);
        float angleIncrementY = angles.y * 2 / raycastArrayDiameterY;

        for (int x = 0; x < raycastArrayDiameterX; x++)
        {
            for (int y = 0; y < raycastArrayDiameterY; y++)
            {
                Vector3 raycastDirection = Quaternion.Euler(angleIncrementX * x, angleIncrementY * y, 0) * direction;

                RaycastHit rh;
                if (Physics.SphereCast(origin, raycastDiameter, raycastDirection, out rh, range, viewable))
                {
                    hits.Add(rh);
                }
            }
        }

        return hits.ToArray();
    }



    public static RaycastHit[] BoundsConeThing(Transform origin, float angle, float range, LayerMask viewable, float raycastDiameter = 0.2f)
    {
        List<RaycastHit> hits = new List<RaycastHit>();

        Collider[] objects = Physics.OverlapSphere(origin.position, range, viewable);
        foreach(Collider c in objects)
        {
            // Produces a position the same distance from the origin as the collider, but straight on
            float distanceFromOrigin = Vector3.Distance(origin.position, c.bounds.center);
            Vector3 centreOfConeAtDistanceEquivalentToCollider = origin.position + (origin.forward.normalized * distanceFromOrigin);
            // Figures out the part of the collider that is the closest to the centre of the cone's diameter
            Vector3 closestPoint = c.bounds.ClosestPoint(centreOfConeAtDistanceEquivalentToCollider);

            if (Vector3.Angle(origin.forward, closestPoint - origin.position) < angle) // If the angle of that point is inside the cone, perform a raycast check
            {
                // Use Bounds.ClosestPoint four times, with points to the left, right, up and down of the bounding box (relative to the cone centre). Then use Vector3.Distance to calculate the distances and produce a rectangle of certain dimensions.
                Vector3 upPoint = c.bounds.center + origin.up * 999999999999999;
                Vector3 downPoint = c.bounds.center + -origin.up * 999999999999999;
                Vector3 leftPoint = c.bounds.center + -origin.right * 999999999999999;
                Vector3 rightPoint = c.bounds.center + origin.right * 999999999999999;
                upPoint = c.bounds.ClosestPoint(upPoint);
                downPoint = c.bounds.ClosestPoint(downPoint);
                leftPoint = c.bounds.ClosestPoint(leftPoint);
                rightPoint = c.bounds.ClosestPoint(rightPoint);

                // Produces dimensions for a rectangular area to sweep with raycasts
                float scanAreaY = Vector3.Distance(upPoint, downPoint);
                float scanAreaX = Vector3.Distance(leftPoint, rightPoint);
                
                /*
                // Debug info
                Debug.DrawLine(c.bounds.ClosestPoint(upPoint), c.bounds.ClosestPoint(downPoint), Color.red);
                Debug.DrawLine(c.bounds.ClosestPoint(leftPoint), c.bounds.ClosestPoint(rightPoint), Color.green);
                print("Scan area dimensions: " + scanAreaX + ", " + scanAreaY);
                */

                // Divide the rectangle dimensions by the sphereCastDiameter to obtain the amount of spherecasts necessary to cover the area.
                int raycastArrayLength = Mathf.CeilToInt(scanAreaX / raycastDiameter);
                int raycastArrayHeight = Mathf.CeilToInt(scanAreaY / raycastDiameter);

                // Creates variables to determine how far apart to space the raycasts on each axis
                float spacingX = scanAreaX / raycastArrayLength;
                float spacingY = scanAreaY / raycastArrayHeight;

                // Creates axes along which to align the raycasts
                Vector3 raycastGridAxisX = (rightPoint - c.bounds.center).normalized;
                Vector3 raycastGridAxisY = (upPoint - c.bounds.center).normalized;

                // Cast an array of rays to 'sweep' the square for line of sight.
                for (int y = 0; y < raycastArrayHeight; y++)
                {
                    for (int x = 0; x < raycastArrayLength; x++)
                    {
                        // Creates coordinates along the sweep area for the raycast. 0,0 would be the centre, so for each axis I am taking away a value equivalent to half of that axis dimension, so I can use the bottom-left corner as 0,0
                        float distanceX = spacingX * x - scanAreaX / 2;
                        float distanceY = spacingY * y - scanAreaY / 2;

                        // Starts with c.bounds.centre, then adds direction values multiplied by the appropriate distance value for each axis, to create the point that you want the raycast to hit.
                        Vector3 raycastAimPoint = c.bounds.center + (raycastGridAxisX * distanceX) + (raycastGridAxisY * distanceY);

                        // From that point, it creates a direction value for the raycast to aim in.
                        Vector3 raycastAimDirection = raycastAimPoint - origin.position;

                        // Launches raycast
                        if (Vector3.Angle(raycastAimDirection, origin.forward) < angle) // If the raycast direction is still inside the cone
                        {
                            // It's actually a BoxCast, so the raycasts tesselate and there are no blind spots.
                            RaycastHit lineOfSightCheck;
                            if (Physics.BoxCast(origin.position, new Vector3(raycastDiameter / 2, raycastDiameter / 2, raycastDiameter / 2), raycastAimDirection, out lineOfSightCheck, Quaternion.identity, range, viewable))
                            {
                                if (lineOfSightCheck.collider == c && (hits.Contains(lineOfSightCheck) == false))
                                {
                                    //Debug.DrawLine(origin.position, raycastAimPoint, Color.cyan);
                                    //print("FOV function has seen " + c.name);
                                    hits.Add(lineOfSightCheck);

                                    // Ends for loops prematurely, ensuring no more unnecessary rays are cast if an object has already been found.
                                    x = raycastArrayLength;
                                    y = raycastArrayHeight;
                                    //This is important for performance, otherwise this function will be very laggy.
                                    //Even then, do not run this every frame. I would probably advise running this every second or so on a timer.
                                }
                            }
                        }
                    }
                }
            }
        }

        return hits.ToArray();
    }





    public static List<Collider> FieldOfView(Vector3 origin, Vector3 direction, Vector2 angles, float range, LayerMask viewable)
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

        /*
        // Launches a raycast between the cover position and the attacker
        RaycastHit lineOfSightCheck;
        if (Physics.Raycast(viewer, viewed - viewer, out lineOfSightCheck, Vector3.Distance(viewer, viewed), coverCriteria))
        {
            GameObject g = lineOfSightCheck.collider.gameObject; // Gets transform of object

            DamageHitbox dh = g.GetComponent<DamageHitbox>(); // If object is a DamageHitbox, find the root object, which is the actual thing being tracked if it's an enemy
            if (dh != null)
            {
                g = dh.GetRootObject();
            }

            foreach(GameObject go in exceptions)
            {
                if (g == go)
                {
                    return true;
                }
            }

            return false;
        }

        return true; // If the raycast did not hit anything, there is nothing inbetween the two objects (except for things that the raycast does not deem important)
        */
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