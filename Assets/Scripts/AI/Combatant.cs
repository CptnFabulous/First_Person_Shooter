using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combatant : AI
{
    [Header("Projectile attacks")]
    public ProjectileAttack[] projectileAttacks;
    public MeleeAttack[] meleeAttacks;

    public override void Awake()
    {
        base.Awake();

        foreach (ProjectileAttack a in projectileAttacks)
        {
            a.c = this;
        }
        
        foreach (MeleeAttack a in meleeAttacks)
        {
            a.c = this;
        }
        
    }
}