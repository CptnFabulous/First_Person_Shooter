using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class CurrentPlayerSettings
{
    // Player stats
    float playerFieldOfView;
    float sensitivityX;
    float sensitivityY;

    
    





    void ApplySettings(PlayerHandler ph)
    {
        ph.pc.fieldOfView.defaultValue = playerFieldOfView;
        ph.pc.sensitivityX = sensitivityX;
        ph.pc.sensitivityY = sensitivityY;


    }
}


public class GraphicsMenu : MenuWindow
{
    [Header("General")]
    public Button applyButton;


    [Header("Settings")]

    
    public Dropdown monitor;
    Display previousMonitor;


    public Toggle fullScreenMode;
    //FullScreenMode mode;
    public Dropdown resolution;
    List<Vector2> resolutions;
    
    public Dropdown refreshRate;
    List<int> refreshRates;




    public Dropdown qualityPreset;
    // Advanced
    // CHECK VARIOUS GRAPHICAL OPTIONS


    Display prevMonitor;
    bool prevFullScreenMode;
    Resolution prevResolution;
    int prevQualityLevel;


    public override void OnEnable()
    {
        base.OnEnable();

        previousMonitor = Display.main;


        

        //prevMonitor = Display.
        /*
        SetupMonitor();
        SetupFullscreen();
        SetupResolutionAndRefreshRate();
        SetupGraphicsQuality();
        */
    }



    void SetupResolutions()
    {
        List<Vector2> foundResolutions = new List<Vector2>();
        foreach(Resolution r in Screen.resolutions)
        {
            Vector2 wh = new Vector2(r.width, r.height);
            if (!foundResolutions.Contains(wh))
            {
                foundResolutions.Add(wh);
            }
        }

        resolutions = foundResolutions;
    }




    class ResolutionAndRefreshRates
    {
        public int width;
        public int height;
        public List<int> refreshRates = new List<int>();

        public ResolutionAndRefreshRates(int w, int h, int refreshRate)
        {
            width = w;
            height = h;
            refreshRates.Add(refreshRate);
        }
    }

    void SetupResolutionAndRefreshRateDropdowns()
    {
        List<ResolutionAndRefreshRates> options = new List<ResolutionAndRefreshRates>();
        foreach (Resolution r in Screen.resolutions)
        {

            // Check if a ResolutionAndRefreshRates exists with r's resolution.
            if (true)
            {
                // If so, check it and see if it has r's refresh rate.
                if (false)
                {
                    // If not, add the refresh rate.
                }
            }
            else
            {
                // If not, create one and add the current refresh rate to it.
                options.Add(new ResolutionAndRefreshRates(r.width, r.height, r.refreshRate));
            }
        }
    }
















    void SetupRefreshRates()
    {
        List<Resolution> refreshRates = new List<Resolution>();

        Vector2 currentResolution = new Vector2(Screen.width, Screen.height);
        foreach (Resolution r in Screen.resolutions)
        {
            Vector2 wh = new Vector2(r.width, r.height);

            // If the resolution is the same but the framerate isn't, add it. This way you get all the framerates for the current resolution.
            if (wh == currentResolution && !refreshRates.Contains(r))
            {
                refreshRates.Add(r);
            }
        }

        // Theoretically sorts refresh rates. I kinda copied the code off the internet.
        refreshRates.Sort((rr1, rr2) => rr1.refreshRate.CompareTo(rr2.refreshRate));

        /*
        List<int> refreshRates = new List<int>();
        
        Vector2 crurentResolution = new Vector2(Screen.width, Screen.height);
        foreach (Resolution r in Screen.resolutions)
        {
            Vector2 wh = new Vector2(r.width, r.height);
            int rr = r.refreshRate;

            if (wh == crurentResolution && !refreshRates.Contains(rr))
            {
                refreshRates.Add(rr);
            }
        }

        // Theoretically sorts refresh rates. I kinda copied the code off the internet.
        refreshRates.Sort((rr1, rr2) => rr1.CompareTo(rr2));
        */
    }













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
    void SetupFullscreen()
    {
        SetupToggle(fullScreenMode, ApplyFullscreen, Screen.fullScreen);
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
    void SetupGraphicsQuality()
    {
        List<string> presets = new List<string>(QualitySettings.names);
        SetupDropdown(qualityPreset, presets, ApplyGraphicsQuality, 0);
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

    void ApplyMonitor(int m)
    {
        //Screen.
        //Display.

    }
    void ApplyFullscreen(bool b)
    {
        Screen.fullScreen = b;
    }
    void ApplyResolutionAndRefreshRate(int i)
    {
        Resolution r = Screen.resolutions[i];
        Screen.SetResolution(r.width, r.height, Screen.fullScreen, r.refreshRate);
    }
    void ApplyGraphicsQuality(int q)
    {
        QualitySettings.SetQualityLevel(q);
    }


    #endregion


    void ApplyVideoSettings()
    {

    }

    #region Set up option interactables
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

}
