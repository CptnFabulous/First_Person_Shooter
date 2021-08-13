using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEventReceiver
{
    void OnAttack(AttackMessage message);

    void OnDamage(DamageMessage message);

    void OnKill(KillMessage message);

    void OnInteract(InteractMessage message);

    void OnSpawn(SpawnMessage message);
}
