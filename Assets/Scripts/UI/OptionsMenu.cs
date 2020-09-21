using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class OptionsMenu : Menu
{


    // Video
    // Simple
    public Dropdown monitor;
    public Toggle fullScreenMode;
    public Dropdown resolution;
    public Dropdown qualityPreset;

    FullScreenMode mode;


    // Advanced
    // CHECK VARIOUS GRAPHICAL OPTIONS

    // Audio
    public AudioMixer audioMixer;



    public Slider masterVolume;
    public Dropdown language;
    public Dropdown subtitles;
    public Toggle muteAudioOnFocusLoss;

    // Controls
    public Dropdown keyboardPreset;
    public Dropdown controllerPreset;

    // Gameplay
    public Slider sensitivityX;
    public Slider sensitivityY;
    public Toggle invertYAxis;
    public Toggle controllerVibration;

    // Accessibility
    public Toggle tutorials;
    public Toggle aimAssist;
    public Dropdown colourBlindMode;
    public Slider reticleColourRed;
    public Slider reticleColourGreen;
    public Slider reticleColourBlue;


    private void Start()
    {
        

        

    }



    #region Change video settings
    void PopulateVideoMenu()
    {
        // Set monitor
        List<string> monitors = new List<string>();
        foreach (Display d in Display.displays)
        {
            monitors.Add(d.ToString());
            // Update display to show correct variable
        }
        monitor.AddOptions(monitors);
        monitor.onValueChanged.AddListener(SetMonitor);



        // Set fullscreen
        fullScreenMode.enabled = Screen.fullScreen;
        fullScreenMode.onValueChanged.AddListener(SetFullscreen);



        // Set resolution
        List<string> resolutions = new List<string>();
        foreach(Resolution r in Screen.resolutions)
        {
            string text = r.width + " x " + r.height + " @ " + r.refreshRate;
            resolutions.Add(text);
            // Update display to show correct variable
        }
        resolution.AddOptions(resolutions);
        resolution.onValueChanged.AddListener(SetResolution);



        // Set graphics quality level
        List<string> presets = new List<string>(QualitySettings.names);
        qualityPreset.AddOptions(presets);
        // Update display to show correct variable
        qualityPreset.onValueChanged.AddListener(SetGraphicsQuality);
    }

    void SetMonitor(int m)
    {
        //Screen.
        //Display.
    }

    void SetFullscreen(bool b)
    {
        Screen.fullScreen = b;
    }
    /*
    void SetFullscreen(int i)
    {
        Screen.fullScreenMode = (FullScreenMode)i;
    }
    */

    void SetResolution(int i)
    {
        Resolution r = Screen.resolutions[i];
        //Screen.SetResolution(r.width, r.height, );
    }

    void SetGraphicsQuality(int q)
    {
        QualitySettings.SetQualityLevel(q);
    }
    #endregion



    #region Change audio settings

    void SetVolume(float v)
    {
        audioMixer.SetFloat("Volume", v);
    }

    #endregion
}
