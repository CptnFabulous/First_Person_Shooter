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
    [HideInInspector] public bool isActive;

    [HideInInspector] public PlayerHealth ph;
    [HideInInspector] public PlayerController pc;
    [HideInInspector] public WeaponHandler wh;
    [HideInInspector] public AmmunitionInventory a;
    [HideInInspector] public HeadsUpDisplay hud;
    [HideInInspector] public GameStateHandler gsh;

    PlayerState currentState;

    private void Awake()
    {
        ph = GetComponent<PlayerHealth>();
        pc = GetComponent<PlayerController>();
        wh = GetComponent<WeaponHandler>();
        a = GetComponent<AmmunitionInventory>();
        hud = GetComponent<HeadsUpDisplay>();
        gsh = GetComponent<GameStateHandler>();
    }
}
