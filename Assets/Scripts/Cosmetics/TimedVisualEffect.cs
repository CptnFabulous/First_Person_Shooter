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
        isPaused = false;

        directControl = false;
    }

    public void Pause()
    {
        isPaused = true;

        directControl = false;
    }

    public void Stop()
    {
        timer = 0;
        isPaused = true;

        directControl = false;
    }

    public void Restart(float duration)
    {
        lifetime = duration;
        timer = 0;
        isPaused = false;

        directControl = false;
    }

    public void SetTo(float newTime)
    {
        timer = newTime;

        directControl = true;
    }
}
