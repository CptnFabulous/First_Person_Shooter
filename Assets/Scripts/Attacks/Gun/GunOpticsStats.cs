using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunOpticsStats : MonoBehaviour
{
    public float magnification = 4;
    public float transitionTime = 0.5f;
    public float moveSpeedReductionPercentage = 0.5f;
    public float aimSwaySpeed = 1;

    public Transform aimPosition;

    public Transform sightLine;
    public Sprite scopeGraphic;
    public bool disableReticle;

    bool isAiming;
    float adsTimer;

    void HandleGunOptics(Gun gun, GunFiringMode mode)
    {
        // If aim toggle is enabled, change whenever the player presses the aim button
        if (gun.playerHolding.toggleAim == true && Input.GetButtonDown("MouseRight"))
        {
            isAiming = !isAiming;
        }
        else // If not, 
        {
            isAiming = Input.GetButton("MouseRight");
        }

        if (isAiming)
        {
            /*
            // Produces two varying noise values for the two axes (these will be between zero and one).
            float swayX = Mathf.PerlinNoise(Time.time * aimSwaySpeed, 0);
            float swayY = Mathf.PerlinNoise(0, Time.time * aimSwaySpeed);
            Vector2 sway = new Vector2(swayX, swayY); // Combine into a Vector2
            sway = (sway - Vector2.one * 0.5f) * 2; // Subtract 0.5 then multiply by 2 so the values are between -1 and 1
            sway = sway.normalized * gun.playerHolding.standingAccuracy.Calculate(); // Normalise and multiply by player's current accuracy
            
            Quaternion swayedAimDirection = Quaternion.Euler(sway.y, sway.x, 0);
            gun.playerHolding.handler.movement.aimDirectionTransform.localRotation = swayedAimDirection;
            */
        }
        else
        {
            //gun.playerHolding.handler.movement.aimDirectionTransform.localRotation = Quaternion.identity;
        }


        
        // Lerp ADS values if not finished lerping to the appropriate position
        if (adsTimer > 0 && adsTimer < 1)
        {
            // Determine how much to change the timer per frame, based on the transition time and when the 
            float amountToAddToTimer = Time.deltaTime / transitionTime;
            if (!isAiming)
            {
                amountToAddToTimer = -amountToAddToTimer;
            }
            adsTimer += amountToAddToTimer; // Update the timer accordingly
            adsTimer = Mathf.Clamp01(adsTimer);


            // Lerp values accordingly
            

            PlayerController pc = gun.playerHolding.handler.movement;

            // Change FOV for zoom (update later to use variablevaluefloat)
            float defaultFOV = pc.fieldOfView.defaultValue;
            pc.playerCamera.fieldOfView = Mathf.Lerp(defaultFOV, defaultFOV / magnification, adsTimer);

            // Reduce sensitivity and movement speed, alter accuracy if specified
            gun.sensitivityWhileAiming.SetIntensity(adsTimer);
            gun.speedWhileAiming.SetIntensity(adsTimer);

            // Lerp weapon model between hip position and ADS position. Should I make this work with the MoveWeaponModel function?
            Transform wmt = gun.weaponModel;

            Vector3 relativePosition = Misc.PositionWhereChildIsAtSamePositionAsAnotherTransform(wmt.position, sightLine.position, pc.head.position);
            Quaternion relativeRotation = Misc.RotationWhereChildIsAtSameRotationAsAnotherTransform(wmt.rotation, sightLine.rotation, pc.head.rotation);
            wmt.position = Vector3.Lerp(mode.general.heldPosition.position, relativePosition, adsTimer);
            wmt.rotation = Quaternion.Lerp(mode.general.heldPosition.rotation, relativeRotation, adsTimer);

            // Toggle overlay
            gun.playerHolding.handler.hud.ADSTransition(adsTimer, scopeGraphic);
        }



    }

    void LerpADS()
    {

    }

    void CancelADSInstantly()
    {

    }
}