using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class MuzzleFlashEffect : TimedVisualEffect
{
    public Vector3 scale = Vector3.one;
    public float brightnessRange;

    Light l;

    private void Awake()
    {
        l = GetComponent<Light>();
    }

    // Update is called once per frame
    public override void Update()
    {
        transform.localScale = scale * magnitudeOverLifetime.Evaluate(timer);
        l.range = brightnessRange * magnitudeOverLifetime.Evaluate(timer);
        base.Update();
    }
}