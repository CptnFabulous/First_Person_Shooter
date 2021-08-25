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
        mesh.mesh = Misc.MeshFromNavMesh();
    }
}
