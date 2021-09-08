using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public static class AIRelevantPositionHandler
{



    static NavMeshData currentMesh;



    static float spacingBetweenCasts = 1;

    public static void GenerateNewPositions(NavMeshData currentMesh, LayerMask terrainDetection)
    {
        
        
        Bounds meshBounds = currentMesh.sourceBounds;

        for (float x = meshBounds.min.x; x < meshBounds.max.x; x += spacingBetweenCasts)
        {
            for (float z = meshBounds.min.z; z < meshBounds.max.z; z += spacingBetweenCasts)
            {
                // Creates positions to launch a raycast down onto the navmesh
                Vector3 rayOrigin = currentMesh.rotation * new Vector3(x, meshBounds.max.y, z);
                rayOrigin += meshBounds.center;
                Vector3 rayDirection = currentMesh.rotation * Vector3.down;

                // Find positions on the NavMesh, and for each one 
                //Physics.Ov

                RaycastHit[] hits = Physics.RaycastAll(rayOrigin, rayDirection, meshBounds.extents.y * 2, terrainDetection);



            }
        }
    }
}
