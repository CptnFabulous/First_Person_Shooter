using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Dodge : AIMovementBehaviour
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




        dodgeLocation = DodgeDirectionalAttack(attacker, minimumDodgeDistance, checkRaycastNumber, terrainDetection);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ai.na.SetDestination(dodgeLocation.position);
    }
    /*
    class NullableVector3
    {
        public Vector3 position;
    }
    */
    NullableVector3 DodgeDirectionalAttack(Transform attackerLocation, float minimumDodgeDistance, int checkRaycastNumber, LayerMask terrainDetection)
    {
        #region Launch raycasts to check for occupied space
        Vector3 attackerDirection = attackerLocation.position - ai.transform.position;
        // Find locations perpendicular to the attacker

        bool[] emptySpaces = new bool[checkRaycastNumber];
        for (int i = 0; i < checkRaycastNumber; i++)
        {
            emptySpaces[i] = true;

            Quaternion scanDirection = Quaternion.Euler(90, 0, (360 / checkRaycastNumber) * i);
            if (Physics.Raycast(ai.transform.position, scanDirection * attackerDirection, minimumDodgeDistance, terrainDetection))
            {
                emptySpaces[i] = false;
            }
        }
        #endregion

        #region Check if there is any empty space to dodge to
        bool spaceFound = false;
        foreach(bool b in emptySpaces)
        {
            if (b == true)
            {
                spaceFound = true;
            }
        }
        if (spaceFound == false)
        {
            return null;
        }
        #endregion

        #region Average empty space directions to obtain ideal space to dodge to
        // Averages directions of raycasts that return true, to determine the most empty space to dodge in.
        // This may need to be updated to remove outliers!
        float average = 0;
        int sampleSize = 0;
        for (int i = 0; i < emptySpaces.Length; i++)
        {
            if (emptySpaces[i] == true)
            {
                average += i;
                sampleSize += 1;
            }
        }
        average /= sampleSize;

        // t f f t t t t f f f t t f
        #endregion

        #region Produce new dodge location and verify with NavMesh.SamplePosition
        Vector3 dodgeDirection = (Quaternion.Euler(90, 0, (360 / checkRaycastNumber) * average) * attackerDirection) * minimumDodgeDistance; // Produces a Vector3 obtained from moving a distance of minimumDodgeDistance in the desired dodge direction

        NavMeshHit meshLocation;
        if (NavMesh.SamplePosition(dodgeDirection, out meshLocation, 2 * ai.na.height, NavMesh.AllAreas))
        {
            return new NullableVector3 { position = meshLocation.position };
        }

        return null;
        #endregion
    }
}
