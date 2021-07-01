using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEditor;
using UnityEngine;
//using System.Linq;

[System.Serializable]
public class MeleeAttack : MonoBehaviour
{
    [Header("Damage stats")]
    public int damage = 20;
    public float windupDuration = 0.2f;
    public float attackDuration = 0.2f;
    public float cooldownDuration = 0.1f;

    [Header("Detection stats")]
    public float minRange = 0.5f;
    public float maxRange = 1;
    public Vector2 attackDimensionsInDegrees = new Vector2(10, 90);
    public float swingAngle = 60;
    int numberOfSegmentsForDetection = 9;
    public LayerMask hitDetection = ~0;

    IEnumerator inProgressAttack;


    private void Start()
    {
        
    }

    void GenerateCollisionMesh()
    {
        float attackWidthFromCentre = attackWidthInDegrees / 2;
        float segmentAngleWidth = attackLengthInDegrees / numberOfSegmentsForDetection;

        Vector3 topBack = Vector3.forward * minRange;
        Vector3 topFront = Vector3.forward * maxRange;
        Vector3 bottomBack = Misc.AngledDirection(new Vector3(segmentAngleWidth, 0, 0), Vector3.forward, Vector3.up).normalized * minRange;
        Vector3 bottomFront = Misc.AngledDirection(new Vector3(segmentAngleWidth, 0, 0), Vector3.forward, Vector3.up).normalized * maxRange;

        /*
        Vector3 topBackLeft = topBack + Vector3.left * attackWidthFromCentre;
        Vector3 topBackRight = topBack + Vector3.right * attackWidthFromCentre;
        Vector3 topFrontLeft = topFront + Vector3.left * attackWidthFromCentre;
        Vector3 topFrontRight = topFront + Vector3.right * attackWidthFromCentre;
        Vector3 bottomBackLeft = bottomBack + Vector3.left * attackWidthFromCentre;
        Vector3 bottomBackRight = bottomBack + Vector3.right * attackWidthFromCentre;
        Vector3 bottomFrontLeft = bottomFront + Vector3.left * attackWidthFromCentre;
        Vector3 bottomFrontRight = bottomFront + Vector3.right * attackWidthFromCentre;
        */

        Vector3 topBackLeft = new Vector3(-0.5f, 0.5f, -0.5f);
        Vector3 topBackRight = new Vector3(0.5f, 0.5f, -0.5f);
        Vector3 topFrontLeft = new Vector3(-0.5f, 0.5f, 0.5f);
        Vector3 topFrontRight = new Vector3(0.5f, 0.5f, 0.5f);
        Vector3 bottomBackLeft = new Vector3(-0.5f, -0.5f, -0.5f);
        Vector3 bottomBackRight = new Vector3(0.5f, -0.5f, -0.5f);
        Vector3 bottomFrontLeft = new Vector3(-0.5f, -0.5f, 0.5f);
        Vector3 bottomFrontRight = new Vector3(0.5f, -0.5f, 0.5f);

        Vector3[] vertices = new Vector3[]
        {
            // Back
            topBackLeft,
            topBackRight,
            bottomBackLeft,
            bottomBackRight,
            // Front
            topFrontRight,
            topFrontLeft,
            bottomFrontRight,
            bottomFrontLeft,
            // Top
            topFrontLeft,
            topFrontRight,
            topBackLeft,
            topBackRight,
            // Bottom
            bottomFrontLeft,
            bottomFrontRight,
            bottomBackLeft,
            bottomBackRight,
            // Left
            topFrontLeft,
            topBackLeft,
            bottomFrontLeft,
            bottomBackLeft,
            // Right
            topBackRight,
            topFrontRight,
            bottomBackRight,
            bottomFrontRight,
        };
        int[] triangles = new int[]
        {
            
            // Back
            0,2,1,2,3,1,
            // Front
            4,6,5,6,7,5,
            // Top
            8,10,9,10,11,9,
            // Bottom
            12,13,14,14,13,15,
            // Left
            16,18,17,18,19,17,
            // Right
            20,22,21,22,23,21
            
            /*
            // Back
            0,1,2,2,1,3,
            // Front
            4,5,6,6,5,7,
            // Top
            8,9,10,10,9,11,
            // Bottom
            12,13,14,14,13,15,
            // Left
            16,17,18,18,17,19,
            // Right
            20,21,22,22,21,23
            */
        };

        Mesh hitDetectionMesh = new Mesh();

        hitDetectionMesh.vertices = vertices;
        //hitDetectionMesh.uv = uv;
        hitDetectionMesh.triangles = triangles;

        GameObject meshObject = new GameObject("Hit Detection Mesh", typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider));

        meshObject.GetComponent<MeshFilter>().mesh = hitDetectionMesh;
        meshObject.GetComponent<MeshCollider>().sharedMesh = hitDetectionMesh;
        //meshObject.GetComponent<MeshRenderer>().material = meshMaterial;

        meshObject.transform.SetParent(transform);

        meshObject.transform.localPosition = Vector3.zero;
        meshObject.transform.localRotation = Quaternion.identity;
    }






    public void ExecuteAttack(Vector3 origin, Vector3 forward, Vector3 up)
    {
        inProgressAttack = SwingAttackSequence(origin, forward, up);
        StartCoroutine(inProgressAttack);
    }

    public void EndAttack()
    {
        StopCoroutine(inProgressAttack);
        inProgressAttack = null;
    }

    IEnumerator SwingAttackSequence(Vector3 origin, Vector3 forward, Vector3 up)
    {
        Debug.Log("Winding up attack from " + name);
        
        yield return new WaitForSeconds(windupDuration);


        Debug.Log("Commencing attack from " + name);

        


        float timer = 0;

        while (timer < 1)
        {
            // Attack?



            Quaternion angleThisFrame = Quaternion.Euler(0, (attackDimensionsInDegrees.y * timer) - (attackDimensionsInDegrees.y / 2), swingAngle);



            timer += Time.deltaTime / attackDuration;
            yield return new WaitForEndOfFrame();
        }



    }







    /*


    public IEnumerator PlayerSnapAttack(PlayerHandler origin, Vector3 attackOrigin, Vector3 direction)
    {
        DamageHitbox target = FindEnemyHitbox(origin, attackOrigin, direction);

        if (target == null)
        {
            //Play swing animation
        }


        // Sets up loop
        isAttacking = true;
        float timer = 0;


        // Temporarily disables player controller
        origin.pc.canMove = false;




        Vector3 oldLookDirection = origin.pc.head.transform.forward;
        Vector3 oldPosition = origin.transform.position;

        while (timer <= 1)
        {
            // Count up timer
            timer += Time.deltaTime / attackTime;

            // If the isAttacking bool has been remotely disabled, and end attack prematurely
            if (isAttacking == false)
            {
                EndAttack();
                yield break;
            }

            // Lerp player rotation so they are looking at the enemy
            Vector3 newLookDirection = target.transform.position - origin.transform.position;
            origin.pc.LookAt(Vector3.Lerp(oldLookDirection, newLookDirection, timer));

            // Lerp player position closer to enemy

            yield return new WaitForEndOfFrame();
        }

        // Execute attack (play animation and deal damage)
        target.Damage(damage, origin, DamageType.Bludgeoned, false);


        EndAttack();
    }

   

    

    */


    /*
    DamageHitbox FindEnemyHitbox(Character origin, Vector3 attackOrigin, Vector3 direction)
    {
        // Run a vision cone to check for entities inside the attack angle and range
        RaycastHit[] hits = AIFunction.VisionCone(attackOrigin, direction, origin.transform.up, angle, range, hitDetection, hitDetection);

        // Create a 'target' variable to determine who the 
        DamageHitbox target = null;
        float bestRange = range * 2;

        // Determine the closest hitbox that is an enemy of the attacker
        foreach (RaycastHit rh in hits)
        {
            // Check for damage hitbox script
            DamageHitbox closestHitbox = rh.collider.GetComponent<DamageHitbox>();
            if (closestHitbox != null)
            {
                // Check damage hitbox for character script (if the object detected is part of an entity who can be damaged)
                Character ch = Character.FromObject(closestHitbox.gameObject);
                if (ch != null && origin.HostileTowards(ch))
                {
                    // Check if the collider is inside the angle and range to find the hitbox closest to the origin
                    float r = AIFunction.ComplexColliderDistance(rh.collider, attackOrigin);
                    float a = AIFunction.ComplexColliderAngle(rh.collider, attackOrigin, direction);
                    if (r < bestRange && a < angle)
                    {
                        target = closestHitbox;
                        bestRange = r;
                    }
                }
            }
        }

        return target;
    }

    public void Cancel()
    {
        isAttacking = false;
    }
    */
}
