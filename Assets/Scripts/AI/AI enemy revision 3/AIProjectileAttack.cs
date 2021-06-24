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
        Vector3 direction = wielder.currentTarget.transform.position - wielder.transform.position;
        AttackMessage m = AttackMessage.Ranged(wielder.characterData, wielder.head.position, direction, stats.range, stats.projectilePrefab.diameter, stats.projectileSpread, stats.projectilePrefab.velocity, stats.projectilePrefab.hitDetection);
        EventObserver.TransmitAttack(m); // Transmits a message of the attack the player is about to perform
    }

    public override void TheAttackItself()
    {
        base.TheAttackItself();
    }
}
