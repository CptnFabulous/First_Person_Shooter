using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Random Sound Player", menuName = "ScriptableObjects/Random Sound Player", order = 0)]
public class RandomSoundPlayer : ScriptableObject
{
    public AudioClip[] sounds;
    [Range(-3, 3)]
    public float minPitchVariance = 1;
    [Range(-3, 3)]
    public float maxPitchVariance = 1;
    public float delay;

    public void Play(AudioSource source)
    {
        source.pitch = Random.Range(minPitchVariance, maxPitchVariance);
        source.clip = sounds[Random.Range(0, sounds.Length - 1)];
        source.PlayDelayed(delay);
    }
}
