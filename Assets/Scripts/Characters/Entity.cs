using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public string properName;
    public string description;
    public bool isUniquelyNamed;
    
    /*
    [SerializeField] float timeScale = 1;
    public float TimeScale
    {
        get
        {
            return timeScale;
        }
        set
        {
            timeScale = Mathf.Clamp(value, 0, float.MaxValue);
        }
    }
    public float CurrentTime
    {
        get
        {
            return Time.time * timeScale;
        }
    }
    public float DeltaTime
    {
        get
        {
            return Time.deltaTime * timeScale;
        }
    }
    */

    public virtual void Destroy()
    {
        Destroy(gameObject);
    }
}
