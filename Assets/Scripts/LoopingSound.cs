using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopingSound : ScriptableObject
{
    public AudioClip start;
    public AudioClip loop;
    public AudioClip end;
    float soundDelay;


    public IEnumerator Play(AudioSource source)
    {
        source.loop = false;
        source.clip = start;
        source.Play();

        yield return new WaitWhile(() => source.time < start.length);

        source.loop = true;
        source.clip = loop;
        source.Play();
    }

    void End(AudioSource source)
    {
        source.loop = false;
        source.clip = end;
        source.Play();
    }

    void Cancel(AudioSource source)
    {
        source.Stop();
        source.clip = null;
    }

    void Restart(AudioSource source)
    {
        Cancel(source);
        Play(source);
    }
}

public class RepeatingSound : ScriptableObject
{
    public AudioClip[] clips;
    public float delay;

    // Add specific audio data as well, so I can adjust on the fly

    public void PlaySound(AudioSource source)
    {
        // Randomly selects a clip
        AudioClip randomClip = clips[Random.Range(0, clips.Length - 1)];
        source.PlayOneShot(randomClip);
    }
}
