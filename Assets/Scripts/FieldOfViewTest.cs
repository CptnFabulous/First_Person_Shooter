using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfViewTest : MonoBehaviour
{
    public float angle = 0.75f;
    public float range = 300;
    public float sphereCastDiameter = 0.1f;
    public LayerMask hitDetection = ~0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //RaycastHit[] hits = AI.RaycastVisionCone(transform.position, transform.forward, angle, range, sphereCastDiameter, hitDetection);
        RaycastHit[] hits = AI.RaycastVisionCone(transform, angle, range, sphereCastDiameter, hitDetection);

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
