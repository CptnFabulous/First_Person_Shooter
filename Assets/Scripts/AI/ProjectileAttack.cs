using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class ProjectileAttack : NPCAttack
{
    [Header("Projectile attack stats")]
    public ProjectileStats projectileStats;
    public Transform projectileOrigin;
    public float spread;

    public override void TelegraphAttack()
    {
        base.TelegraphAttack();
        AttackMessage m = AttackMessage.Ranged(c.characterData, c.head.position, c.target.transform.position - c.transform.position, range, projectileStats.diameter, spread, projectileStats.velocity, projectileStats.hitDetection);
        EventObserver.TransmitAttack(m); // Transmits a message of the attack the player is about to perform
    }

    public override void ExecuteAttack()
    {
        base.ExecuteAttack();
        Damage.ShootProjectile(projectileStats, spread, range, c.characterData, c.head, projectileOrigin.position, aimMarker - c.head.position); // Shoots an amount of projectiles based on the attack's damage stats
    }
}