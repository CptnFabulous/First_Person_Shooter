using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class DisappearingEffect : TimedVisualEffect
{
    public float opacity;
    
    Image i;

    private void Awake()
    {
        i = GetComponent<Image>();
    }

    // Update is called once per frame
    public override void Update()
    {
        Color c = i.color;
        c.a = opacity * magnitudeOverLifetime.Evaluate(timer);
        i.color = c;
        base.Update();
    }
}