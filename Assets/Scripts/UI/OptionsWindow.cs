using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsWindow : MenuWindow
{
    [Header("Video")]
    public Dropdown screenResolution;
    public Dropdown qualityPreset;
    public Toggle fullScreen;

    [Header("Audio")]
    public AudioMixer mainMixer;
    public Slider masterVolumeSlider;
    public float minVolume = -80;
    public float maxVolume = 20;

    Resolution[] resolutions;

    // Use this for initialization
    void Start()
    {
        // Set up basic video options
        SetupResolution();
        SetupFullscreen();
        SetupQualityPreset();

        // Set up audio options
        SetupMasterVolume();
    }









    #region Video settings - Basic
    void SetupResolution()
    {
        resolutions = Screen.resolutions;
        screenResolution.ClearOptions();
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

        screenResolution.AddOptions(resOptions);
        screenResolution.value = currentResolutionIndex;
        screenResolution.RefreshShownValue();

        screenResolution.onValueChanged.AddListener(ApplyResolution);
    }
    public void ApplyResolution(int resolutionIndex)
    {
        Resolution r = resolutions[resolutionIndex];
        Screen.SetResolution(r.width, r.height, Screen.fullScreen);
    }

    void SetupFullscreen()
    {
        fullScreen.onValueChanged.AddListener(ApplyFullscreen);
    }
    public void ApplyFullscreen(bool isTrue)
    {
        Screen.fullScreen = isTrue;
    }

    void SetupQualityPreset()
    {
        qualityPreset.ClearOptions();
        List<string> options = new List<string>();
        foreach(string s in QualitySettings.names)
        {
            options.Add(s);
        }
        qualityPreset.AddOptions(options);

        qualityPreset.onValueChanged.AddListener(ApplyQualityPreset);
    }
    public void ApplyQualityPreset(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        print("Graphics quality changed to " + qualityIndex);
    }
    #endregion

    #region Video settings - Advanced

    #endregion

    #region Audio settings
    void SetupMasterVolume()
    {
        masterVolumeSlider.minValue = minVolume;
        masterVolumeSlider.maxValue = maxVolume;
        masterVolumeSlider.onValueChanged.AddListener(ApplyMasterVolume);
    }
    public void ApplyMasterVolume(float volume)
    {
        mainMixer.SetFloat("MasterVolume", volume);
    }




    #endregion

}


/*

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsWindow : MenuWindow
{
    [Header("Audio")]
    public AudioMixer audioMixer;
    public Slider volumeSlider;
    float currentVolume;

    [Header("Video")]
    public Dropdown resolutionDropdown;
    public Dropdown qualityDropdown;
    public Dropdown textureDropdown;
    public Dropdown aaDropdown;
    Resolution[] resolutions;
    public int customQualitySettingPrefix = 6;



    // Start is called before the first frame update
    void Start()
    {
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        resolutions = Screen.resolutions;
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);
            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.RefreshShownValue();
        LoadSettings(currentResolutionIndex);
    }



    #region Apply settings

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("Volume", volume);
        currentVolume = volume;
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetTextureQuality(int textureIndex)
    {
        QualitySettings.masterTextureLimit = textureIndex;
        qualityDropdown.value = customQualitySettingPrefix;
    }

    public void SetAntiAliasing(int aaIndex)
    {
        QualitySettings.antiAliasing = aaIndex;
        qualityDropdown.value = customQualitySettingPrefix;
    }

    public void SetQuality(int qualityIndex)
    {
        if (qualityIndex != customQualitySettingPrefix) // if the user is not using any of the presets
        {
            QualitySettings.SetQualityLevel(qualityIndex);
        }

        //textureDropdown.value = QualitySettings.masterTextureLimit;
        //aaDropdown.value = QualitySettings.antiAliasing;

        switch (qualityIndex)
        {
            case 0: // quality level - very low
                textureDropdown.value = 3;
                aaDropdown.value = 0;
                break;
            case 1: // quality level - low
                textureDropdown.value = 2;
                aaDropdown.value = 0;
                break;
            case 2: // quality level - medium
                textureDropdown.value = 1;
                aaDropdown.value = 0;
                break;
            case 3: // quality level - high
                textureDropdown.value = 0;
                aaDropdown.value = 0;
                break;
            case 4: // quality level - very high
                textureDropdown.value = 0;
                aaDropdown.value = 1;
                break;
            case 5: // quality level - ultra
                textureDropdown.value = 0;
                aaDropdown.value = 2;
                break;
        }

        qualityDropdown.value = qualityIndex;
    }

    #endregion

    public void ExitGame()
    {
        Application.Quit();
    }
    public void SaveSettings()
    {
        PlayerPrefs.SetInt("QualitySettingPreference", qualityDropdown.value);
        PlayerPrefs.SetInt("ResolutionPreference", resolutionDropdown.value);
        PlayerPrefs.SetInt("TextureQualityPreference", textureDropdown.value);
        PlayerPrefs.SetInt("AntiAliasingPreference", aaDropdown.value);
        PlayerPrefs.SetInt("FullscreenPreference", Convert.ToInt32(Screen.fullScreen));
        PlayerPrefs.SetFloat("VolumePreference", currentVolume);
    }

    public void LoadSettings(int currentResolutionIndex)
    {
        if (PlayerPrefs.HasKey("QualitySettingPreference"))
        {
            qualityDropdown.value = PlayerPrefs.GetInt("QualitySettingPreference");
        }
        else
        {
            qualityDropdown.value = 3;
        }

        if (PlayerPrefs.HasKey("ResolutionPreference"))
        {
            resolutionDropdown.value = PlayerPrefs.GetInt("ResolutionPreference");
        }
        else
        {
            resolutionDropdown.value = currentResolutionIndex;
        }

        if (PlayerPrefs.HasKey("TextureQualityPreference"))
        {
            textureDropdown.value = PlayerPrefs.GetInt("TextureQualityPreference");
        }
        else
        {
            textureDropdown.value = 0;
        }
            
        if (PlayerPrefs.HasKey("AntiAliasingPreference"))
        {
            aaDropdown.value = PlayerPrefs.GetInt("AntiAliasingPreference");
        }
        else
        {
            aaDropdown.value = 1;
        }
            
        if (PlayerPrefs.HasKey("FullscreenPreference"))
        {
            Screen.fullScreen = Convert.ToBoolean(PlayerPrefs.GetInt("FullscreenPreference"));
        }
        else
        {
            Screen.fullScreen = true;
        }
            
        if (PlayerPrefs.HasKey("VolumePreference"))
        {
            volumeSlider.value = PlayerPrefs.GetFloat("VolumePreference");
        } 
        else
        {
            float sliderValue = 0;
            audioMixer.GetFloat("Volume", out sliderValue);
            volumeSlider.value = sliderValue;
        }
    }

    
    void HypotheticalQualitySettingUpdateThingIGotFromAGuyOnDiscord()
    {
        // From someone called Foonix #5812    
    
        qualitySelectorControl.options = QualitySettings.names.Select(name => new Dropdown.OptionData(name)).ToList();
        if (ConfigurationManager.Configuration.visualConfiguration.qualityPreset == -1)
        {
            ConfigurationManager.Configuration.visualConfiguration.qualityPreset = QualitySettings.GetQualityLevel();
        }
        qualitySelectorControl.value = ConfigurationManager.Configuration.visualConfiguration.qualityPreset;

        // There was a separation here, I don't know why, maybe they're different things

        if (Configuration.visualConfiguration.qualityPreset != -1)
        {
            QualitySettings.SetQualityLevel(Configuration.visualConfiguration.qualityPreset, true);
        }
    }
    
}
*/