using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    CharacterJoint[] joints;
    Collider[] colliders;
    Rigidbody[] rigidbodies;
    //Matrix4x4[] defaultTransformValues;
    public bool disableCollidersWhenDisabled;

    private void Awake()
    {
        joints = GetComponentsInChildren<CharacterJoint>();
        colliders = GetComponentsInChildren<Collider>();
        rigidbodies = GetComponentsInChildren<Rigidbody>();
        
        for (int i = 0; i < joints.Length; i++)
        {
            joints[i].enableCollision = false;
            joints[i].autoConfigureConnectedAnchor = true;
            joints[i].connectedBody = joints[i].transform.parent.GetComponentInParent<Rigidbody>();
            //Debug.Log("Setting rigidbody of " + joints[i].name + " to " + joints[i].connectedBody);
        }
        /*
        defaultTransformValues = new Matrix4x4[rigidbodies.Length];
        for (int i = 0; i < defaultTransformValues.Length; i++)
        {
            defaultTransformValues[i] = rigidbodies[i].transform.worldToLocalMatrix;
            rigidbodies[i].set
        }
        */
        SetActive(enabled);
    }

    private void OnEnable()
    {
        SetActive(true);
    }
    private void OnDisable()
    {
        SetActive(false);
    }

    void SetActive(bool active)
    {
        //Debug.Log("Setting ragdoll state to " + active);
        for (int i = 0; i < colliders.Length; i++)
        {
            // If the ragdoll is activated, or if it's deactivated but disableCollidersWhenDisabled is false, activate the collider.
            // If the ragdoll is deactivated and disableCollidersWhenDisabled is true, disable it
            colliders[i].enabled = !(!active && disableCollidersWhenDisabled);
        }
        for (int i = 0; i < rigidbodies.Length; i++)
        {
            rigidbodies[i].isKinematic = !active;
        }
        /*
        for (int i = 0; i < joints.Length; i++)
        {
            //joints[i].en
        }
        */
    }
}
