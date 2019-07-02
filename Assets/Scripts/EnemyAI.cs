using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]

public class EnemyAI : MonoBehaviour
{
    
    public GameObject head;

    [Header("Targeting")]
    public GameObject[] targetedCharacters;


    [Header("Attacking")]
    public int damage;
    public int range;

    [Header("Field of Vision")]
    [Min(0)] public float viewRange;
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
        FieldOfVision();
    }

    void TargetEnemy()
    {

    }

    
    void FieldOfVision()
    {
        Collider[] viewedObjects = Physics.OverlapSphere(head.transform.position, viewRange);
        //print(viewedObjects);

        foreach (Collider c in viewedObjects)
        {
            //Vector3 targetDirection = c.transform.position - head.transform.position;


            Vector3 relativePosition_X = new Vector3(c.transform.position.x, head.transform.position.y, c.transform.position.z);
            Vector3 relativePosition_Y = new Vector3(head.transform.position.x, c.transform.position.y, c.transform.position.z);

            //Vector2 visionAngle = new Vector2(Vector3.Angle(targetDirection, head.transform.forward), Vector3.Angle(targetDirection, head.transform.forward));

            Vector2 visionAngle = new Vector2(Vector3.Angle(relativePosition_X, head.transform.forward), Vector3.Angle(relativePosition_Y, head.transform.forward));
            print(visionAngle);
            if (visionAngle.x < horizontalFOV && visionAngle.y < verticalFOV)
            {
                print(c.gameObject.name + " is in the NPC's field of vision.");
            }

            //print(visionAngle);

            //Vector3 targetDirection = Vector3.Angle(g.gameObject.transform.position - head.transform.position, transform.forward);

            //print(targetDirection);
            
            
        }
    }
    

}
