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
    public float pursuePatience = 10;
    float patienceTimer = float.MaxValue;

    public Character target;

    


    [Header("Self-preservation")]
    public SelfPreservation selfPreservationBehaviour;
    public AttackMessage attackToDodge;
    public float dodgeCooldown;
    float dodgeCooldownTimer;





    [Header("Looking at stuff")]
    public float lookSpeed;
    public float lookThreshold;
    bool inLookIENumerator;
    Vector3 aimMarker;
    public float reactionTime;

    public virtual void Awake()
    {
        stateMachine = GetComponent<Animator>();
        hp = GetComponent<NpcHealth>();
        na = GetComponent<NavMeshAgent>();
        characterData = GetComponent<Character>();
        audioOutput = GetComponent<AudioSource>();

        eo = GetComponent<EventObserver>();
        eo.OnAttack += Dodge;
        //EventObserver.AddAttackReceiver(Dodge, this);
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
        dodgeCooldownTimer += Time.deltaTime;

        #region Check for targets (upgrade this to something snazzier)
        bool targetAcquired = target != null;
        if (target == null) // Checks for targets
        {
            target = AcquireTarget();
        }
        else // if a target has already been acquired
        {
            #region Check if out of range and cancel pursuit after a timer
            // If the AI cannot immediately find their target, count up a timer and continue pursuing until the timer expires
            if (Vector3.Distance(transform.position, target.transform.position) > pursueRange || AIFunction.SimpleLineOfSightCheck(target.transform.position, head.position, viewDetection) == false)
            {
                patienceTimer = 0;
            }

            patienceTimer += Time.deltaTime;

            if (patienceTimer >= pursuePatience)
            {
                print("Target out of range");
                target = null;
            }
            #endregion

            Health h = target.GetComponent<Health>();
            if (h != null && h.IsDead)
            {
                target = null;
            }
        }
        #endregion

        #region Set state machine variables
        stateMachine.SetBool("targetAcquired", target != null);
        if (target != null)
        {
            stateMachine.SetFloat("targetDistance", Vector3.Distance(transform.position, target.transform.position));
        }
        stateMachine.SetFloat("targetNavMeshDistance", na.remainingDistance);
        stateMachine.SetInteger("health", hp.values.current);
        #endregion
    }

    #region Looking at stuff
    public void LookTowards(Vector3 position, float degreesPerSecond)
    {
        Quaternion correctRotation = Quaternion.LookRotation(position, transform.up);
        head.transform.rotation = Quaternion.RotateTowards(head.transform.rotation, correctRotation, degreesPerSecond * Time.deltaTime);
    }

    public bool IsLookingAt(Vector3 position, float threshold)
    {
        if (Vector3.Angle(position - head.transform.position, head.transform.forward) < threshold)
        {
            return true;
        }
        return false;
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
            stateMachine.SetBool("mustDodgeAttack", true); // Sets trigger so agent can dodge attack
        }
    }


    #region Target acquisition + checking
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


    bool CanFindTarget()
    {
        return false;
    }


    #endregion



}