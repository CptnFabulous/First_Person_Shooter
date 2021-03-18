using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EntityAttack : MonoBehaviour
{
    public AI user;
    public Entity target;





    [Header("Attack stats")]

    public float attacksPerMinute;



    public Event cosmeticEffects;

    [Header("Aiming and telegraphing")]

    public float aimSpeedWhileTelegraphing;
    public float aimSpeedWhileAttacking;
    
    public float telegraphDelay;
    public float cooldownTime;



    public UnityEvent telegraphCosmeticEffects;




    

    


    /*
    IEnumerator AttackContinuously()
    {

    }
    */


    IEnumerator BurstAttack(int burstAmount, float delay, float cooldown)
    {
        // Create telegraph;
        telegraphCosmeticEffects.Invoke();
        yield return new WaitForSeconds(delay);

        // Creates an int to count the amount of bursts, and a float to count the time between attacks
        int burstCounter = burstAmount;
        float attackTimer = 0;

        // We've made a miniature update loop inside the IEnumerator. This will keep the loop running until burstCounter runs out.
        while (burstCounter > 0)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer > attacksPerMinute / 60)
            {
                // ATTACK!!!!!
                attackTimer = 0;
                burstCounter -= 1;
            }



            yield return new WaitForEndOfFrame();
        }
        
        
        
        




    }

    void EndAttack()
    {

    }




    // Checks if the 
    bool WithinAngleThreshold(float angle)
    {
        Vector3 aimDirection = user.head.forward;
        Vector3 playerDirection = target.transform.position - user.head.position;

        if (Vector3.Angle(aimDirection, playerDirection) <= angle)
        {
            return true;
        }

        return false;
    }

    // Produces a position to initiate an area of effect attack, that is to the side of the target.
    Vector3 AimPositionForAreaOfEffect()
    {
        // Figures out how far away the target is.
        float distanceToTarget = Vector3.Distance(user.head.position, target.transform.position);
        // Creates a new position that's an equivalent distance away, but in the direction the user is looking
        Vector3 aimPoint = user.head.forward * distanceToTarget;

        return aimPoint;
    }

    bool WithinDistanceThreshold(Vector3 aimPoint, float distance)
    {
        if (Vector3.Distance(aimPoint, target.transform.position) <= distance)
        {
            return true;
        }
        return false;
    }

}
