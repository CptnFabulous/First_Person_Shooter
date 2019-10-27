using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float projectileLifetime = 20;

    [Header("Physics")]
    public float velocity;
    public float gravityMultiplier;
    public float diameter;
    public LayerMask hitDetection; // LayerMask ensuring raycast does not hit player's own body

    Vector3 desiredVelocity; // Intended direction the projectile is meant to travel in, this is set at the start of the projectile's lifetime
    Vector3 ballisticDirection; // The direction the projectile will actualy go in
    Vector3 gravityModifier; // An increasing Vector3 value to slowly drag the projectile down with gravity
    public RaycastHit projectileHit; // Point where raycast hits target
    float timerLifetime;

    [HideInInspector] public GameObject origin;
    [HideInInspector] public Faction originFaction;

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
        if (Physics.SphereCast(transform.position, diameter / 2, transform.forward, out projectileHit, raycastLength, hitDetection) && IsAlly(projectileHit.collider.gameObject) == false)
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

    public virtual void OnHit()
    {
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

    bool IsAlly(GameObject g)
    {
        Character ch = Character.FromHit(g);
        if (ch != null && originFaction.Affiliation(ch.faction) == FactionState.Allied)
        {
            return true;
        }
        return false;
    }
}
