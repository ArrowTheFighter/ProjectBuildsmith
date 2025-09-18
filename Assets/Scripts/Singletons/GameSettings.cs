using System;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Xml.Serialization;

public class GameSettings : MonoBehaviour
{

    private static string FilePath =>
        Path.Combine(Application.persistentDataPath, "settings.json");

    public static GameSettings instance;

    public SettingsContainer settingsContainer = new SettingsContainer();

    [Header("Accessibility Settings")]
    public Action<bool> OnScreenFlashChanged;
    public Action<bool> OnScreenShakeChanged;

    public Toggle[] screenFlashToggles;
    public Toggle[] screenShakeToggles;


    [Header("Control Settings")]
    public Action<float> OnCameraSensativityChanged;
    public Slider[] sensitivitySliders;

    [Header("Audio Settings")]
    public Action<float> OnMasterVolumeChanged;
    public Action<float> OnMusicVolumeChanged;
    public Action<float> OnSoundEffectsVolumeChanged;

    public Slider[] MasterVolumeSliders;
    public Slider[] MusicVolumeSliders;
    public Slider[] SoundEffectsVolumeSliders;

    [Header("Graphics Settings")]

    public Toggle[] EnableVsyncToggles;
    public Toggle[] EnableAntiAliasingToggles;

    public Slider[] RenderScaleSliders;

    public Action<bool> OnVsyncChanged;
    public Action<bool> OnAntiAliasingChanged;

    public Action<float> OnRenderScaleChanged;



    // private static string screenFlashKey = "ScreenFlash";
    // private static string screenShakeKey = "ScreenShake";
    // private static string sensitivityKey = "CameraSensitivity";


    void Awake()
    {
        if (instance != this)
            Destroy(instance);
        instance = this;
        LoadSettingsFromFile();
    }

    void Start()
    {
        //ScreenFlashAccesibilitySetting();
        //ScreenShakeAccesibilitySetting();
        //SensitivitySetting();
        SetInitalSettings();
        SetUIValues();
    }

    void SetUIValues()
    {
        //Accessibilitiy settings
        foreach (var t in screenFlashToggles) t.isOn = settingsContainer.ScreenFlash;
        foreach (var t in screenShakeToggles) t.isOn = settingsContainer.ScreenShake;

        //Control settings
        foreach (var t in sensitivitySliders) t.value = settingsContainer.CameraSensitivity;

        //Audio settings
        foreach (var t in MasterVolumeSliders) t.value = settingsContainer.MasterVolume;
        foreach (var t in MusicVolumeSliders) t.value = settingsContainer.MusicVolume;
        foreach (var t in SoundEffectsVolumeSliders) t.value = settingsContainer.SoundEffectsVolume;

        //Graphics settings
        foreach (var t in EnableVsyncToggles) t.isOn = settingsContainer.VSync;
        foreach (var t in EnableAntiAliasingToggles) t.isOn = settingsContainer.AntiAliasing;
        foreach (var t in RenderScaleSliders) t.value = settingsContainer.RenderScale;

    }

    void SetInitalSettings()
    {
        // Accessibility
        SetScreenFlashSetting(settingsContainer.ScreenFlash);
        SetScreenShakeSetting(settingsContainer.ScreenShake);

        // Controls
        SetCameraSensitivity(settingsContainer.CameraSensitivity);

        // Audio
        SetMasterVolume(settingsContainer.MasterVolume);
        SetMusicVolume(settingsContainer.MusicVolume);
        SetSoundEffectsVolume(settingsContainer.SoundEffectsVolume);

        // Graphics
        SetVsync(settingsContainer.VSync);
        SetAntiAliasing(settingsContainer.AntiAliasing);
        SetRenderScale(settingsContainer.RenderScale);
    }

    public void SetScreenFlashSetting(bool value)
    {
        settingsContainer.ScreenFlash = value;
        OnScreenFlashChanged?.Invoke(value);
        SaveSettingsToFile();
        SetUIValues();
    }

    public void SetScreenShakeSetting(bool value)
    {
        settingsContainer.ScreenShake = value;
        OnScreenShakeChanged?.Invoke(value);
        SaveSettingsToFile();
        SetUIValues();
    }

    public void SetCameraSensitivity(float value)
    {
        settingsContainer.CameraSensitivity = value;
        OnCameraSensativityChanged?.Invoke(value);
        SaveSettingsToFile();
        SetUIValues();
    }

    public void SetMasterVolume(float value)
    {
        settingsContainer.MasterVolume = value;
        OnMasterVolumeChanged?.Invoke(value);
        SaveSettingsToFile();
        SetUIValues();
    }

    public void SetMusicVolume(float value)
    {
        settingsContainer.MusicVolume = value;
        OnMusicVolumeChanged?.Invoke(value);
        SaveSettingsToFile();
        SetUIValues();
    }

    public void SetSoundEffectsVolume(float value)
    {
        settingsContainer.SoundEffectsVolume = value;
        OnSoundEffectsVolumeChanged?.Invoke(value);
        SaveSettingsToFile();
        SetUIValues();
    }

    public void SetVsync(bool value)
    {
        settingsContainer.VSync = value;
        OnVsyncChanged?.Invoke(value);
        SaveSettingsToFile();
        SetUIValues();
    }

    public void SetAntiAliasing(bool value)
    {
        settingsContainer.AntiAliasing = value;
        OnAntiAliasingChanged?.Invoke(value);
        SaveSettingsToFile();
        SetUIValues();
    }

    public void SetRenderScale(float value)
    {
        settingsContainer.RenderScale = value;
        OnRenderScaleChanged?.Invoke(value);
        SaveSettingsToFile();
        SetUIValues();
    }

    // void ScreenFlashAccesibilitySetting()
    // {
    //     // Load saved setting
    //     bool screenFlashDisabled = PlayerPrefs.GetInt(screenFlashKey, 0) == 1;

    //     // Apply to toggle
    //     screenFlashToggle.isOn = screenFlashDisabled;

    //     // Hook up listener
    //     screenFlashToggle.onValueChanged.AddListener((value) =>
    //     {
    //         PlayerPrefs.SetInt(screenFlashKey, value ? 1 : 0);
    //         PlayerPrefs.Save();
    //     });
    // }

    // void ScreenShakeAccesibilitySetting()
    // {
    //     // Load saved setting
    //     bool screenShakeDisabled = PlayerPrefs.GetInt(screenShakeKey, 0) == 1;

    //     // Apply to toggle
    //     screenShakeToggle.isOn = screenShakeDisabled;

    //     // Hook up listener
    //     screenShakeToggle.onValueChanged.AddListener((value) =>
    //     {
    //         PlayerPrefs.SetInt(screenShakeKey, value ? 1 : 0);
    //         PlayerPrefs.Save();
    //     });
    // }

    // void SensitivitySetting()
    // {
    //     // Load saved sensitivity (default 0.5 if none saved yet)
    //     float savedSensitivity = PlayerPrefs.GetFloat(sensitivityKey, 0.5f);

    //     // Apply to slider
    //     sensitivitySlider.value = savedSensitivity;

    //     // Apply to camera
    //     if (cameraManager != null)
    //         cameraManager.set_camera_sensitivity(savedSensitivity);

    //     // Hook up listener
    //     sensitivitySlider.onValueChanged.AddListener((value) =>
    //     {
    //         PlayerPrefs.SetFloat(sensitivityKey, value);
    //         PlayerPrefs.Save();

    //         if (cameraManager != null)
    //             cameraManager.set_camera_sensitivity(value);
    //     });
    // }


    public bool IsScreenFlashDisabled()
    {
        return settingsContainer.ScreenFlash;
        //return PlayerPrefs.GetInt(screenFlashKey, 0) == 1;
    }

    public bool IsScreenShakeDisabled()
    {
        return settingsContainer.ScreenShake;
        //return PlayerPrefs.GetInt(screenShakeKey, 0) == 1;
    }

    void SaveSettingsToFile()
    {
        string json = JsonUtility.ToJson(settingsContainer, true); // true = pretty print
        File.WriteAllText(FilePath, json);
        Debug.Log("Saved settings to " + FilePath);
    }

    void LoadSettingsFromFile()
    {
        if (File.Exists(FilePath))
        {
            string json = File.ReadAllText(FilePath);
            settingsContainer = JsonUtility.FromJson<SettingsContainer>(json);
        }
        else
        {
            Debug.Log("No settings file found, using defaults.");
            settingsContainer = new SettingsContainer(); // default values
        }
    }

}

[Serializable]
public class SettingsContainer
{
    //Accessibility
    public bool ScreenFlash;
    public bool ScreenShake;

    //Controls
    public float CameraSensitivity = 0.2f;

    //Sound
    public float MasterVolume = 0.2f;
    public float MusicVolume = 0.2f;
    public float SoundEffectsVolume = 0.2f;

    //Graphics
    public bool VSync = true;
    public bool AntiAliasing;
    public float RenderScale = 1f;
}