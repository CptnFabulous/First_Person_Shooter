using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class MeleeAttack
{
    public int damage = 15;
    public float knockback = 20;
    public float range = 3;
    public float angle = 90;
    public LayerMask detection = ~0;
    public float windup = 0.25f;
    public bool attackFriendlies;
    public DamageType type = DamageType.Slashing;

    [Header("Cosmetics")]
    public UnityEvent windupEffects;
    public UnityEvent attackEffects;
    public UnityEvent hitEffects;

    
    public void SingleAttack(Entity attacker, Vector3 position, Vector3 direction)
    {
        attackEffects.Invoke();

        bool hitSuccessful = false;
        Character attackingCharacter = attacker as Character;
        List<Health> alreadyDamaged = new List<Health>();
        Collider[] inRange = Physics.OverlapSphere(position, range, detection);
        for (int i = 0; i < inRange.Length; i++)
        {
            Vector3 closestPoint = Explosion.GetColliderPointFromCentre(inRange[i], position);

            bool inAngle = Vector3.Angle(direction, closestPoint - position) <= angle / 2;
            if (!inAngle) { continue; }

            Ray checkRay = new Ray(position, closestPoint - position);
            bool lineOfSight = Physics.Raycast(checkRay, out RaycastHit hit, range, detection) && hit.collider == inRange[i];
            if (!lineOfSight) { continue; }

            bool canAttack = attackingCharacter == null || attackingCharacter.CanDamage(hit.collider.GetComponent<Character>(), attackFriendlies, false);
            if (!canAttack) { continue; }


            hitSuccessful = true;

            DamageHitbox dh = hit.collider.GetComponent<DamageHitbox>();
            if (dh != null && !alreadyDamaged.Contains(dh.healthScript))
            {
                dh.Damage(damage, attacker, type);
                alreadyDamaged.Add(dh.healthScript);
            }
            
            Rigidbody rb = hit.collider.GetComponentInParent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForceAtPosition(checkRay.direction * knockback, hit.point, ForceMode.Impulse);
            }
        }

        if (hitSuccessful == true)
        {
            hitEffects.Invoke();
        }
    }

    IEnumerator AttackSequence(Entity attacker, Vector3 position, Vector3 direction)
    {
        windupEffects.Invoke();
        yield return new WaitForSeconds(windup);

        SingleAttack(attacker, position, direction);
    }
}
