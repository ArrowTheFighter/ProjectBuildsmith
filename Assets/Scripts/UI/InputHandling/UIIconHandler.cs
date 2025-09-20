using System;
using TMPro;
using UnityEngine;

public class UIIconHandler : MonoBehaviour
{
    public static UIIconHandler instance;
    public IconDatabase iconDatabase;
    public TextMeshProUGUI[] TextBoxes;
    public TMP_SpriteAsset KeyboardIcons;
    public TMP_SpriteAsset PS4Icons;

    public Action InputDeviceChanged;
    string currentControlScheme = "Keyboard&Mouse";

    void Awake()
    {
        if (instance != this)
            Destroy(instance);
        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UIInputHandler.instance.OnSchemeChange += UpdateTextBoxesIcons;
    }

    void UpdateTextBoxesIcons(string scheme)
    {
        currentControlScheme = scheme;
        InputDeviceChanged?.Invoke();
        // switch (scheme)
        // {
        //     case "Keyboard&Mouse":
        //         SetTextBoxesToUseIconSet(KeyboardIcons);
        //         break;
        //     case "Gamepad":
        //         SetTextBoxesToUseIconSet(PS4Icons);
        //         break;
        // }
    }

    void SetTextBoxesToUseIconSet(TMP_SpriteAsset set)
    {
        foreach (TextMeshProUGUI textMesh in TextBoxes)
        {
            textMesh.spriteAsset = set;
        }
    }

    public string FormatText(string rawText)
    {
        switch (currentControlScheme)
        {
            case "Keyboard&Mouse":
                foreach (var iconEntry in iconDatabase.icons)
                {
                    rawText = rawText.Replace($"<icon={iconEntry.Action_Name}>", $"<sprite name={iconEntry.Icon_Name}>");
                }
                break;
            case "Gamepad":
                foreach (var iconEntry in iconDatabase.icons)
                {
                    rawText = rawText.Replace($"<icon={iconEntry.Action_Name}>", $"<sprite name={iconEntry.Playstation_Icon_Name}>");
                }
                break;
        }
        // foreach (var iconEntry in iconDatabase.icons)
        // {
        //     rawText = rawText.Replace($"<icon={iconEntry.Action_Name}>", $"<sprite name={iconEntry.Icon_Name}>");
        // }
        return rawText;
    }
}
