using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerMeleeAttack : MonoBehaviour
{
    public PlayerHandler playerHandler;

    public MeleeAttack attack;
    public float cooldown = 1;
    float cooldownTimer = float.MaxValue;

    // Update is called once per frame
    void Update()
    {
        cooldownTimer += Time.deltaTime;
        if (Input.GetButtonDown("MeleeAttack") && cooldownTimer >= cooldown + attack.windupDuration + attack.attackDuration)
        {
            Debug.Log("Attack button pressed");
            cooldownTimer = 0;
            attack.ExecuteAttack(playerHandler);
            /*
            RaycastHit meleeHit;
            if (Physics.Raycast(transform.position, transform.forward, out meleeHit, range, hitDetection))
            {
                playerHandler.playerAudio.PlayOneShot(hitSound);
                Damage.PointDamage(playerHandler, meleeHit.collider.gameObject, damage, damageType, false);
                Damage.Knockback(meleeHit.collider.gameObject, knockback, meleeHit.collider.transform.position - transform.position);
            }
            else
            {
                playerHandler.playerAudio.PlayOneShot(missSound);
            }
            */
        }
    }
}
