using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NonPlayerCharacter : MonoBehaviour
{
    NavMeshAgent na;

    private void Awake()
    {
        na = GetComponent<NavMeshAgent>();
    }
}
