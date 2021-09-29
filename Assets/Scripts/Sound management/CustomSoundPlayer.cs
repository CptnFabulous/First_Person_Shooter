using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CustomSoundPlayer : ScriptableObject
{
    #region Universal sound source (reused to play sounds on objects not important enough to have their own sound source)
    static AudioSource source;
    static AudioSource UniversalSource
    {
        get
        {
            if (source == null)
            {
                // If a universal source is not present, create a new object with an AudioSource component to use as one
                source = new GameObject("Universal Sound Source").AddComponent<AudioSource>();
            }
            return source;
        }
    }
    #endregion





    public bool nonDiegetic; // If true, this sound only exists for cosmetic purposes. Otherwise, things in the world can hear it too.





}
