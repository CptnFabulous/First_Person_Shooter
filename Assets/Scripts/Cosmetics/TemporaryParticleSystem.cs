using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class TemporaryParticleSystem : MonoBehaviour
{
    ParticleSystem ps;

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ps.isStopped)
        {
            Destroy(gameObject);
        }
    }
}
