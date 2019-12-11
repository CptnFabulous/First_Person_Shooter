
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    Active,
    Dead,
    InMenus,
    InCutscene
}

public class PlayerHandler : Character
{
    [HideInInspector] public PlayerHealth ph;
    [HideInInspector] public PlayerController pc;
    [HideInInspector] public WeaponHandler wh;
    [HideInInspector] public AmmunitionInventory a;
    [HideInInspector] public HeadsUpDisplay hud;
    [HideInInspector] public GameStateHandler gsh;

    public AudioSource playerAudio;

    PlayerState currentState = PlayerState.Active;

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

    
    public void ChangePlayerState(PlayerState ps)
    {
        currentState = ps;
        switch (currentState)
        {
            case PlayerState.Active: // Resume game
                ph.health.current = ph.health.max;

                transform.rotation = Quaternion.Euler(0, transform.rotation.y, 0);

                pc.rb.constraints = RigidbodyConstraints.FreezeRotation;

                pc.enabled = true;

                wh.CurrentWeapon().enabled = true;

                wh.enabled = true;

                break;
            case PlayerState.Dead: // Game is lost, display fail menu

                pc.rb.constraints = RigidbodyConstraints.None;

                pc.enabled = false;

                wh.CurrentWeapon().enabled = false;

                wh.enabled = false;

                //gsh.ChangeGameState(GameState.Failed);
                gsh.FailGame();

                break;
            case PlayerState.InMenus:
                pc.enabled = false;

                wh.CurrentWeapon().enabled = false;

                wh.enabled = false;

                gsh.PauseGame();
                //gsh.ChangeGameState(GameState.Paused);

                break;
            default:

                break;
        }
    }

    public PlayerState CurrentState()
    {
        return currentState;
    }
    
}