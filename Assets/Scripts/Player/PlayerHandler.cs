
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandler : Character
{
    [HideInInspector] public PlayerHealth ph;
    [HideInInspector] public PlayerController pc;
    [HideInInspector] public WeaponHandler wh;
    [HideInInspector] public AmmunitionInventory a;
    [HideInInspector] public HeadsUpDisplay hud;
    [HideInInspector] public GameStateHandler gsh;
    [HideInInspector] public AudioSource playerAudio;

    

    private void Awake()
    {
        ph = GetComponent<PlayerHealth>();
        pc = GetComponent<PlayerController>();
        wh = GetComponent<WeaponHandler>();
        a = GetComponent<AmmunitionInventory>();
        hud = GetComponent<HeadsUpDisplay>();
        gsh = GetComponent<GameStateHandler>();

        playerAudio = GetComponent<AudioSource>();
    }

    public GameState PlayerState()
    {
        return gsh.CurrentState();
    }

    public void Die()
    {
        pc.rb.constraints = RigidbodyConstraints.None;
        pc.enabled = false;
        wh.CurrentWeapon().enabled = false;
        wh.enabled = false;
        gsh.FailGame();
    }

    public void Respawn(int health, Vector3 position)
    {
        ph.health.current = health;
        transform.rotation = Quaternion.Euler(0, transform.rotation.y, 0);
        transform.position = position;

        pc.rb.constraints = RigidbodyConstraints.FreezeRotation; 
        pc.enabled = true;
        wh.CurrentWeapon().enabled = true;
        wh.enabled = true;
        gsh.ResumeGame();
    }
    
}