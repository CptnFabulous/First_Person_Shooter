using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (PlayerHandler))]
public class PlayerHealth : Health
{
    [HideInInspector] public PlayerHandler ph;

    public void Awake()
    {
        ph = GetComponent<PlayerHandler>();
    }

    public override void Die(DamageType causeOfDeath, GameObject lastAttacker)
    {
        //ph.ChangePlayerState(PlayerState.Dead);

        

        
    }
    
}
