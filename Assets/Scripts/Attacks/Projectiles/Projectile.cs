using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Projectile : MonoBehaviour
{
    [Header("General stats")]
    public float projectileLifetime = 20;
    public UnityEvent onHit;

    [Header("Physics")]
    public float velocity;
    public float gravityMultiplier;
    public float diameter;
    public LayerMask hitDetection = ~0; // LayerMask ensuring raycast does not hit player's own body

    Vector3 desiredVelocity; // Intended direction the projectile is meant to travel in, this is set at the start of the projectile's lifetime
    Vector3 ballisticDirection; // The direction the projectile will actualy go in
    Vector3 gravityModifier; // An increasing Vector3 value to slowly drag the projectile down with gravity

    RaycastHit hit;

    float timerLifetime;

    [HideInInspector] public Character origin;

    // Use this for initialization
    void Start()
    {
        desiredVelocity = transform.forward * velocity; // Creates intended direction and velocity for projectile to travel when it first spawns
        ballisticDirection = transform.position + (desiredVelocity * Time.deltaTime);
    }

    // Update is called once per frame
    void Update()
    {
        float raycastLength = Vector3.Distance(transform.position, ballisticDirection);
        if (Physics.SphereCast(transform.position, diameter / 2, transform.forward, out hit, raycastLength, hitDetection))
        {
            OnHit(hit);
        }

        Move();

        timerLifetime += Time.deltaTime;
        if (timerLifetime >= projectileLifetime)
        {
            Destroy(gameObject);
        }
    }

    public virtual void OnHit(RaycastHit rh)
    {
        onHit.Invoke();
        Destroy(gameObject);
    }

    void Move()
    {
        transform.position = ballisticDirection; // Moves bullet forwards according to ballisticDirection
        ballisticDirection = transform.position + (desiredVelocity * Time.deltaTime); // Updates ballisticDirection to be relative to bullet's new position
        if (gravityMultiplier > 0 && Time.timeScale != 0) // If projectile is actually affected by gravity, and if time is moving in either direction
        {
            gravityModifier += Physics.gravity * gravityMultiplier * Time.deltaTime; // gravity force slowly increases based on Time.deltaTime, which factors in framerates and the speed the game is moving at
            ballisticDirection += gravityModifier; // gravityModifier is added to ballisticDirection so that the bullet attempts to move in the original direction but is dragged down by gravity
            transform.LookAt(ballisticDirection); // Rotates projectile to point in direction it is about to move to appropriately calculate raycasts next frame
        }
    }

    void Ricochet(RaycastHit rh)
    {
        //Vector3.Reflect based on normal of raycasthit
        // Reduce velocity (multiplicative or additive? Should I put velocity dampener in projectile itself or set it based on the physics material?)
    }


    public void SpawnObjectAtPoint(GameObject prefab)
    {
        Instantiate(prefab, hit.point, Quaternion.identity);
    }

    public void SpawnObjectOnSurface(GameObject prefab)
    {
        GameObject newObject = Instantiate(prefab);
        AttachObjectToHitSurface(newObject.transform, hit, Vector3.forward, 0);
    }

    public static void AttachObjectToHitSurface(Transform objectTransform, RaycastHit hit, Vector3 rotationAxis, float distanceOffSurface)
    {
        objectTransform.position = hit.point + (hit.normal * distanceOffSurface);
        objectTransform.rotation = Quaternion.FromToRotation(rotationAxis, hit.normal);
        objectTransform.parent = hit.transform;
    }
}