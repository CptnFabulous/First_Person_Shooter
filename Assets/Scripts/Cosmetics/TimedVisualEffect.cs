﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TimedVisualEffect : MonoBehaviour
{
    public AnimationCurve magnitudeOverLifetime;
    public float lifetime = 1;
    public bool looping;
    public bool directControl;
    public bool playOnEnable;
    
    [Range(0, 1)]
    public float timer = 0;
    bool isPaused = true;
    /*
    private void OnValidate()
    {
        Effect();
    }
    */
    public virtual void Awake()
    {

    }

    // Update is called once per frame
    public void Update()
    {
        if (directControl == false && timer < 1 && isPaused == false)
        {
            timer += Time.deltaTime / lifetime;

            if (timer >= 1)
            {
                if (looping)
                {
                    timer = 0;
                }
                else
                {
                    isPaused = true;
                }
            }
        }

        Effect();
    }

    public virtual void Effect()
    {
        print(timer);
    }

    public void Play()
    {
        isPaused = false;
        timer = 0;
    }

    public void SetPause(bool pause)
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
    }

    public void SetTo(float newTime)
    {
        timer = newTime;
    }

    private void OnEnable()
    {
        if (playOnEnable == true)
        {
            Play();
        }
    }
}
