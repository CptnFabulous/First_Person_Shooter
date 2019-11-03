using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TimedVisualEffect : MonoBehaviour
{
    public AnimationCurve magnitudeOverLifetime;
    public float lifetime;
    bool directControl;

    [HideInInspector] public float timer = 1;
    bool isPaused;
    
    // Update is called once per frame
    public virtual void Update()
    {
        if (directControl == false && timer < 1 && isPaused == false)
        {
            timer += Time.deltaTime / lifetime;
        }
    }

    public void Play()
    {
        timer = 0;
        isPaused = false;
    }

    public void IsPaused(bool pause)
    {
        isPaused = pause;
    }

    public void Stop()
    {
        timer = 0;
        isPaused = true;
    }

    public void Reset(float duration)
    {
        lifetime = duration;
        //timer = 0;
    }

    public void SetTo(float newTime)
    {
        timer = newTime;

        //directControl = true;
    }
}
