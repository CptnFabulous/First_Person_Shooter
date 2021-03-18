using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillPerimeter : MonoBehaviour
{
    public float killRadius = 99999;
    public float delayBetweenSweeps = 1;
    float delayTimer;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        delayTimer -= Time.deltaTime;
        if (delayTimer <= 0) // If cooldown has finished after last sweep
        {
            // Resets timer
            delayTimer = delayBetweenSweeps;

            // Checks all entities in the scene

            Entity[] entitiesInScene = FindObjectsOfType<Entity>();
            foreach(Entity e in entitiesInScene)
            {
                if (Vector3.Distance(transform.position, e.transform.position) > killRadius)
                {
                    /*
                    if () // If the entity has a health meter, kill them
                    {

                    }
                    else if () // If the entity is a poolable object, return it
                    {

                    }
                    else
                    {
                        Destroy(e.gameObject);
                    }
                    */
                }
            }
        }
    }
}
