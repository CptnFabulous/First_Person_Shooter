using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightingAI : AI
{






    
    public VariableValueFloat aimDegreesPerSecond; // How fast the enemy will turn around to look at things.
    [HideInInspector] float lookAngleSpeed; // The current 

    public EntityAttack[] attacks;
    public IEnumerator attackInProgress;








    void AimAtTarget(EntityAttack a, Entity target)
    {
        // Shift aim towards target
        LookTowards(target.transform.position, aimDegreesPerSecond.Calculate());

        // If target is close enough to aim to hit
        // If user is not in the middle of another attack
        // If the attack being aimed is not on cooldown
        if (a.CanHit(a.aimConfirmThreshold) && attackInProgress == null && a.OffCooldown())
        {
            //attackInProgress = StartCoroutine()

            if (a.attacksInBurst > 0)
            {
                attackInProgress = a.BurstAttack(a.attacksInBurst, a.telegraphDelay, a.cooldownTime);
            }
            else
            {
                attackInProgress = a.AttackContinuously(a.telegraphDelay, a.fireBreakThreshold, a.cooldownTime);
            }
            StartCoroutine(attackInProgress);
        }
    }





    void EndAttack()
    {
        StopCoroutine(attackInProgress);
        attackInProgress = null;
    }

    


}
