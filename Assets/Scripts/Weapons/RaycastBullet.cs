using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RaycastBullet : MonoBehaviour
{
    // Bullet physics stats
    [HideInInspector] public float gravityMultiplier;
    [HideInInspector] public float diameter;
    [HideInInspector] public float velocity;
    Vector3 desiredVelocity;
    Vector3 ballisticDirection;
    Vector3 gravityModifier;

    // Damage stats
    [HideInInspector] public int damage;
    [HideInInspector] public float criticalModifier;
    [HideInInspector] public DamageType typeOfDamage;

    // Visual effect variables
    [HideInInspector] public GameObject impactPrefab;
    LineRenderer bulletEffect;

    // Raycast variables
    Ray bulletRay; // Raycast launched to determine shot direction
    RaycastHit bulletHit; // Point where raycast hits target
    [HideInInspector] public LayerMask rayDetection; // LayerMask ensuring raycast does not hit player's own body

    public float projectileLifetime;
    float timerLifetime;

    

    
    

    // Use this for initialization
    void Start()
    {
        bulletRay.origin = transform.position;
        bulletRay.direction = transform.forward;

        desiredVelocity = transform.forward * velocity; // Creates intended direction and velocity for projectile to travel when it first spawns
        ballisticDirection = transform.position + (desiredVelocity * Time.deltaTime);
        //ballisticDirection = transform.position + desiredVelocity * Time.deltaTime;
    }

    // Update is called once per frame
    void Update()
    {
        bulletRay.origin = transform.position;
        bulletRay.direction = transform.forward;
        float raycastLength = Vector3.Distance(transform.position, ballisticDirection);
        if (Physics.SphereCast(bulletRay, diameter / 2, out bulletHit, raycastLength, rayDetection))
        {
            OnHit();
        }
        else
        {
            MoveBullet();
        }

        timerLifetime += Time.deltaTime;
        if (timerLifetime >= projectileLifetime)
        {
            Destroy(gameObject);
        }
    }

    void OnHit()
    {
        //Instantiate(impactPrefab, bulletHit.point, Quaternion.LookRotation(bulletHit.normal));

        // do stuff like deal damage, spawn impact prefab
        //print("Hit something");

        Health killable = bulletHit.collider.GetComponent<Health>();
        if (killable != null)
        {
            killable.TakeDamage(damage, DamageType.Shot);
        }

        /* For critical hit damage
        if (killable != null && [if hitbox has critical hit tag])
        {
            killable.TakeDamage(Mathf.RoundToInt(damage * criticalModifier), DamageType.CriticalShot);
        }
        else if (killable != null)
        {
            killable.TakeDamage(damage, DamageType.Shot);
        }
        */

        Destroy(gameObject);
    }

    void MoveBullet()
    {
        transform.position = ballisticDirection; // Moves bullet forwards according to ballisticDirection

        ballisticDirection = transform.position + (desiredVelocity * Time.deltaTime); // Updates ballisticDirection to be relative to bullet's new position
        if (gravityMultiplier > 0) // Mass/gravity code, is ignored when gravityMultiplier is zero for bullets unaffected by gravity
        {
            gravityModifier += Physics.gravity * gravityMultiplier * Time.deltaTime; // gravity force slowly increases 
            if (Time.timeScale != 0)  // Only applies gravity if time is actually moving in either direction, to ensure gravity does not apply while paused. This feels like a band-aid solution equivalent to a bool saying 'ifUnpaused' and may need changing
            {
                ballisticDirection += gravityModifier; // Adds gravity modifier to ballisticDirection, so it affects the projectile's velocity
            }
            transform.LookAt(ballisticDirection); // Rotates projectile to point in direction it is about to move to appropriately calculate raycasts next frame
        }

        /*
        ballisticDirection = transform.position + (desiredVelocity * Time.deltaTime);
        if (gravityMultiplier > 0) // Currently downwards velocity will continuously increase and may override what speed the bullet would naturally go if it was shot downwards. This may need fixing but for now it's not important.
        {
            gravityModifier += Physics.gravity * gravityMultiplier * Time.deltaTime;
            //print("Gravity modifier = " + gravityModifier);
            ballisticDirection += gravityModifier;
            transform.LookAt(ballisticDirection);
        }
        */
    }
}
