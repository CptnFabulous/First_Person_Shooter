using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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


    


    // Interface based (does not work properly)
    /*
    // Implement the interface IEventReceiver on all scripts that need to reference it
    // When transmitting a message, find every MonoBehaviour in the scene that uses IEventReceiver and is enabled
    // Invoke the appropriate function on every receiver
    // This requires a processor-efficient way to update the receiver list. Interfaces can't have anything like Awake() or OnDestroy() so they can't automatically add and remove themselves
    static List<IEventReceiver> internalReceiversReference;
    public static List<IEventReceiver> ReceiverList
    {
        get
        {
            if (internalReceiversReference == null) // If internal reference does not exist
            {
                //IEventReceiver[] r = FindObjectsOfType<IEventReceiver>();
                //internalReceiversReference = new List<IEventReceiver>(r);
            }

            


            // Update list to remove everything that's null
            internalReceiversReference.RemoveAll(r => r == null);

            // Obtain the internal reference
            return internalReceiversReference;
        }
    }

    public static IEventReceiver[] GetReceiversInScene()
    {
        // Create a list of receivers to add to, and an array of all objects in the scene
        List<IEventReceiver> newList = new List<IEventReceiver>();
        GameObject[] objects = FindObjectsOfType<GameObject>();
        for (int i = 0; i < objects.Length; i++) // Check every object in the scene
        {
            // Get IEventReceivers in each object in the scene and add it to the list
            IEventReceiver[] receiversInObject = objects[i].GetComponents<IEventReceiver>();
            newList.AddRange(receiversInObject);
        }

        return newList.ToArray();
    }

    public static void TransmitAttack(AttackMessage message)
    {
        foreach(IEventReceiver receiver in ReceiverList)
        {
            receiver.OnAttack(message);
        }
    }
    public static void TransmitDamage(DamageMessage message)
    {
        foreach (IEventReceiver receiver in ReceiverList)
        {
            receiver.OnDamage(message);
        }
    }
    public static void TransmitKill(KillMessage message)
    {
        foreach (IEventReceiver receiver in ReceiverList)
        {
            receiver.OnKill(message);
        }
    }
    public static void TransmitInteraction(InteractMessage message)
    {
        foreach (IEventReceiver receiver in ReceiverList)
        {
            receiver.OnInteract(message);
        }
    }
    public static void TransmitSpawning(SpawnMessage message)
    {
        foreach (IEventReceiver receiver in ReceiverList)
        {
            receiver.OnSpawn(message);
        }
    }
    */

    // Delegate based
    /*
    public event System.Action<AttackMessage> OnAttack;
    public event System.Action<DamageMessage> OnDamage;
    public event System.Action<KillMessage> OnKill;
    public event System.Action<InteractMessage> OnInteract;
    public event System.Action<SpawnMessage> OnSpawn;

    static EventHandler internalReference;
    public static EventHandler Current
    {
        get
        {
            if (internalReference == null) // If internal reference does not exist
            {
                // Assign an EventHandler that already exists
                internalReference = FindObjectOfType<EventHandler>();

                if (internalReference == null) // If it's still null because one was not found
                {
                    // Make a new one and record it
                    GameObject manager = new GameObject("EventHandler");
                    internalReference = manager.AddComponent<EventHandler>();
                }
            }

            // Obtain the internal reference
            return internalReference;
        }
    }

    public static void TransmitAttack(AttackMessage message)
    {
        Current.OnAttack(message);
    }
    public static void TransmitDamage(DamageMessage message)
    {
        Current.OnDamage(message);
    }
    public static void TransmitKill(KillMessage message)
    {
        Current.OnKill(message);
    }
    public static void TransmitInteraction(InteractMessage message)
    {
        Current.OnInteract(message);
    }
    public static void TransmitSpawning(SpawnMessage message)
    {
        Current.OnSpawn(message);
    }
    */
}

public class AttackMessage
{
    #region Variables
    public enum AttackType
    {
        Ranged,
        Melee,
        AreaOfEffect,
        ExplosiveRanged,
    }

    public Character attacker; // The entity performing the attack
    public AttackType type;
    public LayerMask hitDetection = ~0;
    public LayerMask damageableThings = (1 << 9);

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
    #endregion

    #region Create new message
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

        m.charactersAtRisk = m.GetCharactersAtRisk(); // Performs a calculation to find all enemies within the attack's boundaries. DO THIS LAST, after all the proper variables have been established for accurate calculations

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
    #endregion

    // Obtains a list of all characters in the path of the attack. This is performed once and then referred to later, to reduce unnecessary calculations
    Character[] GetCharactersAtRisk()
    {
        List<Character> list = new List<Character>();

        switch (type)
        {
            case AttackType.Ranged:
                
                // Perform a vision cone check
                RaycastHit[] thingsInLineOfFire = AIFunction.VisionCone(origin, direction, Vector3.up, coneAngle, maxRange, damageableThings, hitDetection);
                foreach(RaycastHit rh in thingsInLineOfFire)
                {
                    // Check raycasthit collider to see if it is a character with a faction
                    Character c = Character.FromObject(rh.collider.gameObject);

                    // If there is a character class
                    // If the character class is not already in the list
                    // If the character class is considered an enemy of the attacker
                    if (c != null && list.Contains(c) == false && attacker.HostileTowards(c))
                    {
                        // If so, the character is added to the list of at risk characters
                        list.Add(c);
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

        return list.ToArray();
    }

    // Checks if a character is in the line of fire
    public bool AtRisk(Character c)
    {
        foreach(Character ch in charactersAtRisk)
        {
            if (ch == c)
            {
                return true;
            }
        }

        return false;
    }

    // Checks if a position is in the line of fire
    public bool AtRisk(Vector3 positionChecked, Collider[] characterColliders)
    {
        switch(type)
        {
            case AttackType.Ranged:

                // Check if the character is inside the cone of fire

                // If inside range
                if (Vector3.Distance(origin, positionChecked) < maxRange)
                {
                    // If inside angle
                    if (Vector3.Angle(direction, positionChecked - origin) < coneAngle)
                    {
                        // If inside line of sight
                        if (AIFunction.LineOfSightCheckWithExceptions(positionChecked, origin, hitDetection, characterColliders))
                        {
                            return true;
                        }
                    }
                }

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

    public DamageMessage(Character _attacker, Character _victim, DamageType _method, int _amount)
    {
        attacker = _attacker;
        victim = _victim;
        method = _method;
        amount = _amount;
    }
}

public class KillMessage
{
    public Character attacker;
    public Character victim;
    public DamageType causeOfDeath;

    public KillMessage(Character _attacker, Character _victim, DamageType _causeOfDeath)
    {
        attacker = _attacker;
        victim = _victim;
        causeOfDeath = _causeOfDeath;
    }
}

public class InteractMessage
{
    public PlayerHandler player;
    public Interactable interactable;

    public InteractMessage(PlayerHandler _player, Interactable _interactable)
    {
        player = _player;
        interactable = _interactable;
    }
}

public class SpawnMessage
{
    public Entity spawned;
    public Vector3 location;

    public SpawnMessage(Entity _spawned, Vector3 _location)
    {
        spawned = _spawned;
        location = _location;
    }
}