using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    // Bullet physics stats
    [HideInInspector] public float mass;
    [HideInInspector] public float radius;
    [HideInInspector] public float velocity;
    [HideInInspector] public bool gravityAffected;
    [HideInInspector] public float resolution;

    // Damage stats
    [HideInInspector] public int damage;
    [HideInInspector] public float criticalModifier;

    // Visual effect stats
    [HideInInspector] public GameObject projectileVisual;
    [HideInInspector] public GameObject impactPrefab;


    public float projectileLifetime;
    float timerLifetime;

    Ray bulletRay; // Raycast launched to determine shot direction
    RaycastHit bulletHit; // Point where raycast hits target
    LayerMask ignorePlayer = ~(1 << 8); // LayerMask ensuring raycast does not hit player's own body

    // Use this for initialization
    void Start()
    {
        bulletRay.origin = transform.position;
        bulletRay.direction = transform.forward;
    }

    // Update is called once per frame
    void Update()
    {
        resolution = velocity * Time.deltaTime;

        if (Physics.SphereCast(bulletRay, radius, out bulletHit, resolution, ignorePlayer))
        {
            Instantiate(impactPrefab, bulletHit.point, Quaternion.LookRotation(bulletHit.normal));

            // do stuff like deal damage, spawn impact prefab
            Destroy(gameObject);
        }
        else
        {
            transform.position += transform.forward * resolution; // Moves entire bullet gameobject
            bulletRay.origin = transform.position;

            if (gravityAffected)
            {
                Vector3 gravityModifier = transform.position + transform.forward * resolution + Physics.gravity * mass * Time.deltaTime;
                transform.LookAt(gravityModifier);
                bulletRay.direction = transform.forward;
                // Update direction
            }
        }

        timerLifetime += Time.deltaTime;
        if (timerLifetime >= projectileLifetime)
        {
            Destroy(gameObject);
        }

        ProjectileVisuals();
    }

    void ProjectileVisuals()
    {
        projectileVisual.transform.rotation = transform.rotation;
        projectileVisual.transform.position += transform.up * resolution;
    }
}
