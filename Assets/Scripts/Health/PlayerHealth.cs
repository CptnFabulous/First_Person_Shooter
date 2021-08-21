using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (PlayerHandler))]
public class PlayerHealth : Health
{
    
    [HideInInspector] public PlayerHandler ph;

    public float wackyTestVariable;
    

    public override void Awake()
    {
        base.Awake();
        ph = GetComponent<PlayerHandler>();
    }

    public override void Damage(int damageAmount, Entity origin, DamageType damageSource)
    {
        base.Damage(damageAmount, origin, damageSource);
        ph.hud.PlayerDamageFeedback();
    }

    public override void Die(DamageType causeOfDeath, Entity lastAttacker)
    {
        base.Die(causeOfDeath, lastAttacker);
        //ph.ChangePlayerState(PlayerState.Dead);
        ph.Die();

        
    }


    
}
