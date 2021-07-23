using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class ProjectileAttack : NPCAttack
{
    [Header("Projectile attack stats")]
    public Projectile prefab;
    public int count = 1;
    public Transform projectileOrigin;
    public float spread;

    public override void TelegraphAttack()
    {
        base.TelegraphAttack();
        AttackMessage m = AttackMessage.Ranged(c.characterData, c.head.position, c.target.transform.position - c.transform.position, range, prefab.diameter, spread, prefab.velocity, prefab.hitDetection);
        EventObserver.TransmitAttack(m); // Transmits a message of the attack the player is about to perform
    }

    public override void ExecuteAttack()
    {
        base.ExecuteAttack();
        Damage.ShootProjectile(prefab, count, spread, range, c.characterData, c.head.position, aimMarker - c.head.position, c.head.up, projectileOrigin.position); // Shoots an amount of projectiles based on the attack's damage stats
    }
}