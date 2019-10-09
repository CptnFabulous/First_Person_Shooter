using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class MuzzleFlashEffect : MonoBehaviour
{
    public AnimationCurve sizeOverLifetime;
    public Vector3 scale = Vector3.one;
    public float brightnessRange;
    public float lifetime;

    Light l;
    float timer = 1;

    private void Awake()
    {
        l = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = scale * sizeOverLifetime.Evaluate(timer);
        l.range = brightnessRange * sizeOverLifetime.Evaluate(timer);

        if (timer < 1)
        {
            timer += Time.deltaTime / lifetime;
        }
    }

    public void Play(float duration)
    {
        lifetime = duration;
        timer = 0;
    }
}
