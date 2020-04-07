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
        /*
        foreach (MeleeAttack a in meleeAttacks)
        {
            a.c = this;
        }
        */
    }

    public void Dodge(Character attacker, Character victim)
    {
        if (victim == c) // Checks incoming attack message to see if it is the one being attacked
        {
            this.attacker = attacker; // Specifies attacker to dodge from
            stateMachine.SetBool("mustDodgeAttack", true); // Sets trigger so agent can dodge attack
        }
    }
}
