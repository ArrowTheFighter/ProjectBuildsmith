using System;
using TMPro;
using UnityEngine;
using System.Text.RegularExpressions;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using EasyTextEffects.Editor.MyBoxCopy.Extensions;

public class UIIconHandler : MonoBehaviour
{
    public static UIIconHandler instance;
    public IconDatabase iconDatabase;
    public TextMeshProUGUI[] TextBoxes;
    public TMP_SpriteAsset KeyboardIcons;
    public TMP_SpriteAsset PS4Icons;

    public Action InputDeviceChanged;
    public string CurrentControlDevice = "Keyboard";
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
        //InputSystem.onEvent += DeviceChanged;
        GameplayInput.instance.playerInput.onActionTriggered += DeviceChanged;
    }

    void DeviceChanged(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && context.control != null)
        {
            //Debug.Log($"Device changed: {context.control.device.name}");
            if (CurrentControlDevice != context.control.device.name)
            {
                CurrentControlDevice = context.control.device.name;
                InputDeviceChanged?.Invoke();
            }
        }
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
        if (string.IsNullOrEmpty(rawText)) return rawText;
        switch (currentControlScheme)
        {
            case "Keyboard&Mouse":
                foreach (var iconEntry in iconDatabase.icons)
                {
                    //rawText = rawText.Replace($"<icon={iconEntry.Action_Name}>", $"<sprite name={iconEntry.Icon_Name}>");
                    rawText = Regex.Replace(
                        rawText,
                        $@"<icon={Regex.Escape(iconEntry.Action_Name)}>",  // pattern
                        $"<sprite name={iconEntry.Icon_Name}>",            // replacement
                        RegexOptions.IgnoreCase                           // ignore case
                    );
                }
                break;
            case "Gamepad":
                foreach (var iconEntry in iconDatabase.icons)
                {

                    string iconText = iconEntry.Xbox_Icon_Name;

                    string lower = CurrentControlDevice.ToLower();

                    print($" current control deveice: {CurrentControlDevice}");

                    if (lower.Contains("playstation") || lower.Contains("dualshock") || lower.Contains("duelsense"))
                    {
                        iconText = iconEntry.Playstation_Icon_Name;
                    }
                    else if (lower.Contains("xbox") || lower.Contains("xinput"))
                    {
                        iconText = iconEntry.Xbox_Icon_Name;
                    }
                    else if (lower.Contains("switch"))
                    {
                        iconText = iconEntry.Switch_Icon_Name;
                    }
                    // switch (CurrentControlDevice)
                    // {
                    //     case "DualShock4GamepadHID":
                    //         print("showing playstation icon");
                    //         iconText = iconEntry.Playstation_Icon_Name;
                    //         break;
                    //     // case "Xbox":
                    //     //     iconText = iconEntry.Xbox_Icon_Name;
                    //     //     break;
                    //     case "SwitchProControllerHID":
                    //         iconText = iconEntry.Switch_Icon_Name;
                    //         break;
                    //     default:
                    //         iconText = iconEntry.Xbox_Icon_Name;
                    //         break;
                    // }
                    //rawText = rawText.Replace($"<icon={iconEntry.Action_Name}>", $"<sprite name={iconEntry.Playstation_Icon_Name}>");
                    rawText = Regex.Replace(
                        rawText,
                        $@"<icon={Regex.Escape(iconEntry.Action_Name)}>",  // pattern
                        $"<sprite name={iconText}>",            // replacement
                        RegexOptions.IgnoreCase                           // ignore case
                    );
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
