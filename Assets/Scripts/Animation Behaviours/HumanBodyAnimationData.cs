using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanBodyAnimationData : MonoBehaviour
{
    public AIEntity ai;

    public Transform pelvis;
    public Vector3 pelvisEulerAnglesToAccountFor;
    public Transform upperTorso;
    public Vector3 torsoEulerAnglesToAccountFor;

    


    private void LateUpdate()
    {
        SetTorsoRotation();
    }



    public void SetTorsoRotation()
    {
        // Rotates the character model to face in the same horizontal direction the AI is looking in, accounting for the weird rotations of the model
        Vector3 horizontalDirectionCharacterIsFacing = Vector3.ProjectOnPlane(ai.head.transform.forward, ai.transform.up);
        horizontalDirectionCharacterIsFacing.Normalize();
        Quaternion torsoRotation = Quaternion.LookRotation(horizontalDirectionCharacterIsFacing, ai.transform.up);
        pelvis.transform.rotation = torsoRotation * Quaternion.Euler(pelvisEulerAnglesToAccountFor);

        // Rotates upper torso so the model is facing and aiming in the direction the AI is looking
        Quaternion upperTorsoRotation = Quaternion.LookRotation(ai.head.transform.forward, ai.transform.up);
        upperTorso.transform.rotation = upperTorsoRotation * Quaternion.Euler(torsoEulerAnglesToAccountFor);
    }
}