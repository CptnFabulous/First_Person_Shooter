using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NonPlayerCharacter : MonoBehaviour
{
    NavMeshAgent na;

    // Start is called before the first frame update
    public virtual void Start()
    {
        na = GetComponent<NavMeshAgent>();
    }
    /*
    // Update is called once per frame
    void Update()
    {
        
    }
    */
}
