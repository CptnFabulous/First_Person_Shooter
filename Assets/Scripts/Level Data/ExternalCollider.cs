using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class ExternalCollider : MonoBehaviour
{
    [System.Serializable]
    public class CollisionEvent : UnityEvent<Collision>
    {
        public CollisionEvent()
        {

        }
    }

    [System.Serializable]
    public class TriggerEvent : UnityEvent<Collider>
    {
        public TriggerEvent()
        {

        }
    }

    public CollisionEvent collisionEnter;
    public CollisionEvent collisionStay;
    public CollisionEvent collisionExit;
    public TriggerEvent triggerEnter;
    public TriggerEvent triggerStay;
    public TriggerEvent triggerExit;

    private void OnCollisionEnter(Collision collision)
    {
        collisionEnter.Invoke(collision);
    }
    private void OnCollisionStay(Collision collision)
    {
        collisionStay.Invoke(collision);
    }
    private void OnCollisionExit(Collision collision)
    {
        collisionExit.Invoke(collision);
    }
    private void OnTriggerEnter(Collider other)
    {
        triggerEnter.Invoke(other);
    }
    private void OnTriggerStay(Collider other)
    {
        triggerStay.Invoke(other);
    }
    private void OnTriggerExit(Collider other)
    {
        triggerExit.Invoke(other);
    }

}
