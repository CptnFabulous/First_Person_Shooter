
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandler : Character
{
    //public new PlayerHealth HealthData { get; private set; }
    
    [HideInInspector] public PlayerController movement;
    [HideInInspector] public WeaponHandler weapons;
    [HideInInspector] public AmmunitionInventory ammo;
    [HideInInspector] public GameStateHandler stateHandler;
    [HideInInspector] public AudioSource audio;
    [Header("Player-specific scripts")]
    public HeadsUpDisplay hud;



    public override void Awake()
    {
        movement = GetComponent<PlayerController>();
        weapons = GetComponent<WeaponHandler>();
        ammo = GetComponent<AmmunitionInventory>();
        stateHandler = GetComponent<GameStateHandler>();

        audio = GetComponent<AudioSource>();


        hud.player = this;

        base.Awake();
    }

    public GameState PlayerState()
    {
        return stateHandler.CurrentState();
    }

    public override void Destroy()
    {
        Die();
    }

    public void Die()
    {
        health.values.current = 0;
        movement.rb.constraints = RigidbodyConstraints.None;
        movement.enabled = false;
        weapons.CurrentWeapon().enabled = false;
        weapons.enabled = false;
        stateHandler.FailGame();
    }

    public void Respawn(int newHealth, Vector3 position)
    {
        health.values.current = newHealth;
        transform.rotation = Quaternion.Euler(0, transform.rotation.y, 0);
        transform.position = position;

        movement.rb.constraints = RigidbodyConstraints.FreezeRotation;
        movement.enabled = true;
        weapons.CurrentWeapon().enabled = true;
        weapons.enabled = true;
        stateHandler.ResumeGame();
    }
    
}