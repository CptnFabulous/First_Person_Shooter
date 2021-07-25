using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class AITargetingBeam : MonoBehaviour
{
    LineRenderer beam;
    AICombatant ai;

    public float distance;
    public LayerMask hitDetection = ~0;

    Vector3 aimPosition;

    private void Awake()
    {
        beam = GetComponent<LineRenderer>();
        ai = GetComponentInParent<AICombatant>();
    }

    public void SetBeam()
    {
        Debug.Log("Setting beam length");
        
        float beamLength = distance;

        RaycastHit rh;
        if (Physics.Raycast(ai.head.transform.position, ai.head.transform.forward, out rh, distance, hitDetection))
        {
            beamLength = Vector3.Distance(ai.head.transform.position, rh.point);
        }

        aimPosition = ai.head.transform.position + ai.head.transform.forward * beamLength;
    }

    private void Update()
    {
        Vector3[] beamPositions = new Vector3[]
        {
            //transform.InverseTransformPoint(transform.position),
            //transform.InverseTransformPoint(finalPosition)
            transform.position,
            aimPosition
        };

        beam.SetPositions(beamPositions);
    }
}
