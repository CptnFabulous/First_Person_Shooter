﻿using System.Collections;
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


        DamageHitbox hitbox = bulletHit.collider.GetComponent<DamageHitbox>();
        if (hitbox != null)
        {
            if (hitbox.critical == true)
            {
                hitbox.Damage(Mathf.RoundToInt(damage * criticalModifier), DamageType.CriticalShot);
            }
            else
            {
                hitbox.Damage(damage, DamageType.Shot);
            }
        }

        Destroy(gameObject);
    }

    void MoveBullet()
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
}
