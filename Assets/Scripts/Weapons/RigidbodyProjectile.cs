using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodyProjectile : MonoBehaviour {

    [Header("Impact properties")]
    public float detectionCastLength;
    public float projectileLifetime;

    /*
    [Header("Damage")]
    [Tooltip("Damage points dealt upon a successful hit, per projectile.")]
    public int damage;
    [Tooltip("Time taken to reload the weapon, in seconds.")]
    public int damageCritical;
    [Tooltip("Radius in which splash damage is dealt.")]
    public float splashRadius;
    [Tooltip("The way that splash damage decreases the further an enemy is from the explosion. Currently does not work.")]
    public AnimationCurve splashFalloff;
    [Tooltip("The amount of force applied to an enemy struck by the projectile.")]
    public float hitKnockback;
    [Tooltip("The amount of force applied to enemies caught in the weapon's splash radius.")]
    public float splashKnockback;
    */


    [Header("Visual effects")]
    [Tooltip("What the projectile looks like")]
    public GameObject projectileModel;
    [Tooltip("Particle effect while moving through air")]
    public ParticleSystem flightEffect;
    [Tooltip("Particle effect on impact with surface")]
    public GameObject impactEffect;
    [Tooltip("Noise made while moving through air")]
    public AudioClip flightNoise;
    [Tooltip("Noise made on impact with surface")]
    public AudioClip impactNoise;
    
    

    float lifetimeTimer;

    bool isShooting;

    Ray impact;
    RaycastHit impactPoint;


    // Use this for initialization
    void Start ()
    {
        isShooting = true;
	}
	
	// Update is called once per frame
	void Update ()
    {
        /*
        if (isShooting == true)
        {
            AudioSource.PlayClipAtPoint(flightNoise, transform.position);
        }
        */

        impact.origin = transform.position; // Sets the origin of isGrounded ray to the projectile
        impact.direction = transform.up; // Sets isGrounded direction to cast directly forward from the projectile tip (the projectile faces upwards)

        CapsuleCollider cc = GetComponent<CapsuleCollider>();

        //Raycast impact is cast to detect if the projectile has hit a target. Less resource-intensive than OnCollisionEnter().
        if (Physics.SphereCast(impact, cc.radius, (cc.height) / 2 + 0.01f))
        {
            print("Projectile hit");
            Instantiate(impactEffect, transform.position, Quaternion.identity);
            //AudioSource.PlayClipAtPoint(impactNoise, transform.position);

            // do stuff that a projectile would do when it hits a target

            Destroy(gameObject);
        }

        lifetimeTimer += Time.deltaTime;
        if (lifetimeTimer >= projectileLifetime)
        {
            Destroy(gameObject);
        }


    }

    /*

    void OnCollisionEnter(Collision surface)
    {
        //Determine if collision target is an enemy (what is the best way to do this?), deal damage accordingly

        isShooting = false;

        Instantiate(impactEffect, transform.position, Quaternion.identity);
        AudioSource.PlayClipAtPoint(impactNoise, transform.position);

    }
    */

}
