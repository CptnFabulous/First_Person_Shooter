using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerMeleeAttack : MonoBehaviour
{
    public PlayerHandler playerHandler;

    public MeleeAttack attack;
    public float cooldown = 0.75f;
    float cooldownTimer = float.MaxValue;

    // Update is called once per frame
    void Update()
    {
        cooldownTimer += Time.deltaTime;
        if (Input.GetButtonDown("MeleeAttack") && cooldownTimer >= cooldown + attack.windup)
        {
            cooldownTimer = 0;
            attack.SingleAttack(playerHandler, transform.position, transform.forward);
            
        }
    }
}
