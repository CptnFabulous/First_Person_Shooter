using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DodgeAreaOfEffect : AIMovementBehaviour
{
    /*
    public float dodgeRadius; // The minimum distance required to dodge. This needs to update based on the attack being dodged.
    public LayerMask terrainDetection;
    public int checkRaycastNumber = 8;

    public Transform attacker;
    */
    [Header("Dodge directional attack")]
    public float minimumDodgeDistance;
    public int checkRaycastNumber;
    public LayerMask terrainDetection;

    [Header("Dodge area of effect")]
    public float minimumDodgeRadius;

    Transform attacker;
    NullableVector3 dodgeLocation;
    
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);




        //dodgeLocation = Dodge(attacker, minimumDodgeDistance, checkRaycastNumber, terrainDetection);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Setting destination for " + ai.name + " from DodgeAreaOfEffect behaviour");
        ai.na.SetDestination(dodgeLocation.position);
    }

    /*
    NullableVector3 Dodge(Transform attackerLocation, float minimumDodgeDistance, int checkRaycastNumber, LayerMask terrainDetection)
    {
        
    }
    */
}
