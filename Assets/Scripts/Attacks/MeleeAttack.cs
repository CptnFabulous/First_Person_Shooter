using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
//using System.Linq;

[System.Serializable]
public class MeleeAttack : MonoBehaviour
{
    [Header("Damage stats")]
    public int damage = 20;
    public float knockback = 5;
    public float windupDuration = 0.1f;
    public float attackDuration = 0.1f;
    public float returnDuration = 0.1f;

    [Header("Detection stats")]
    /*
    public float minRange = 0.5f;
    public float maxRange = 1;
    public float swingWidth = 0.1f;
    public float swingLengthInDegrees = 90;
    public float swingAngle = 60;
    public int numberOfSegmentsForDetection = 9;
    public LayerMask hitDetection = ~0;

    MeshCollider hitDetectionCollider;
    */
    public Transform hitbox;
    public Transform neutral;
    public Transform swingStart;
    public Transform swingEnd;
    public Transform boxcastOrigin;
    public Vector3 boxcastDimensions; 
    public float hitDetectionRaycastLength;
    public LayerMask hitDetection = ~0;
    Collider[] exceptions;

    IEnumerator inProgressAttack;


    [Header("Cosmetics")]
    public Material debugMaterial;
    public UnityEvent effectsOnWindup;
    public UnityEvent effectsOnSwing;
    public UnityEvent effectsOnHit;



    private void Awake()
    {
        // Generate collision mesh
        //GenerateCollisionMesh();






        // Reset weapon position
        hitbox.SetPositionAndRotation(neutral.position, neutral.rotation);

        // Establish hit detection exceptions (e.g. if you somehow hit yourself)
        exceptions = GetComponentsInParent<Collider>();
    }

    /*
    void GenerateCollisionMesh()
    {
        // Generates the variables to calculate the dimensions of the collider
        float attackWidthFromCentre = swingWidth / 2;
        float segmentAngleLength = swingLengthInDegrees / numberOfSegmentsForDetection;

        Vector3 topBack = Vector3.forward * minRange;
        Vector3 topFront = Vector3.forward * maxRange;
        Vector3 bottomBack = Misc.AngledDirection(new Vector3(-segmentAngleLength, 0, 0), Vector3.forward, Vector3.up).normalized * minRange;
        Vector3 bottomFront = Misc.AngledDirection(new Vector3(-segmentAngleLength, 0, 0), Vector3.forward, Vector3.up).normalized * maxRange;

        // Generates the eight necessary corners for our detection prism.
        Vector3 topBackLeft = topBack + Vector3.left * attackWidthFromCentre;
        Vector3 topBackRight = topBack + Vector3.right * attackWidthFromCentre;
        Vector3 topFrontLeft = topFront + Vector3.left * attackWidthFromCentre;
        Vector3 topFrontRight = topFront + Vector3.right * attackWidthFromCentre;
        Vector3 bottomBackLeft = bottomBack + Vector3.left * attackWidthFromCentre;
        Vector3 bottomBackRight = bottomBack + Vector3.right * attackWidthFromCentre;
        Vector3 bottomFrontLeft = bottomFront + Vector3.left * attackWidthFromCentre;
        Vector3 bottomFrontRight = bottomFront + Vector3.right * attackWidthFromCentre;

        // Inputs the corners into an array usable in a mesh class. For some dumb reason you have to input a vertex multiple times for each face it's connected to.
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
        
        // Creates the triangles from each 
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
        };

        Mesh hitDetectionMesh = new Mesh();
        hitDetectionMesh.vertices = vertices;
        hitDetectionMesh.triangles = triangles;

        // Create mesh collider object
        GameObject meshObject = new GameObject(name + " - hit detection mesh", typeof(MeshFilter));
        meshObject.GetComponent<MeshFilter>().mesh = hitDetectionMesh;

        MeshCollider collider = meshObject.AddComponent<MeshCollider>();
        collider.sharedMesh = hitDetectionMesh;
        collider.isTrigger = true;

        if (debugMaterial != null)
        {
            MeshRenderer detectionVisual = meshObject.AddComponent<MeshRenderer>();
            detectionVisual.material = debugMaterial;
        }

        // Set up collision mesh to prepare for attack
        meshObject.transform.SetParent(transform);
        meshObject.transform.localPosition = Vector3.zero;
        meshObject.transform.localRotation = Quaternion.identity;
        meshObject.SetActive(false);
        hitDetectionCollider = collider;
    }
    */

    public void ExecuteAttack(Character origin)
    {
        //inProgressAttack = ColliderBasedAttack();
        inProgressAttack = Swing(origin);
        StartCoroutine(inProgressAttack);
    }
    
    
    IEnumerator Swing(Character origin)
    {
        #region Windup
        effectsOnWindup.Invoke();
        float timer = 0;
        while (timer <= 1)
        {
            hitbox.position = Vector3.Slerp(neutral.position, swingStart.position, timer);
            hitbox.rotation = Quaternion.Slerp(neutral.rotation, swingStart.rotation, timer);
            timer += Time.deltaTime / windupDuration;
            yield return new WaitForEndOfFrame();
        }
        #endregion

        effectsOnSwing.Invoke();

        // The list alreadyHit also contains the exceptions, since they're the same variables that can render a potential hit invalid
        List<Collider> alreadyHit = new List<Collider>(exceptions);
        List<Health> alreadyDamaged = new List<Health>();
        
        timer = 0;
        while (timer <= 1)
        {
            // Launches a boxcast
            RaycastHit[] hits = Physics.BoxCastAll(boxcastOrigin.position, new Vector3(boxcastDimensions.x / 2, boxcastDimensions.y / 2, boxcastDimensions.z / 2), boxcastOrigin.forward, boxcastOrigin.rotation, hitDetectionRaycastLength, hitDetection);
            foreach(RaycastHit hit in hits)
            {
                // If the collider wasn't already hit on a previous frame
                if (!alreadyHit.Contains(hit.collider))
                {
                    alreadyHit.Add(hit.collider);
                    effectsOnHit.Invoke();

                    // Checks if the object is damageable, and if it has already been damaged
                    DamageHitbox dh = hit.collider.GetComponent<DamageHitbox>();
                    if (dh != null && dh.healthScript != null && !alreadyDamaged.Contains(dh.healthScript))
                    {
                        dh.Damage(damage, origin, DamageType.Bludgeoned, false);
                        alreadyDamaged.Add(dh.healthScript);
                    }
                }
            }

            hitbox.position = Vector3.Lerp(swingStart.position, swingEnd.position, timer);
            hitbox.rotation = Quaternion.Lerp(swingStart.rotation, swingEnd.rotation, timer);
            timer += Time.deltaTime / windupDuration;
            yield return new WaitForEndOfFrame();
        }

        #region End swing
        timer = 0;
        while (timer <= 1)
        {
            hitbox.position = Vector3.Slerp(swingEnd.position, neutral.position, timer);
            hitbox.rotation = Quaternion.Slerp(swingEnd.rotation, neutral.rotation, timer);
            timer += Time.deltaTime / windupDuration;
            yield return new WaitForEndOfFrame();
        }
        #endregion

        EndAttack();
    }

    /*
    IEnumerator Thrust()
    {

    }
    */
    
    
    /*
    IEnumerator ColliderBasedAttack()
    {
        Debug.Log("Winding up attack from " + name);
        effectsOnWindup.Invoke();
        yield return new WaitForSeconds(windupDuration);

        Debug.Log("Commencing attack from " + name);
        effectsOnSwing.Invoke();

        hitDetectionCollider.gameObject.SetActive(true);

        float checkCounter = 0;
        while (checkCounter < numberOfSegmentsForDetection)
        {
            Debug.Log("Detection " + checkCounter + " out of " + numberOfSegmentsForDetection);
            
            float detectionAngle = ((swingLengthInDegrees / numberOfSegmentsForDetection) * checkCounter) - (swingLengthInDegrees / 2);
            hitDetectionCollider.transform.localRotation = Quaternion.Euler(detectionAngle, 0, 0);
            hitDetectionCollider.transform.localRotation = Quaternion.Euler(0, 0, swingAngle) * hitDetectionCollider.transform.localRotation;
            hitDetectionCollider.transform.Rotate(0, 0, swingAngle);
            // Do attack stuff


            

            if (checkCounter < numberOfSegmentsForDetection - 1)
            {
                yield return new WaitForSeconds(attackDuration / numberOfSegmentsForDetection);
            }
            
            checkCounter += 1;
        }

        hitDetectionCollider.gameObject.SetActive(false);

        EndAttack();
    }
    */

    public void EndAttack()
    {
        StopCoroutine(inProgressAttack);
        inProgressAttack = null;
        hitbox.SetPositionAndRotation(neutral.position, neutral.rotation);
    }
}
