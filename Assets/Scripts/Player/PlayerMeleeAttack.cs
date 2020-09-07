using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMeleeAttack : MonoBehaviour
{
    public PlayerHandler playerHandler;

    public int damage = 10;
    public float knockback = 5;
    public float cooldown = 1;
    float cooldownTimer = float.MaxValue;

    public float range = 2;
    public LayerMask hitDetection;

    public AudioClip hitSound;
    public AudioClip missSound;
    public DamageType damageType;

    // Update is called once per frame
    void Update()
    {
        cooldownTimer += Time.deltaTime;
        if (Input.GetButtonDown("MeleeAttack") && cooldownTimer >= cooldown)
        {
            cooldownTimer = 0;

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
        }
    }
}
