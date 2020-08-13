using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfViewTest : MonoBehaviour
{
    public float angle = 0.75f;
    public float range = 300;
    public float boxCastDiameter = 0.2f;
    public LayerMask hitDetection = ~0;


    float t;

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

        /*
        Transform origin = transform;



        // Produces a position the same distance from the origin as the collider, but straight on
        float distanceFromOrigin = Vector3.Distance(origin.position, c.bounds.center);
        Vector3 centreOfConeAtDistanceEquivalentToCollider = origin.position + (origin.forward.normalized * distanceFromOrigin);
        // Figures out the part of the collider that is the closest to the centre of the cone's diameter
        Vector3 closestPoint = c.bounds.ClosestPoint(centreOfConeAtDistanceEquivalentToCollider);

        if (Vector3.Angle(origin.forward, closestPoint - origin.position) < angle) // If the angle of that point is inside the cone, perform a raycast check
        {
            Vector3 upPoint = c.bounds.center + origin.up * 999999999999999;
            Vector3 downPoint = c.bounds.center + -origin.up * 999999999999999;
            Vector3 leftPoint = c.bounds.center + -origin.right * 999999999999999;
            Vector3 rightPoint = c.bounds.center + origin.right * 999999999999999;

            float scanAreaY = Vector3.Distance(c.bounds.ClosestPoint(upPoint), c.bounds.ClosestPoint(downPoint));
            float scanAreaX = Vector3.Distance(c.bounds.ClosestPoint(leftPoint), c.bounds.ClosestPoint(rightPoint));

            Debug.DrawLine(upPoint, downPoint, Color.blue);
            Debug.DrawLine(leftPoint, rightPoint, Color.yellow);
            Debug.DrawLine(c.bounds.ClosestPoint(upPoint), c.bounds.ClosestPoint(downPoint), Color.red);
            Debug.DrawLine(c.bounds.ClosestPoint(leftPoint), c.bounds.ClosestPoint(rightPoint), Color.green);
            print("Scan area dimensions: " + scanAreaX + ", " + scanAreaY);
        }
        else
        {
            print("Object not in field of view");
        }
        */


        //RaycastHit[] hits = AI.BoundsConeThing(transform, angle, range, hitDetection, 0.2f);



    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawCube(c.bounds.center, c.bounds.size);


        t += Time.deltaTime;
        if (t > 0.5f)
        {
            RaycastHit[] hits = AI.BoundsConeThing(transform, angle, range, hitDetection, boxCastDiameter);

            foreach (RaycastHit rh in hits)
            {
                Gizmos.DrawWireCube(rh.collider.bounds.center, rh.collider.bounds.size);
                //Gizmos.DrawIcon(rh.point, rh.collider.name, true);
                Gizmos.DrawWireCube(rh.point, new Vector3(boxCastDiameter, boxCastDiameter, boxCastDiameter));
            }

            t = 0;
        }


        
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
