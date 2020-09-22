using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class OptionsMenu : Menu
{

    [Header("Video")]
    // Simple
    public Dropdown monitor;
    public Toggle fullScreenMode;
    public Dropdown resolution;
    //public Dropdown refreshRate;
    public Dropdown qualityPreset;
    //FullScreenMode mode;
    // Advanced
    // CHECK VARIOUS GRAPHICAL OPTIONS

    [Header("Audio")]
    public AudioMixer audioMixer;
    public Slider masterVolume;
    public Dropdown language;
    public Dropdown subtitles;
    public Toggle muteAudioOnFocusLoss;

    [Header("Controls")]
    public Dropdown keyboardPreset;
    public Dropdown controllerPreset;

    [Header("Gameplay")]
    public Slider sensitivityX;
    public Slider sensitivityY;
    public Toggle invertYAxis;
    public Toggle controllerVibration;

    [Header("Accessibility")]
    public Toggle tutorials;
    public Toggle aimAssist;
    public Dropdown colourBlindMode;
    public Slider reticleColourRed;
    public Slider reticleColourGreen;
    public Slider reticleColourBlue;




    //float rowHeight;


    private void Start()
    {
        //rowHeight = 0;




    }



    #region Set up options
    /*
    void InstantiateSelectableInList(Selectable s, Selectable prefab)
    {
        s = Instantiate(prefab, options.content);
        RectTransform rt = s.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(0, rowHeight);
        rowHeight -= rt.rect.height;
    }
    */
    void SetupDropdown(Dropdown d, List<string> textOptions, UnityEngine.Events.UnityAction<int> call, int setOnStart)
    {
        d.AddOptions(textOptions);
        d.onValueChanged.AddListener(call);
    }

    void SetupToggle(Toggle t, UnityEngine.Events.UnityAction<bool> call, bool setOnStart)
    {
        t.enabled = setOnStart;
        t.onValueChanged.AddListener(call);
    }

    void SetupSlider(Slider s, UnityEngine.Events.UnityAction<float> call, float setOnStart)
    {
        s.onValueChanged.AddListener(call);
    }
    #endregion

    #region Graphics
    void SetupMonitor()
    {
        List<string> monitors = new List<string>();
        foreach (Display d in Display.displays)
        {
            monitors.Add(d.ToString());
            // Update display to show correct variable
        }
        SetupDropdown(monitor, monitors, ApplyMonitor, 0);
    }
    void ApplyMonitor(int m)
    {
        //Screen.
        //Display.
    }


    void SetupFullscreen()
    {
        SetupToggle(fullScreenMode, ApplyFullscreen, Screen.fullScreen);
    }
    void ApplyFullscreen(bool b)
    {
        Screen.fullScreen = b;
    }


    void SetupResolutionAndRefreshRate()
    {
        List<string> resolutions = new List<string>();
        foreach (Resolution r in Screen.resolutions)
        {
            string text = r.width + " x " + r.height + " @ " + r.refreshRate;
            resolutions.Add(text);
        }
        SetupDropdown(resolution, resolutions, ApplyResolutionAndRefreshRate, 0);
    }
    void ApplyResolutionAndRefreshRate(int i)
    {
        Resolution r = Screen.resolutions[i];
        Screen.SetResolution(r.width, r.height, Screen.fullScreen, r.refreshRate);
    }
    /*
    void SetupResolution()
    {
        List<Vector2> resolutions = new List<Vector2>();
        List<string> labels = new List<string>();

        foreach (Resolution r in Screen.resolutions)
        {
            Vector2 res = new Vector2(r.width, r.height);
            if (!resolutions.Contains(res))
            {
                resolutions.Add(res);
                labels.Add(res.x + " X " + res.y);
            }
        }

        SetupDropdown(resolution, labels, ApplyResolution, 0);
    }

    void SetupRefreshRate(Vector2 referenceResolution)
    {
        List<int> indexes = new List<int>();
        
        for(int i = 0; i < Screen.resolutions.Length; i++)
        {
            Resolution r = Screen.resolutions[i];
            Vector2 res = new Vector2(r.width, r.height);
            if (res == referenceResolution)
            {
                indexes.Add(i);
            }
        }
    }

    void ApplyResolution(int index)
    {
        Vector2 resolution;
        SetupRefreshRate
    }
    */

    void SetupGraphicsQuality()
    {
        List<string> presets = new List<string>(QualitySettings.names);
        SetupDropdown(qualityPreset, presets, ApplyGraphicsQuality, 0);
    }
    void ApplyGraphicsQuality(int q)
    {
        QualitySettings.SetQualityLevel(q);
    }
    #endregion



    #region Audio
    void SetupMasterVolume()
    {
        SetupSlider(masterVolume, ApplyMasterVolume, 0);
    }
    void ApplyMasterVolume(float v)
    {
        audioMixer.SetFloat("MasterVolume", v);
    }

    #endregion

    #region Controls

    #endregion

    #region Gameplay

    #endregion

    #region Accessibility

    #endregion
}
