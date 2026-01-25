using UnityEngine;
using TMPro;

public class ColorBlindModeToggle : MonoBehaviour
{
    [SerializeField] private TMP_StyleSheet normalStyle;
    [SerializeField] private TMP_StyleSheet colorBlindStyle;

    private void OnEnable()
    {
        ScriptRefrenceSingleton.instance.gameSettings
            .OnColorBlindModeChanged += ApplyStyle;
    }

    private void OnDisable()
    {
        ScriptRefrenceSingleton.instance.gameSettings
            .OnColorBlindModeChanged -= ApplyStyle;
    }

    private void Start()
    {
        ApplyStyle(ScriptRefrenceSingleton.instance.gameSettings
            .IsColorBlindModeDisabled());
    }


    private void ApplyStyle(bool isColorBlindModeDisabled)
    {
        TMP_Settings.defaultStyleSheet =
            isColorBlindModeDisabled ? colorBlindStyle : normalStyle;

        RefreshAllTMPText();
    }

    public static void RefreshAllTMPText()
    {
        var allTMP = Object.FindObjectsByType<TMP_Text>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
            );

        foreach (var tmp in allTMP)
        {
            tmp.havePropertiesChanged = true;
            tmp.ForceMeshUpdate();
        }
    }
}