using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightingAI : AI
{







    float lookAngleSpeed;


    IEnumerator currentAttack;
    void AimAtTarget(EntityAttack a, Entity target)
    {
        LookTowards(target.transform.position, lookAngleSpeed);

    }


}
