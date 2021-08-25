using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelData : MonoBehaviour
{
    public static LevelData internalReferenceForCurrent;
    public static LevelData Current
    {
        get
        {
            if (internalReferenceForCurrent == null)
            {
                internalReferenceForCurrent = FindObjectOfType<LevelData>();
            }

            return internalReferenceForCurrent;
        }
    }


    public string ReferenceName
    {
        get
        {
            return gameObject.scene.name;
        }
    }

    public string properName;
    public SpawnPoint placeToSpawn;
    public PlayerObjective[] objectivesToComplete;

    


    


    private void Start()
    {
        
    }
}
