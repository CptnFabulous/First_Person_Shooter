using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// The EventHandler script will exist on a gameobject located in the scene.
// All EventObserver scripts will reference this script
// EventObserver scripts will add their functions to the appropriate delegates when enabled, and remove them when disabled.
// When an appropriate game event happens, that function will use a Transmit function to search for this EventHandler and trigger the referenced functions in the appropriate delegate.

public class EventHandler : MonoBehaviour
{
    public List<EventObserver> eventObservers = new List<EventObserver>();
    // Add more delegates, functions, etc. if I need to add new game events in the future
}

public class AttackMessage
{
    public enum AttackType
    {
        Ranged,
        Melee,
        AreaOfEffect,
        ExplosiveRanged,
    }
    
    public Character attacker; // The entity performing the attack
    public AttackType type;
    public LayerMask hitDetection;

    // Important data about attack zone for enemy avoidance
    public Vector3 origin;
    // Directional ranged attacks
    public Vector3 direction; // The direction of the attack
    public float maxRange; // The maximum range of the attack
    public float projectileDiameter; // How wide the initial attack or projectile will be
    public float coneAngle; // How much the attack can deviate in terms of direction
    public float velocity; // Speed at which the attack will move through the air
    // Area of effect
    public Vector3 impactPosition; // The point that the area of effect emanates from
    public float effectRadius; // The maximum distance the area of effect will emanate
    // Melee
    public float delay; // Duration of telegraph before the melee attack deals damage

    public static AttackMessage Ranged(Character attacker, Vector3 origin, Vector3 direction, float maxRange, float projectileDiameter, float coneAngle, float velocity, LayerMask hitDetection)
    {
        return new AttackMessage { attacker = attacker, type = AttackType.Ranged, origin = origin, direction = direction, maxRange = maxRange, projectileDiameter = projectileDiameter, coneAngle = coneAngle, velocity = velocity, hitDetection = hitDetection };
    }

    public static AttackMessage Melee(Character attacker, Vector3 direction, float radius, float angle, float maxRange, float delay)
    {
        return null;
    }

    public static AttackMessage AreaOfEffect(Character attacker, Vector3 point, float effectRadius)
    {
        return null;
    }

    public static AttackMessage ExplosiveProjectile(Character attacker, Vector3 direction, float radius, float angle, float speed, Vector3 impactPoint, float effectRadius)
    {
        //return new AttackMessage { attacker = attacker, }
        return null;
    }
    /*
    public bool LineOfSightWholeCollider(Vector3 rayOrigin, Collider[] hitboxes, LayerMask hitDetection)
    {
        List<Vector3> hitboxNormals = new List<Vector3>();
        foreach(Collider c in hitboxes)
        {
            hitboxNormals += c.
        }
    }
    */


    public static RaycastHit[] ConeCastAll(Vector3 origin, float maxRadius, Vector3 direction, float maxDistance, float coneAngle)
    {
        // UPDATE THIS TO LAUNCH TWO RAYCASTS if the angle is greater than 90*
        
        RaycastHit[] sphereCastHits = Physics.SphereCastAll(origin - new Vector3(0, 0, maxRadius), maxRadius, direction, maxDistance);
        List<RaycastHit> coneCastHitList = new List<RaycastHit>();

        if (sphereCastHits.Length > 0)
        {
            for (int i = 0; i < sphereCastHits.Length; i++)
            {
                //sphereCastHits[i].collider.gameObject.GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f);
                Vector3 hitPoint = sphereCastHits[i].point;
                Vector3 directionToHit = hitPoint - origin;
                float angleToHit = Vector3.Angle(direction, directionToHit);

                if (angleToHit < coneAngle)
                {
                    coneCastHitList.Add(sphereCastHits[i]);
                }
            }
        }

        RaycastHit[] coneCastHits = new RaycastHit[coneCastHitList.Count];
        coneCastHits = coneCastHitList.ToArray();

        return coneCastHits;
    }


    public bool AtRisk(Collider[] hitboxes)
    {
        switch(type)
        {
            case AttackType.Ranged:

                RaycastHit[] fieldOfView = ConeCastAll(origin, maxRange, direction, maxRange, coneAngle);
                foreach (RaycastHit rh in fieldOfView) // Checks raycast hits against specified colliders in case one of them was hit
                {
                    foreach (Collider c in hitboxes)
                    {
                        if (rh.collider == c) // If one of the colliders was hit
                        {
                            RaycastHit lineOfSightCheck;
                            if (Physics.Raycast(origin, rh.point - origin, out lineOfSightCheck, Vector3.Distance(origin, rh.point), hitDetection)) // Launches line of sight check from origin to the hit point
                            {
                                if (lineOfSightCheck.collider == c)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }

                break;
            case AttackType.Melee:

                // Checks if the position is inside the angle and range of the melee attack

                break;
            case AttackType.AreaOfEffect:

                // Checks if the position is inside the blast radius

                break;
            case AttackType.ExplosiveRanged:

                // Checks simultaneously for the cone of fire and blast radius

                break;
            default:

                break;
        }
        return false;
    }



    public bool AtRisk(Vector3 position)
    {
        switch(type)
        {
            case AttackType.Ranged:

                // Checks if the position is inside the cone of fire
                Debug.Log(Vector3.Angle(position - origin, direction));
                if (Vector3.Angle(position - origin, direction) <= coneAngle) // Is the enemy outside the cone of fire?
                //if (Vector3.Angle(direction, position - origin) <= coneAngle) // Is the enemy outside the cone of fire?
                {
                    return true;
                    // Have a way to check for projectile width, I do not know this yet
                    
                    if (AI.LineOfSight(origin, position, hitDetection, attacker.gameObject)) // Is the enemy behind cover?
                    //if (AI.LineOfSight(position, attacker.transform, hitDetection))
                    {
                        return true;
                    }
                }

                break;
            case AttackType.Melee:

                // Checks if the position is inside the angle and range of the melee attack

                break;
            case AttackType.AreaOfEffect:

                // Checks if the position is inside the blast radius

                break;
            case AttackType.ExplosiveRanged:

                // Checks simultaneously for the cone of fire and blast radius

                break;
            default:

                break;
        }
        return false;
    }
}

public class DamageMessage
{
    public Character attacker;
    public Character victim;
    public DamageType method;
    public int amount;

    public static DamageMessage New(Character attacker, Character victim, DamageType method, int amount)
    {
        return new DamageMessage { attacker = attacker, victim = victim, method = method, amount = amount };
    }
}

public class KillMessage
{
    public Character attacker;
    public Character victim;
    public DamageType causeOfDeath;
    public static KillMessage New(Character attacker, Character victim, DamageType causeOfDeath)
    {
        return new KillMessage { attacker = attacker, victim = victim, causeOfDeath = causeOfDeath };
    }
}

public class InteractMessage
{
    public PlayerHandler player;
    public Interactable interactable;

    public static InteractMessage New(PlayerHandler player, Interactable interactable)
    {
        return new InteractMessage { player = player, interactable = interactable };
    }
}

public class SpawnMessage
{
    public Entity spawned;
    public Vector3 location;

    public static SpawnMessage New(Entity spawned, Vector3 location)
    {
        return new SpawnMessage { spawned = spawned, location = location };
    }
}