using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICombatant : AIEntity
{
    [Header("Attacking")]
    public AIAttack[] attacks;
    public IEnumerator currentAttack;



}
