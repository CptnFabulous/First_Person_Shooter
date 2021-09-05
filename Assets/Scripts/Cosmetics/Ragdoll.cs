using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    CharacterJoint[] joints;
    Collider[] colliders;
    Rigidbody[] rigidbodies;
    public bool startActive;
    public bool disableCollidersWhenDisabled;



    private void Awake()
    {
        joints = GetComponentsInChildren<CharacterJoint>();
        colliders = GetComponentsInChildren<Collider>();
        rigidbodies = GetComponentsInChildren<Rigidbody>();
        
        for (int i = 0; i < joints.Length; i++)
        {
            joints[i].enableCollision = false;
            joints[i].connectedBody = joints[i].transform.parent.GetComponentInParent<Rigidbody>();
            Debug.Log("Setting rigidbody of " + joints[i].name + " to " + joints[i].connectedBody);
        }
        SetActive(enabled);
    }

    void SetActive(bool active)
    {
        /*
        for (int i = 0; i < joints.Length; i++)
        {
            //joints[i].en
        }
        */
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
    }

    private void OnEnable()
    {
        SetActive(true);
    }

    private void OnDisable()
    {
        SetActive(false);
    }
}
