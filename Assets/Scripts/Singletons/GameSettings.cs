using UnityEngine;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour
{
    public Toggle screenFlashToggle;
    public Toggle screenShakeToggle;

    public Slider sensitivitySlider;
    public CameraManager cameraManager;

    private static string screenFlashKey = "ScreenFlash";
    private static string screenShakeKey = "ScreenShake";
    private static string sensitivityKey = "CameraSensitivity";

    void Start()
    {
        ScreenFlashAccesibilitySetting();
        ScreenShakeAccesibilitySetting();
        SensitivitySetting();
    }

    void ScreenFlashAccesibilitySetting()
    {
        // Load saved setting
        bool screenFlashDisabled = PlayerPrefs.GetInt(screenFlashKey, 0) == 1;

        // Apply to toggle
        screenFlashToggle.isOn = screenFlashDisabled;

        // Hook up listener
        screenFlashToggle.onValueChanged.AddListener((value) =>
        {
            PlayerPrefs.SetInt(screenFlashKey, value ? 1 : 0);
            PlayerPrefs.Save();
        });
    }

    void ScreenShakeAccesibilitySetting()
    {
        // Load saved setting
        bool screenShakeDisabled = PlayerPrefs.GetInt(screenShakeKey, 0) == 1;

        // Apply to toggle
        screenShakeToggle.isOn = screenShakeDisabled;

        // Hook up listener
        screenShakeToggle.onValueChanged.AddListener((value) =>
        {
            PlayerPrefs.SetInt(screenShakeKey, value ? 1 : 0);
            PlayerPrefs.Save();
        });
    }

    void SensitivitySetting()
    {
        // Load saved sensitivity (default 0.5 if none saved yet)
        float savedSensitivity = PlayerPrefs.GetFloat(sensitivityKey, 0.5f);

        // Apply to slider
        sensitivitySlider.value = savedSensitivity;

        // Apply to camera
        if (cameraManager != null)
            cameraManager.set_camera_sensitivity(savedSensitivity);

        // Hook up listener
        sensitivitySlider.onValueChanged.AddListener((value) =>
        {
            PlayerPrefs.SetFloat(sensitivityKey, value);
            PlayerPrefs.Save();

            if (cameraManager != null)
                cameraManager.set_camera_sensitivity(value);
        });
    }


    public static bool IsScreenFlashDisabled()
    {
        return PlayerPrefs.GetInt(screenFlashKey, 0) == 1;
    }

    public static bool IsScreenShakeDisabled()
    {
        return PlayerPrefs.GetInt(screenShakeKey, 0) == 1;
    }
}