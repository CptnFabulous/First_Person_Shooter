﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIProjectileAttack : AIAttack
{
    [Header("Gun stats")]
    public GunGeneralStats stats;


    public override void Telegraph()
    {
        base.Telegraph();
        
        //Vector3 direction = wielder.currentTarget.transform.position - wielder.LookOrigin;
        Vector3 direction = DetermineEnemyPosition() - wielder.LookOrigin;
        AttackMessage m = AttackMessage.Ranged(wielder.characterData, wielder.LookOrigin, direction, stats.range, stats.projectilePrefab.diameter, stats.projectileSpread, stats.projectilePrefab.velocity, stats.projectilePrefab.hitDetection);
        EventJunction.Transmit(m);
        //EventObserver.TransmitAttack(m); // Transmits a message of the attack the player is about to perform
    }

    public override void SingleAttack()
    {
        base.SingleAttack();
        stats.Shoot(wielder.characterData, wielder.LookOrigin, wielder.LookDirection, wielder.LookUp);
    }
}
