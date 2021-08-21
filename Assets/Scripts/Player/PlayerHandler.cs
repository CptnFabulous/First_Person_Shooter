
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandler : Character
{
    //public new PlayerHealth HealthData { get; private set; }
    
    
    [HideInInspector] public PlayerHealth ph;
    [HideInInspector] public PlayerController movement;
    [HideInInspector] public WeaponHandler wh;
    [HideInInspector] public AmmunitionInventory a;
    [HideInInspector] public HeadsUpDisplay hud;
    [HideInInspector] public GameStateHandler gsh;
    [HideInInspector] public AudioSource playerAudio;

    

    public override void Awake()
    {
        
        /*
        HealthData = GetComponent<PlayerHealth>();

        HealthData.wackyTestVariable = 5;

        PlayerHandler pHandler = GetComponent<PlayerHandler>();

        Character c = pHandler;
        Debug.Log(c.HealthData.ToString());

        pHandler.HealthData.wackyTestVariable = 42069f;

        Character ch = GetComponent<Character>();
        ch.HealthData.wack
        */




        ph = GetComponent<PlayerHealth>();
        movement = GetComponent<PlayerController>();
        wh = GetComponent<WeaponHandler>();
        a = GetComponent<AmmunitionInventory>();
        hud = GetComponent<HeadsUpDisplay>();
        gsh = GetComponent<GameStateHandler>();

        playerAudio = GetComponent<AudioSource>();

        base.Awake();
    }

    public GameState PlayerState()
    {
        return gsh.CurrentState();
    }

    public override void Destroy()
    {
        Die();
    }

    public void Die()
    {
        ph.values.current = 0;
        movement.rb.constraints = RigidbodyConstraints.None;
        movement.enabled = false;
        wh.CurrentWeapon().enabled = false;
        wh.enabled = false;
        gsh.FailGame();
    }

    public void Respawn(int health, Vector3 position)
    {
        ph.values.current = health;
        transform.rotation = Quaternion.Euler(0, transform.rotation.y, 0);
        transform.position = position;

        movement.rb.constraints = RigidbodyConstraints.FreezeRotation;
        movement.enabled = true;
        wh.CurrentWeapon().enabled = true;
        wh.enabled = true;
        gsh.ResumeGame();
    }
    
}