using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshToMesh : MonoBehaviour
{
    public NavMeshData referenceNavMesh;
    MeshFilter mesh;

    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponent<MeshFilter>();
        mesh.mesh = FromNavMesh();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static Mesh FromNavMesh()
    {
        var navMesh = NavMesh.CalculateTriangulation();
        Vector3[] vertices = navMesh.vertices;
        int[] polygons = navMesh.indices;
        //navMesh.

        List<Vector3> verts = new List<Vector3>(vertices);

        Mesh mapMesh = new Mesh();
        mapMesh.SetVertices(verts);
        mapMesh.SetIndices(polygons, MeshTopology.Triangles, 0);
        mapMesh.RecalculateNormals();
        mapMesh.RecalculateTangents();
        mapMesh.RecalculateBounds();
        

        return mapMesh;
    }
}
