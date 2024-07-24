using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] Dropdown resolutionDropDown;
    [SerializeField] Slider slider_effects;
    [SerializeField] Slider slider_bso;
    [SerializeField] Dropdown graphicsDropdown;
    [SerializeField] Toggle fullScreenToggle;


    Resolution[] resolutions;
    void Start()
    {
        // Set volume as last value saved
        float storedVolume = PlayerPrefs.GetFloat("SliderEffectsVolumeLevel", -15);
        slider_effects.value = storedVolume;

        // Set volume as last value saved
        storedVolume = PlayerPrefs.GetFloat("SliderBSOVolumeLevel", -15);
        slider_bso.value = storedVolume;

        // Get resolution options and set stored value;
        int w = PlayerPrefs.GetInt("ResolutionW", Screen.currentResolution.width);
        int h = PlayerPrefs.GetInt("ResolutionH", Screen.currentResolution.height);

        resolutions = Screen.resolutions;
        resolutionDropDown.ClearOptions();

        List<string> resolutionOptions = new List<string>();
        int currentRes = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            resolutionOptions.Add(option);

            if(resolutions[i].width == w &&
                resolutions[i].height == h)
            {
                currentRes = i;
            }
        }
        resolutionDropDown.AddOptions(resolutionOptions);
        resolutionDropDown.value = currentRes;
        resolutionDropDown.RefreshShownValue();

        // Set quality value
        int quality = PlayerPrefs.GetInt("Quality", 3);
        graphicsDropdown.value = quality;

        // Set toggle
        bool isFullScreen = true;
        if (PlayerPrefs.GetInt("Fullscreen", 1) != 1)
            isFullScreen = false;
        fullScreenToggle.isOn = isFullScreen;

    }

    public void setVolume_Effects(float v)
    {
        audioMixer.SetFloat("Effects_Volume", v);
        PlayerPrefs.SetFloat("SliderEffectsVolumeLevel", v);
    }

    public void setVolume_BSO(float v)
    {
        audioMixer.SetFloat("BSO_Volume", v);
        PlayerPrefs.SetFloat("SliderBSOVolumeLevel", v);
    }

    public void setQuality(int i)
    {
        QualitySettings.SetQualityLevel(i);
        PlayerPrefs.SetInt("Quality", i);
    }

    public void setResolution(int i)
    {
        Resolution res = resolutions[i];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
        PlayerPrefs.SetInt("ResolutionW", res.width);
        PlayerPrefs.SetInt("ResolutionH", res.height);

    }

    public void setFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
        if(isFullScreen)
            PlayerPrefs.SetInt("Fullscreen", 1);
        else
            PlayerPrefs.SetInt("Fullscreen", 0);
    }
}
