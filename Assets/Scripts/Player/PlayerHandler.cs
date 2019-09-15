﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandler : MonoBehaviour
{

    public bool isActive;

    [HideInInspector] public PlayerHealth h;
    [HideInInspector] public PlayerController pc;
    [HideInInspector] public WeaponHandler wh;
    [HideInInspector] public AmmunitionInventory a;
    [HideInInspector] public HeadsUpDisplay hud;
    [HideInInspector] public PauseMenu pm;


    private void Awake()
    {
        h = GetComponent<PlayerHealth>();
        pc = GetComponent<PlayerController>();
        wh = GetComponent<WeaponHandler>();
        a = GetComponent<AmmunitionInventory>();
        hud = GetComponent<HeadsUpDisplay>();
        pm = GetComponent<PauseMenu>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
