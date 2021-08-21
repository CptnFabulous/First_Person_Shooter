using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FlyingAgent : MonoBehaviour
{

    public NavMeshAgent na;
    Vector3 destination;
    bool destinationEstablished;



    public float maxVerticalDistance;
    public LayerMask hitDetection;



    private void Awake()
    {
        na = GetComponent<NavMeshAgent>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // If line of sight is established between the agent's actual position and the position

        float heightRelativeToAgent = -na.baseOffset - na.height;

        RaycastHit upCheck;
        if (Physics.SphereCast(transform.position, na.radius, Vector3.up, out upCheck, maxVerticalDistance, hitDetection))
        {
            //upCheck.
            
            Vector3 maxHeight = upCheck.point + (-Vector3.up * na.height / 2);
            if (destination.y <= maxHeight.y)
            {
                transform.Translate(Vector3.up * na.speed * Time.deltaTime);
            }
            
        }
        RaycastHit downCheck;
        if (Physics.SphereCast(transform.position, na.radius, -Vector3.up, out downCheck, maxVerticalDistance, hitDetection))
        {

        }
    }
}
