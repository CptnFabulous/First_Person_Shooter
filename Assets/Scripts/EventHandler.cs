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
    public LayerMask thingsInDanger = 9;

    Character[] charactersAtRisk;

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
    public Vector2 attackAngles;

    public static AttackMessage Ranged(Character attacker, Vector3 origin, Vector3 direction, float maxRange, float projectileDiameter, float coneAngle, float velocity, LayerMask hitDetection)
    {
        AttackMessage m = new AttackMessage();
        m.attacker = attacker;
        m.type = AttackType.Ranged;
        m.origin = origin;
        m.direction = direction;
        m.maxRange = maxRange;
        m.projectileDiameter = projectileDiameter;
        m.coneAngle = coneAngle;
        m.velocity = velocity;
        m.hitDetection = hitDetection;

        return m;
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

    public Character[] CharactersAtRisk()
    {
        List<Character> list = new List<Character>();

        switch (type)
        {
            case AttackType.Ranged:

                // Perform a line of sight check
                RaycastHit[] thingsInLineOfFire = AIFunction.VisionCone(origin, direction, Vector3.up, coneAngle, maxRange, thingsInDanger, hitDetection);
                foreach(RaycastHit rh in thingsInLineOfFire)
                {
                    Character c = Character.FromHit(rh.collider.gameObject); // Checks if there is an object in 

                    if (c.faction.Affiliation(attacker.faction) == FactionState.Hostile)
                    {
                        if (list.Contains(c) == false)
                        {
                            list.Add(c);
                        }
                    }
                }


                break;

            case AttackType.Melee:





                break;

            case AttackType.AreaOfEffect:







                break;

            case AttackType.ExplosiveRanged:








                break;

            default:

                break;
        }

        return new Character[0];
    }

    public bool AtRisk(Vector3 positionChecked, Collider[] characterColliders)
    {
        switch(type)
        {
            case AttackType.Ranged:

                // Check if the character is inside the cone of fire




                break;

            case AttackType.Melee:


                // Save this until I have an actual melee attack system


                break;

            case AttackType.AreaOfEffect:

                // Check if the character is inside the blast radius and behind cover


                break;

            case AttackType.ExplosiveRanged:


                // ?? Somehow combine a vision cone check with a blast radius check

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