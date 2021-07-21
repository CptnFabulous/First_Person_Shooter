using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class BasicOptionsMenuRevisionTwo : MonoBehaviour
{
    public Dropdown screenResolutionDropdown;

    public AudioMixer audio;

    Resolution[] resolutions;

    // Use this for initialization
    void Start()
    {
        resolutions = Screen.resolutions;
        screenResolutionDropdown.ClearOptions();
        List<string> resOptions = new List<string>();

        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            resOptions.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        screenResolutionDropdown.AddOptions(resOptions);
        screenResolutionDropdown.value = currentResolutionIndex;
        screenResolutionDropdown.RefreshShownValue();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetVolume(float volume)
    {
        print(volume);
        audio.SetFloat("MasterVolume", volume);
    }

    public void SetGraphicsQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        print("Graphics quality changed to " + qualityIndex);
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution r = resolutions[resolutionIndex];
        Screen.SetResolution(r.width, r.height, Screen.fullScreen);
    }

    public void SetFullscreen(bool isTrue)
    {
        Screen.fullScreen = isTrue;
    }
}
