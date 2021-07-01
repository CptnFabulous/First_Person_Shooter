using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateMeshTest : MonoBehaviour
{
    public Material meshMaterial;
    
    
    // Start is called before the first frame update
    void Start()
    {
        GenerateSquare();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void GenerateSquare()
    {
        /*
        Vector3[] vertices = new Vector3[]
        {
            new Vector3(0, 1, 0),
            new Vector3(1, 1, 0),
            new Vector3(0, 0, 0),
            new Vector3(1, 0, 0)
        };
        
        Vector2[] uv = new Vector2[]
        {
            new Vector2(0, 1),
            new Vector2(1, 1),
            new Vector2(0, 0),
            new Vector2(1, 0)
        };
        
        int[] triangles = new int[]
        {
            0,
            1,
            2,
            2,
            1,
            3
        };
        */

        /*
        Vector3[] vertices = new Vector3[]
        {
            new Vector3(0, 1, 0),
            new Vector3(1, 1, 0),
            new Vector3(0, 0, 0),
            new Vector3(1, 0, 0),
            new Vector3(0, 1, 1),
            new Vector3(1, 1, 1),
            new Vector3(0, 0, 1),
            new Vector3(1, 0, 1)
        };
        Vector2[] uv = new Vector2[]
        {
            new Vector2(0, 1),
            new Vector2(1, 1),
            new Vector2(0, 0),
            new Vector2(1, 0),

            new Vector2(0, 1),
            new Vector2(1, 1),
            new Vector2(0, 0),
            new Vector2(1, 0),

            new Vector2(0, 1),
            new Vector2(1, 1),
            new Vector2(0, 0),
            new Vector2(1, 0),

            new Vector2(0, 1),
            new Vector2(1, 1),
            new Vector2(0, 0),
            new Vector2(1, 0),

            new Vector2(0, 1),
            new Vector2(1, 1),
            new Vector2(0, 0),
            new Vector2(1, 0),

            new Vector2(0, 1),
            new Vector2(1, 1),
            new Vector2(0, 0),
            new Vector2(1, 0)
        };
        int[] triangles = new int[]
        {
            // Front
            0,
            1,
            2,
            2,
            1,
            3,

            // Back
            6,
            5,
            8,
            8,
            5,
            7,

            // Top
            0,
            5,
            6,
            6,
            1,
            0,

            // Bottom
            2,
            3,
            7,
            7,
            3,
            8,

            // Left
            5,
            0,
            7,
            7,
            0,
            2,

            // Right
            1,
            6,
            3,
            3,
            6,
            8
        };
        */

        float minRange = 2;
        float maxRange = 5;


        float attackWidthInDegrees = 10;
        float attackLengthInDegrees = 75;
        float angle;
        float numberOfSegmentsForDetection = 10;


        float attackWidthFromCentre = attackWidthInDegrees / 2;
        float segmentAngleWidth = attackLengthInDegrees / numberOfSegmentsForDetection;

        Vector3 topBack = Vector3.forward * minRange;
        Vector3 topFront = Vector3.forward * maxRange;
        Vector3 bottomBack = Misc.AngledDirection(new Vector3(segmentAngleWidth, 0, 0), Vector3.forward, Vector3.up).normalized * minRange;
        Vector3 bottomFront = Misc.AngledDirection(new Vector3(segmentAngleWidth, 0, 0), Vector3.forward, Vector3.up).normalized * maxRange;

        /*
        Vector3 topBackLeft = topBack + Vector3.left * attackWidthFromCentre;
        Vector3 topBackRight = topBack + Vector3.right * attackWidthFromCentre;
        Vector3 topFrontLeft = topFront + Vector3.left * attackWidthFromCentre;
        Vector3 topFrontRight = topFront + Vector3.right * attackWidthFromCentre;
        Vector3 bottomBackLeft = bottomBack + Vector3.left * attackWidthFromCentre;
        Vector3 bottomBackRight = bottomBack + Vector3.right * attackWidthFromCentre;
        Vector3 bottomFrontLeft = bottomFront + Vector3.left * attackWidthFromCentre;
        Vector3 bottomFrontRight = bottomFront + Vector3.right * attackWidthFromCentre;
        */

        Vector3 topBackLeft = new Vector3(-0.5f, 0.5f, -0.5f);
        Vector3 topBackRight = new Vector3(0.5f, 0.5f, -0.5f);
        Vector3 topFrontLeft = new Vector3(-0.5f, 0.5f, 0.5f);
        Vector3 topFrontRight = new Vector3(0.5f, 0.5f, 0.5f);
        Vector3 bottomBackLeft = new Vector3(-0.5f, -0.5f, -0.5f);
        Vector3 bottomBackRight = new Vector3(0.5f, -0.5f, -0.5f);
        Vector3 bottomFrontLeft = new Vector3(-0.5f, -0.5f, 0.5f);
        Vector3 bottomFrontRight = new Vector3(0.5f, -0.5f, 0.5f);

        Vector3[] vertices = new Vector3[]
        {
            // Back
            topBackLeft,
            topBackRight,
            bottomBackLeft,
            bottomBackRight,
            // Front
            topFrontRight,
            topFrontLeft,
            bottomFrontRight,
            bottomFrontLeft,
            // Top
            topFrontLeft,
            topFrontRight,
            topBackLeft,
            topBackRight,
            // Bottom
            bottomFrontLeft,
            bottomFrontRight,
            bottomBackLeft,
            bottomBackRight,
            // Left
            topFrontLeft,
            topBackLeft,
            bottomFrontLeft,
            bottomBackLeft,
            // Right
            topBackRight,
            topFrontRight,
            bottomBackRight,
            bottomFrontRight,
        };
        int[] triangles = new int[]
        {
            
            // Back
            0,2,1,2,3,1,
            // Front
            4,6,5,6,7,5,
            // Top
            8,10,9,10,11,9,
            // Bottom
            12,13,14,14,13,15,
            // Left
            16,18,17,18,19,17,
            // Right
            20,22,21,22,23,21
            
            /*
            // Back
            0,1,2,2,1,3,
            // Front
            4,5,6,6,5,7,
            // Top
            8,9,10,10,9,11,
            // Bottom
            12,13,14,14,13,15,
            // Left
            16,17,18,18,17,19,
            // Right
            20,21,22,22,21,23
            */
        };

        
        /*
        // Figure out cube mesh data
        Mesh cm = GameObject.CreatePrimitive(PrimitiveType.Cube).GetComponent<MeshFilter>().mesh;
        Mesh cubeMesh = new Mesh();
        cubeMesh.vertices = cm.vertices;
        cubeMesh.triangles = cm.triangles;
        string verts = "Vertices = ";
        for (int i = 0; i < cubeMesh.vertices.Length; i++)
        {
            verts += cubeMesh.vertices[i] + ", ";
        }
        Debug.Log(verts);
        string tris = "Triangles = ";
        for (int i = 0; i < cubeMesh.triangles.Length; i++)
        {
            tris += cubeMesh.triangles[i] + ", ";
        }
        Debug.Log(tris);
        */

        
        Mesh hitDetectionMesh = new Mesh();

        hitDetectionMesh.vertices = vertices;
        //hitDetectionMesh.uv = uv;
        hitDetectionMesh.triangles = triangles;

        GameObject meshObject = new GameObject("Hit Detection Mesh", typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider));

        meshObject.GetComponent<MeshFilter>().mesh = hitDetectionMesh;
        meshObject.GetComponent<MeshCollider>().sharedMesh = hitDetectionMesh;
        meshObject.GetComponent<MeshRenderer>().material = meshMaterial;

        

        meshObject.transform.position = transform.position;
        
    }
}
