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

    WaitForSeconds delayYield;

    public void PlayWithoutDelay(AudioSource source)
    {
        source.pitch = Random.Range(minPitchVariance, maxPitchVariance);
        int index = Random.Range(0, sounds.Length - 1);
        //Debug.Log("Playing sound clip " + sounds[index].name);
        source.PlayOneShot(sounds[index]);
    }

    public void Play(AudioSource source)
    {
        if (delay <= 0)
        {
            PlayWithoutDelay(source);
            return;
        }
        MonoBehaviour behaviourToRunFrom = source.GetComponent<MonoBehaviour>();
        behaviourToRunFrom.StartCoroutine(DelayPlay(source));
    }

    public IEnumerator DelayPlay(AudioSource source)
    {
        if (delayYield == null)
        {
            delayYield = new WaitForSeconds(delay);
        }
        yield return delayYield;


        PlayWithoutDelay(source);
    }
}
