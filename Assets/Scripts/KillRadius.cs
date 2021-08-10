using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillRadius : MonoBehaviour
{
    public float radius = 999;
    public float delayBetweenChecks = 5;
    float checkTimer;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        checkTimer += Time.deltaTime;
        if (checkTimer > delayBetweenChecks)
        {
            //Debug.Log("Kill sweep on frame " + Time.frameCount);
            checkTimer = 0;
            Entity[] entities = FindObjectsOfType<Entity>();
            foreach(Entity e in entities)
            {
                if (Vector3.Distance(transform.position, e.transform.position) > radius)
                {
                    e.Destroy();
                }
            }
        }
    }
}
