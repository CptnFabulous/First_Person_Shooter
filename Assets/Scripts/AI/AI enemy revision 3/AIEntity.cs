using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIEntity : MonoBehaviour//, ILogHandler
{
    [HideInInspector] public NpcHealth hp;
    [HideInInspector] public NavMeshAgent na;
    [HideInInspector] public Character characterData;
    [HideInInspector] public AudioSource audioOutput;
    [HideInInspector] public EventObserver eo;
    public Animator animationController;
    public Animator aiStateMachine;

    [Header("Current target")]
    public Character currentTarget;

    [Header("Detection")]
    public Transform head;
    public float viewRange = 50;
    //[Range(0, 180)] public float xFOV = 60;
    //[Range(0, 180)] public float yFOV = 60;
    public LayerMask viewDetection = ~0;
    public float pursueRange = 60;
    public float pursuePatience = 10;
    float patienceTimer = float.MaxValue;

    [Header("Self-preservation")]
    public float reactionTime = 0.5f;
    public SelfPreservation selfPreservationBehaviour;
    public float dodgeCooldown = 2;
    float dodgeCooldownTimer;
    public AttackMessage attackToDodge;

    [Header("Looking at stuff")]
    public float defaultLookDegreesPerSecond = 30;
    public float lookThreshold = 0.2f;
    bool inLookIENumerator;
    Quaternion lookDirectionQuaternion; // Represents the current direction the AI is looking in.
    public Vector3 LookOrigin // The point in space the AI looks and aims from.
    {
        get
        {
            return head.transform.position;
        }
    }
    public Vector3 LookDirection // The direction the AI is looking in, converted into an easy Vector3 value.
    {
        get
        {
            return lookDirectionQuaternion * Vector3.forward;
        }
    }
    public Vector3 LookUp // A direction directly up perpendicular to the direction the AI is looking.
    {
        get
        {
            return lookDirectionQuaternion * Vector3.up;
        }
    }

    public virtual void Awake()
    {
        lookDirectionQuaternion = head.transform.rotation;
        aiStateMachine = GetComponent<Animator>();
        hp = GetComponent<NpcHealth>();
        na = GetComponent<NavMeshAgent>();
        characterData = GetComponent<Character>();
        audioOutput = GetComponent<AudioSource>();

        eo = GetComponent<EventObserver>();
        eo.OnAttack += Dodge;
    }

    // Update is called once per frame
    void Update()
    {
        dodgeCooldownTimer += Time.deltaTime;

        Debug.DrawRay(na.destination, Vector3.up * 10, Color.blue, 2);
        Debug.DrawLine(transform.position, na.destination, Color.cyan);
        

        PursueTargetUpdate();



        UpdateStateMachineVariables();

        if (animationController != null)
        {
            UpdateAnimatorVariables();
        }
    }

    #region Looking around
    // Continuously rotates AI aim over time to look at position value, at a speed of degreesPerSecond
    public void RotateLookTowards(Vector3 position, float degreesPerSecond) 
    {
        Quaternion correctRotation = Quaternion.LookRotation(position - LookOrigin, transform.up);
        lookDirectionQuaternion = Quaternion.RotateTowards(lookDirectionQuaternion, correctRotation, degreesPerSecond * Time.deltaTime);
        Debug.DrawRay(LookOrigin, LookDirection * 99, Color.grey);
    }

    // Is the AI looking close enough to the position to meet the angle threshold?
    public bool IsLookingAt(Vector3 position, float threshold)
    {
        if (Vector3.Angle(position - LookOrigin, LookDirection) <= threshold)
        {
            return true;
        }
        return false;
    }

    // Continuously rotates AI aim to return to looking in the direction it is moving.
    public void ReturnToNeutralLookPosition()
    {
        RotateLookTowards(LookOrigin + transform.forward, defaultLookDegreesPerSecond);
    }

    // Rotates AI aim to look at something, in a specified time.
    public IEnumerator LookAtThing(Vector3 position, float lookTime, float threshold, AnimationCurve lookCurve)
    {
        inLookIENumerator = true;

        float timer = 0;

        Quaternion originalRotation = lookDirectionQuaternion;

        while (timer < 1)
        {
            timer += Time.deltaTime / lookTime;

            lookDirectionQuaternion = Quaternion.Lerp(originalRotation, Quaternion.LookRotation(position, transform.up), lookCurve.Evaluate(timer));
            
            yield return new WaitForEndOfFrame();
        }

        inLookIENumerator = false;
        print("Agent is now looking at " + position + ".");
    }
    #endregion

    #region Seeking targets
    void PursueTargetUpdate()
    {
        if (currentTarget == null) // If the AI has not acquired a target, check for one
        {
            currentTarget = AcquireTarget();
        }

        if (currentTarget != null) // If the AI has a target, check if said target is still worth pursuing
        {
            // If the AI cannot immediately find their target, count up a timer and continue pursuing until the timer expires
            if (Vector3.Distance(transform.position, currentTarget.transform.position) > pursueRange || AIFunction.SimpleLineOfSightCheck(currentTarget.transform.position, LookOrigin, viewDetection) == false)
            {
                patienceTimer = 0;
            }

            patienceTimer += Time.deltaTime;

            if (patienceTimer >= pursuePatience)
            {
                print("Target out of range");
                currentTarget = null;
            }

            Health h = currentTarget.GetComponent<Health>();
            if (h != null && h.IsAlive() == false)
            {
                currentTarget = null;
            }
        }
    }

    Character AcquireTarget()
    {
        Collider[] thingsInEnvironment = Physics.OverlapSphere(LookOrigin, viewRange);
        foreach (Collider thing in thingsInEnvironment)
        {
            if (AIFunction.LineOfSight(LookOrigin, thing.transform, viewDetection))
            {
                //print("Line of sight established between agent and " + thing.name);
                Character targetCharacter = thing.transform.root.GetComponent<Character>();
                if (targetCharacter != null && characterData.HostileTowards(targetCharacter))
                {
                    return targetCharacter;
                }
            }
        }

        return null;
    }

    /*
    public bool IsTargetWithinRange(Vector3 position, float threshold)
    {
        // Calculates distance between entity and target
        float distanceToTarget = Vector3.Distance(LookOrigin, position);
        // Calculates a position that's the same distance out as the target, but in the direction the AI is currently aiming.
        Vector3 positionInAimDirectionButCloseToTarget = (position - LookOrigin).normalized * distanceToTarget;
        if (Vector3.Distance(position, positionInAimDirectionButCloseToTarget) <= threshold)
        {
            return true;
        }
        return false;
    }
    */
    #endregion

    #region Avoiding damage
    void Dodge(AttackMessage am)
    {
        // If the cooldown has finished after the last dodge
        // If the AI is willing to dodge attacks
        // If the AI is not already dodging an attack
        // If the AI is at risk of being damaged
        if (dodgeCooldownTimer >= dodgeCooldown && selfPreservationBehaviour != SelfPreservation.Suicidal && attackToDodge == null && am.AtRisk(characterData))
        {
            dodgeCooldownTimer = 0; // Resets timer
            attackToDodge = am; // Specifies attack to dodge from
            aiStateMachine.SetBool("mustDodgeAttack", true); // Sets trigger so agent can dodge attack
        }
    }
    #endregion

    public void UpdateStateMachineVariables()
    {
        aiStateMachine.SetBool("targetAcquired", currentTarget != null);
        if (currentTarget != null)
        {
            aiStateMachine.SetFloat("targetDistance", Vector3.Distance(transform.position, currentTarget.transform.position));
        }
        aiStateMachine.SetFloat("targetNavMeshDistance", na.remainingDistance);
        aiStateMachine.SetInteger("health", hp.health.current);
    }

    public void UpdateAnimatorVariables()
    {
        animationController.SetBool("IsMoving", na.velocity.magnitude > 0);
        animationController.SetFloat("MovementSpeed", na.velocity.magnitude);
    }

    private void LateUpdate()
    {
        head.transform.LookAt(LookOrigin + LookDirection, transform.up);
    }
}
