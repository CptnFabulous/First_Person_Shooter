using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfViewTest : MonoBehaviour
{
    public float angle;
    public float range;
    public float boxCastDiameter;
    public LayerMask stuffBeingSearchedFor = ~0;
    public LayerMask hitDetection = ~0;



    float t;

    public Collider c;


    RaycastHit[] hits;

    // Start is called before the first frame update
    void Start()
    {
        
    }




    



    // Update is called once per frame
    void Update()
    {
        

        t += Time.deltaTime;
        if (t > 0.5f)
        {
            hits = AIFunction.VisionCone(transform.position, transform.forward, transform.up, angle, range, stuffBeingSearchedFor, hitDetection, boxCastDiameter);
            t = 0;
        }


    }

    private void OnGUI()
    {
        if (hits != null && hits.Length > 0)
        {
            Vector2 screenUnit = new Vector2(Screen.width / 16, Screen.height / 9);
            Rect boxPos = new Rect(0, 0, screenUnit.x * 3, screenUnit.y * 9);

            string text = "Objects viewed:";
            foreach (RaycastHit rh in hits)
            {
                text += "\n" + rh.collider.name + ", " + rh.point;
            }

            GUI.Box(boxPos, text);
        }
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawCube(c.bounds.center, c.bounds.size);

        
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
