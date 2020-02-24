using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfViewTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        List<GameObject> o = GetFieldOfView(transform, 100, 60, 30);
        string list = "FOV: ";
        foreach(GameObject obj in o)
        {
            list += obj.name + ", ";
        }
        print(list);
    }

    public List<GameObject> GetFieldOfView(Transform viewOrigin, float viewRange, float horizontalFOV, float verticalFOV)
    {
        List<GameObject> objectsInView = new List<GameObject>();
        Collider[] objects = Physics.OverlapSphere(viewOrigin.position, viewRange); // Checks for all objects in range
        foreach (Collider c in objects)
        {
            RaycastHit lineOfSight;
            if (Physics.Raycast(viewOrigin.position, c.transform.position - viewOrigin.position, out lineOfSight, viewRange)) // Launch a raycast to check if the object is actually in the NPC's line of sight.
            {
                if (lineOfSight.collider == c) // If raycast hits object being checked for line of sight.
                {
                    Vector3 relativePosition_X = new Vector3(c.transform.position.x, viewOrigin.position.y, c.transform.position.z) - viewOrigin.position;
                    Vector3 relativePosition_Y = new Vector3(viewOrigin.position.x, c.transform.position.y, c.transform.position.z) - viewOrigin.position;
                    Vector2 visionAngle = new Vector2(Vector3.Angle(relativePosition_X, viewOrigin.forward), Vector3.Angle(relativePosition_Y, viewOrigin.forward));
                    if (visionAngle.x < horizontalFOV && visionAngle.y < verticalFOV)
                    {
                        print("NPC " + gameObject.name + "has spotted " + c.gameObject.name + ".");

                        // Add c.gameObject to viewedObjects array, I need to figure out how to do this!
                        objectsInView.Add(c.gameObject);
                    }
                }
            }
        }

        return objectsInView;
    }
}
