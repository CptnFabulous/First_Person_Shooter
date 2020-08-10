using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfViewTest : MonoBehaviour
{
    public float angle = 0.75f;
    public float range = 300;
    public float sphereCastDiameter = 0.1f;
    public LayerMask hitDetection = ~0;




    public Collider c;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //RaycastHit[] hits = AI.RaycastVisionCone(transform.position, transform.forward, angle, range, sphereCastDiameter, hitDetection);
        //RaycastHit[] hits = AI.RaycastVisionCone(transform, angle, range, sphereCastDiameter, hitDetection);

        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;
        float testAngle = 0.01f;

        

        float distanceFromOrigin = Vector3.Distance(origin, c.bounds.center);
        Vector3 centreOfConeAtDistanceEquivalentToCollider = origin + (direction.normalized * distanceFromOrigin);
        Vector3 closestPoint = c.bounds.ClosestPoint(centreOfConeAtDistanceEquivalentToCollider);
        if (Vector3.Angle(direction, closestPoint - origin) < testAngle)
        {
            print("Object is inside the field of vision");
        }
        else
        {
            print("Object cannot be detected");
        }


    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(c.bounds.center, c.bounds.size);
    }

    /*
    private void OnDrawGizmos() // Lecks' stuff
    {
        Vector3 origin = transform.position;
        float range = 50;
        var direction = transform.forward;
        int amountOfRaycastRings = 20;
        float angleIncrement = 360f / amountOfRaycastRings;
        for (int i = 0; i < amountOfRaycastRings; i++)
        {
            Vector3 raycastDirection = Quaternion.AngleAxis(angleIncrement * i, transform.forward) * Quaternion.AngleAxis(5, transform.right) * direction;
            Color c = Color.white;
            float cv = (c.r / amountOfRaycastRings) * i;
            c = new Color(cv, cv, cv);
            Gizmos.color = c;
            Gizmos.DrawLine(origin, origin + raycastDirection * range);
        }

        //Vector3 rd = Quaternion.AngleAxis(0, 90, 180 or 270, tr.forward) * Quaternion.AngleAxis(spread, tr.right) * tr.forward;

        
        Transform tr = transform;
        float spread = 10;
        Vector3 rd = Quaternion.AngleAxis(0, tr.forward) * Quaternion.AngleAxis(spread, tr.right) * tr.forward;

        Vector3 reticleOffsetPoint = rd * range;
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(tr.position, tr.position + reticleOffsetPoint);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(tr.position, tr.position + tr.forward * range);
              
    }
    */
}
