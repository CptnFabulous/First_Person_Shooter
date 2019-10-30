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

    // Update is called once per frame
    public override void Update()
    {
        //Color c = graphic.color;
        //c *= gradient.Evaluate(magnitudeOverLifetime.Evaluate(timer));
        //graphic.color = c;
        graphic.color = gradient.Evaluate(magnitudeOverLifetime.Evaluate(timer));
        base.Update();
    }
}