using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]

public class EnemyAI : MonoBehaviour
{
    public int healthMax;
    public int healthCurrent;


    public GameObject head;

    [Header("Field of Vision")]
    public float viewRange;
    [Range(0, 180)]
    public float horizontalFOV;
    [Range(0, 180)]
    public float verticalFOV;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    /*
    void FieldOfVision()
    {
        Collider[] viewedObjects = Physics.OverlapSphere(head.transform.position, viewRange);

        foreach (Collider c in viewedObjects)
        {
            // Vector3 targetDirection = g.transform.position - head.transform.position;

            Vector3 targetDirection = Vector3.Angle(g.gameObject.transform.position - head.transform.position, transform.forward);

            print(targetDirection);
            
            
        }
    }
    */

    public void Damage(int damageAmount)
    {
        healthCurrent -= damageAmount;
    }
}
