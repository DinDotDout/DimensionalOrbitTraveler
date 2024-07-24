using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class ReloadSettings : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;
    void Start()
    {
        // Set user prefs
        setVolume_BSO();
        setVolume_Effects();
        setQuality();
        setResolution();
        setFullScreen();
    }

    public void setVolume_BSO()
    {
        float storedBSOVolume = PlayerPrefs.GetFloat("SliderBSOVolumeLevel", -15);
        audioMixer.SetFloat("BSO_Volume", storedBSOVolume);
    }

    public void setVolume_Effects()
    {
        float storedEffectsVolume = PlayerPrefs.GetFloat("SliderEffectsVolumeLevel", -15);
        audioMixer.SetFloat("Effects_Volume", storedEffectsVolume);
    }

    public void setQuality()
    {
        int quality = PlayerPrefs.GetInt("Quality", 3);
        QualitySettings.SetQualityLevel(quality);
    }

    public void setResolution()
    {
        int w = PlayerPrefs.GetInt("ResolutionW", Screen.currentResolution.width);
        int h = PlayerPrefs.GetInt("ResolutionH", Screen.currentResolution.height);
        Screen.SetResolution(w, h, Screen.fullScreen);
    }

    public void setFullScreen()
    {
        bool isFullScreen = true;
        if (PlayerPrefs.GetInt("Fullscreen",1) != 1)
            isFullScreen = false;
        Screen.fullScreen = isFullScreen;
    }
}
