using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TimedVisualEffect : MonoBehaviour
{
    public AnimationCurve magnitudeOverLifetime;
    public float lifetime;

    [HideInInspector] public float timer = 1;
    bool isPaused;
    
    // Update is called once per frame
    public virtual void Update()
    {
        if (timer < 1 && isPaused == false)
        {
            timer += Time.deltaTime / lifetime;
        }
    }

    public void Play()
    {
        isPaused = false;
    }

    public void Pause()
    {
        isPaused = true;
    }

    public void Stop()
    {
        timer = 0;
        isPaused = true;
    }

    public void Restart(float duration)
    {
        lifetime = duration;
        timer = 0;
        isPaused = false;
    }

    public void SetTo(float newTime)
    {
        timer = newTime;
    }
}
