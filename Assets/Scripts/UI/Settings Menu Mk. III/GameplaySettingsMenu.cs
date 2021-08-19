using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplaySettingsMenu : SettingsMenu
{
    PlayerHandler playerToUpdate;

    [Header("Interactables for settings")]
    public Slider fieldOfView;
    public float fieldOfViewDefault = 60;
    public Slider cameraSensitivityX;
    public float cameraSensitivityXDefault = 2.5f;
    public Slider cameraSensitivityY;
    public float cameraSensitivityYDefault = 2.5f;
    public Toggle toggleCrouch;
    public bool toggleCrouchDefault = true;
    public Toggle toggleADS;
    public bool toggleADSDefault = false;
    //public Toggle enableLeftHandedness;
    //public bool enableLeftHandednessDefault = false;
    //public ColourAlteringThing reticleColour;
    //public Color reticleColourDefault = Color.white;

    public override void MenuSpecificSetup()
    {
        base.MenuSpecificSetup();

        /*
        I don't fully understand the code here, but I can somehow add an underscore and a lambda expression to allow me to put parameterless functions in actions that do have them assigned.
        If I don't do this, I can't put a parameterless function in an event that has them.
        
        ACTUAL EXPLANATION:
        From Mrp1 on Discord, who provided the solution in the first place

        _ => bleh creates a new, anonymous function that takes in anything you give it
        And immediately throws away whatever you give it
        It pretty much makes a func
        void _GENERATED_FUNNCTION(TYPE_GIVEN _) { YourFunction();} 
        */
        fieldOfView.onValueChanged.AddListener(_ => OnOptionAlter());
        cameraSensitivityX.onValueChanged.AddListener(_ => OnOptionAlter());
        cameraSensitivityY.onValueChanged.AddListener(_ => OnOptionAlter());
        toggleCrouch.onValueChanged.AddListener(_ => OnOptionAlter());
        toggleADS.onValueChanged.AddListener(_ => OnOptionAlter());
        //enableLeftHandedness.onValueChanged.AddListener(_ => OnOptionAlter());
    }
    public override void RefreshSettings()
    {
        base.RefreshSettings();

        Debug.Log("Refreshing options in player settings menu");
        playerToUpdate = GetComponentInParent<PlayerHandler>();
        // Updates interactable options to represent the current settings
        fieldOfView.value = playerToUpdate.movement.fieldOfView.defaultValue;
        cameraSensitivityX.value = playerToUpdate.movement.sensitivityX;
        cameraSensitivityY.value = playerToUpdate.movement.sensitivityY;
        toggleCrouch.isOn = playerToUpdate.movement.toggleCrouch;
        toggleADS.isOn = playerToUpdate.wh.toggleAim;
    }
    public override void SaveSettings()
    {
        // Applies settings here. This has two benefits:
        // One, having a saving function means the player is not locked into any experimental decisions they make.
        // Two, I don't have to make a new function and add it as a listener for each option.
        
        playerToUpdate.movement.fieldOfView.defaultValue = fieldOfView.value;
        playerToUpdate.movement.sensitivityX = cameraSensitivityX.value;
        playerToUpdate.movement.sensitivityY = cameraSensitivityY.value;
        playerToUpdate.movement.toggleCrouch = toggleCrouch.isOn;
        playerToUpdate.wh.toggleAim = toggleADS.isOn;
        //Debug.Log("Left handed set to " + enableLeftHandedness.isOn + ", but this function doesn't do anything yet.");
        //playerToUpdate.hud.reticleDefaultColour = 
        base.SaveSettings();
    }
    public override void RevertToDefaultSettings()
    {
        // Switches all settings in this category to the preset functions assigned upon opening the game for the first time
        playerToUpdate.movement.fieldOfView.defaultValue = fieldOfViewDefault;
        playerToUpdate.movement.sensitivityX = cameraSensitivityXDefault;
        playerToUpdate.movement.sensitivityY = cameraSensitivityYDefault;
        playerToUpdate.movement.toggleCrouch = toggleCrouchDefault;
        playerToUpdate.wh.toggleAim = toggleADSDefault;
        //Debug.Log("Left handed set to " + enableLeftHandednessDefault + ", but this function doesn't do anything yet.");
        //playerToUpdate.hud.reticleDefaultColour = reticleColourDefault;
        base.RevertToDefaultSettings();
    }

    

}
