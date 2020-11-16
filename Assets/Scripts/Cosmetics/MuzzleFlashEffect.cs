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

    public override void Effect()
    {
        transform.localScale = scale * magnitudeOverLifetime.Evaluate(timer);
        l.range = brightnessRange * magnitudeOverLifetime.Evaluate(timer);
    }
}