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
    [Range(0, 180)] public float xFOV = 60;
    [Range(0, 180)] public float yFOV = 60;
    public LayerMask viewDetection = ~0;
    public float pursueRange = 60;
    public float pursuePatience = 10;
    float patienceTimer = float.MaxValue;

    [Header("Self-preservation")]
    public SelfPreservation selfPreservationBehaviour;
    public AttackMessage attackToDodge;
    public float dodgeCooldown = 2;
    float dodgeCooldownTimer;

    [Header("Looking at stuff")]
    public float lookSpeed = 120;
    public float lookThreshold = 0.2f;
    bool inLookIENumerator;

    public Quaternion LookDirectionValue { get; private set; }
    public Vector3 AimMarker { get; private set; }

    bool notPresentlyLookingAtSomethingSpecific = true;


    public float reactionTime = 0.5f;

    public virtual void Awake()
    {
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



        PursueTargetUpdate();



        UpdateStateMachineVariables();

        if (animationController != null)
        {
            UpdateAnimatorVariables();
        }
    }

    #region Looking around

    public void TrackLookTowards(Vector3 position, float unitsPerSecond)
    {
        if (notPresentlyLookingAtSomethingSpecific)
        {
            notPresentlyLookingAtSomethingSpecific = false;
            AimMarker = head.position;
            LookDirectionValue = head.rotation;
        }
        
        AimMarker = Vector3.MoveTowards(AimMarker, position, unitsPerSecond * Time.deltaTime);
        head.transform.LookAt(position, transform.up);
    }

    public bool DistanceIsLookingAt(Vector3 position, float threshold)
    {
        if (Vector3.Distance(AimMarker, position) <= threshold)
        {
            return true;
        }
        return false;
    }
    
    public void RotateLookTowards(Vector3 position, float degreesPerSecond)
    {
        if (notPresentlyLookingAtSomethingSpecific)
        {
            notPresentlyLookingAtSomethingSpecific = false;
            AimMarker = head.position;
            LookDirectionValue = head.rotation;
        }

        Quaternion correctRotation = Quaternion.LookRotation(position - head.transform.position, transform.up);
        LookDirectionValue = Quaternion.RotateTowards(LookDirectionValue, correctRotation, degreesPerSecond * Time.deltaTime);
        head.transform.rotation = LookDirectionValue;
    }

    public bool AngleIsLookingAt(Vector3 position, float threshold)
    {
        if (Vector3.Angle(position - head.transform.position, head.transform.forward) <= threshold)
        {
            return true;
        }
        return false;
    }
    
    public void ResetLookDirection()
    {
        notPresentlyLookingAtSomethingSpecific = true;
        AimMarker = head.transform.position;
        LookDirectionValue = Quaternion.Euler(0, 0, 0);
        head.localRotation = LookDirectionValue; // Head position is reset
    }

    public bool IsTargetWithinRange(Vector3 position, float threshold)
    {
        // Calculates distance between entity and target
        float distanceToTarget = Vector3.Distance(head.transform.position, position);
        // Calculates a position that's the same distance out as the target, but in the direction the AI is currently aiming.
        Vector3 positionInAimDirectionButCloseToTarget = (position - head.transform.position).normalized * distanceToTarget;
        if (Vector3.Distance(position, positionInAimDirectionButCloseToTarget) <= threshold)
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

        while (timer < 1)
        {
            timer += Time.deltaTime / lookTime;

            Quaternion lookLerp = Quaternion.Lerp(originalRotation, Quaternion.LookRotation(position, transform.up), lookCurve.Evaluate(timer));
            head.transform.rotation = lookLerp;

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
            if (Vector3.Distance(transform.position, currentTarget.transform.position) > pursueRange || AIFunction.SimpleLineOfSightCheck(currentTarget.transform.position, head.position, viewDetection) == false)
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
        Collider[] thingsInEnvironment = Physics.OverlapSphere(head.transform.position, viewRange);
        foreach (Collider thing in thingsInEnvironment)
        {
            if (AIFunction.LineOfSight(head.position, thing.transform, viewDetection))
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
            // Resets timer
            dodgeCooldownTimer = 0;

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
}
