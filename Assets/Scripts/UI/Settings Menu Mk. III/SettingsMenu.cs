using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class SettingsMenu : MonoBehaviour
{
    public Button applyChanges;
    public Button revertToPrevious;
    public Button revertToDefault;
    
    
    // Start is called before the first frame update
    void Awake()
    {
        applyChanges.onClick.AddListener(SaveSettings);
        revertToPrevious.onClick.AddListener(RevertToPreviousSettings);
        revertToDefault.onClick.AddListener(RevertToDefaultSettings);

        MenuSpecificSetup();
        RefreshSettings();
    }
    /*
    // Update is called once per frame
    void Update()
    {
        
    }
    */


    public virtual void MenuSpecificSetup()
    {
        // Add listeners to enable the apply and revert buttons when a function is changed from before.
        // Load in particular variables
    }

    public virtual void RefreshSettings()
    {
        // Sets up options. In inherited classes with actual settings to alter, you would change the options to match the actual variables.
        applyChanges.enabled = false;
        revertToPrevious.enabled = false;
    }

    public void OnOptionAlter()
    {
        applyChanges.enabled = true;
        revertToPrevious.enabled = true;
    }

    public virtual void SaveSettings()
    {
        // In an inherited class, apply the settings in the options interactables to whatever settings they are supposed to represent
        RefreshSettings();
    }

    public void RevertToPreviousSettings()
    {
        Debug.Log("Reverting interactable options to current settings");
        RefreshSettings();
    }

    public virtual void RevertToDefaultSettings()
    {
        RefreshSettings();
    }
}
