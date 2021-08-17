using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplaySettingsMenu : MonoBehaviour
{
    public PlayerHandler playerToUpdate;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetFieldOfView(float fieldOfView)
    {
        playerToUpdate.movement.fieldOfView.defaultValue = fieldOfView;
    }
    public void SetCameraSensitivity(Vector2 newSensitivity)
    {
        playerToUpdate.movement.sensitivityX = newSensitivity.x;
        playerToUpdate.movement.sensitivityY = newSensitivity.y;
    }
    public void SetCrouchToggle(bool isToggleable)
    {
        playerToUpdate.movement.toggleCrouch = isToggleable;
    }
    public void SetReticleColour(Color newColour)
    {
        playerToUpdate.hud.reticleDefaultColour = newColour;
    }
    public void SetADSToggle(bool isToggleable)
    {
        playerToUpdate.wh.toggleAim = isToggleable;
    }




    public void SetWeaponHandedness(bool isLeftHanded)
    {
        Debug.Log("Left handed set to " + isLeftHanded + ", but this function doesn't do anything yet.");
    }

}
