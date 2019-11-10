using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ColourTransitionEffect : TimedVisualEffect
{
    public Gradient gradient;
    [HideInInspector] public Image graphic;

    private void Awake()
    {
        graphic = GetComponent<Image>();
    }

    public override void Effect()
    {
        graphic.color = gradient.Evaluate(magnitudeOverLifetime.Evaluate(timer));
    }
}