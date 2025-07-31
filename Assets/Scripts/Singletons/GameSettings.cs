using UnityEngine;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour
{
    public Toggle screenFlashToggle;
    public Toggle screenShakeToggle;

    private static string screenFlashKey = "ScreenFlash";
    private static string screenShakeKey = "ScreenShake";

    void Start()
    {
        ScreenFlashAccesibilitySetting();
        ScreenShakeAccesibilitySetting();
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

    public static bool IsScreenFlashDisabled()
    {
        return PlayerPrefs.GetInt(screenFlashKey, 0) == 1;
    }

    public static bool IsScreenShakeDisabled()
    {
        return PlayerPrefs.GetInt(screenShakeKey, 0) == 1;
    }
}