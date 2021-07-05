using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MeleeCollisionDetector : MonoBehaviour
{
    public MeleeAttack attackToSendDataTo;

    private void OnTriggerEnter(Collision collision)
    {
        
    }
}
