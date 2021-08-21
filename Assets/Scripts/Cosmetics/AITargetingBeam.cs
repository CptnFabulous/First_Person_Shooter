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
        Debug.Log(ai);
        beam.enabled = false;
    }

    public void SetBeam()
    {
        //Debug.Log("Setting beam length");
        
        float beamLength = distance;

        RaycastHit rh;
        if (Physics.Raycast(ai.LookOrigin, ai.LookDirection, out rh, distance, hitDetection))
        {
            beamLength = Vector3.Distance(ai.LookOrigin, rh.point);
        }

        aimPosition = ai.LookOrigin + ai.LookDirection * beamLength;
        enabled = true;
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
