using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combatant : AI
{
    [Header("Projectile attacks")]
    public ProjectileAttack[] projectileAttacks;
    public MeleeAttack[] meleeAttacks;
}
