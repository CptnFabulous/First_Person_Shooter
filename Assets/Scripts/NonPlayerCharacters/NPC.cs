using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum ObjectiveType
{
    Seek,
    Evade,
    Eliminate
    // Add more objective types in future
}

[RequireComponent(typeof (Character))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AudioSource))]
public class NPC : MonoBehaviour
{
    //[Header("Standard variables")]

    //Pathfinding variables
    [HideInInspector] public Character ch;
    [HideInInspector] public NavMeshAgent na;
    [HideInInspector] public AudioSource audioSource;
    int waypointIndex;
    bool retracing;

    public virtual void Awake()
    {
        ch = GetComponent<Character>();
        na = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
    }

    #region Pathfinding behaviours
    public void Seek(GameObject target, float pursueRange)
    {
        /*
        NavMeshHit pointFound;
        if (NavMesh.SamplePosition(target.transform.position, out pointFound, pursueRange, 0))
        {
            na.destination = pointFound.position;
        }
        */

        NavMeshHit pointFound;
        if (NavMesh.SamplePosition(target.transform.position, out pointFound, pursueRange, NavMesh.AllAreas))
        {
            na.destination = pointFound.position;
        }
        else
        {
            na.destination = target.transform.position;
        }

        
        //if (na.pathStatus == NavMeshPathStatus.)



        /* // FIGURE OUT HOW TO USE Navmesh.SamplePosition
        NavMeshHit pointFound;
        if (NavMesh.SamplePosition(target.transform.position, out pointFound, pursueRange, 1))
        {
            na.destination = pointFound.position;
        }
        */

    }

    public void Flee(GameObject target, float range)
    {

    }

    #region Idle behaviours

    public void ResumePatrol(Transform[] waypoints)
    {
        int resumeIndex = 0;

        for(int i = 0; i < waypoints.Length; i++)
        {
            if (Vector3.Distance(transform.position, waypoints[i].position) <= Vector3.Distance(transform.position, waypoints[resumeIndex].position))
            {
                resumeIndex = i;
            }
        }

        waypointIndex = resumeIndex;
    }

    public void PatrolLoop(Transform[] waypoints, float threshold)
    {
        na.destination = waypoints[waypointIndex].position;

        if (Vector3.Distance(transform.position, waypoints[waypointIndex].position) >= threshold)
        {
            waypointIndex += 1;
            if (waypointIndex >= waypoints.Length)
            {
                waypointIndex = 0;
            }
        }
    }

    public void PatrolEndToEnd(Transform[] waypoints, float threshold)
    {
        na.destination = waypoints[waypointIndex].position;

        if (Vector3.Distance(transform.position, waypoints[waypointIndex].position) >= threshold)
        {
            if (retracing)
            {
                waypointIndex -= 1;
                if (waypointIndex < 0)
                {
                    retracing = false;
                }
            }
            else
            {
                waypointIndex += 1;
                if (waypointIndex >= waypoints.Length)
                {
                    retracing = true;
                }
            }
            waypointIndex = Mathf.Clamp(waypointIndex, 0, waypoints.Length - 1);
        }
    }

    public void Wander(Transform[] waypoints, float threshold)
    {
        na.destination = waypoints[waypointIndex].position;

        if (Vector3.Distance(transform.position, waypoints[waypointIndex].position) >= threshold)
        {
            waypointIndex = Random.Range(0, waypoints.Length - 1);
        }
    }

    public void StandStill()
    {
        na.destination = transform.position;
    }
    #endregion

    #endregion
}