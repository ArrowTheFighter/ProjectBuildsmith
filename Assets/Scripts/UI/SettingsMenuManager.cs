using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuManager : MonoBehaviour
{


    [Header("Settings Menus")]
    public GameObject AudioMenu;
    public GameObject ControlsMenu;
    public GameObject GraphicsMenu;
    public GameObject AccessibilityMenu;

    [Header("Settings Menu Buttons")]
    public Button OpenAudioButton;
    public Button OpenControlsButton;
    public Button OpenGraphicsButton;
    public Button OpenAccessibilityButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void CloseAllSettingsMenus()
    {
        AudioMenu.SetActive(false);
        ControlsMenu.SetActive(false);
        GraphicsMenu.SetActive(false);
        AccessibilityMenu.SetActive(false);

        OpenAudioButton.interactable = true;
        OpenGraphicsButton.interactable = true;
        OpenControlsButton.interactable = true;
        OpenAccessibilityButton.interactable = true;
    }

    public void OpenAudioMenu()
    {
        CloseAllSettingsMenus();
        AudioMenu.SetActive(true);
        OpenAudioButton.interactable = false;
        ScriptRefrenceSingleton.instance.uIInputHandler.ClosedMenu();
        ScriptRefrenceSingleton.instance.uIInputHandler.defaultButton = OpenAccessibilityButton.gameObject;
        ScriptRefrenceSingleton.instance.uIInputHandler.OpenedMenu();
    }

    public void OpenControlsMenu()
    {
        CloseAllSettingsMenus();
        ControlsMenu.SetActive(true);
        OpenControlsButton.interactable = false;
        ScriptRefrenceSingleton.instance.uIInputHandler.ClosedMenu();
        ScriptRefrenceSingleton.instance.uIInputHandler.defaultButton = OpenGraphicsButton.gameObject;
        ScriptRefrenceSingleton.instance.uIInputHandler.OpenedMenu();
    }

    public void OpenGraphicsMenu()
    {
        CloseAllSettingsMenus();
        GraphicsMenu.SetActive(true);
        OpenGraphicsButton.interactable = false;
        ScriptRefrenceSingleton.instance.uIInputHandler.ClosedMenu();
        ScriptRefrenceSingleton.instance.uIInputHandler.defaultButton = OpenAudioButton.gameObject;
        ScriptRefrenceSingleton.instance.uIInputHandler.OpenedMenu();
    }

    public void OpenAccessibilityMenu()
    {
        CloseAllSettingsMenus();
        AccessibilityMenu.SetActive(true);
        OpenAccessibilityButton.interactable = false;
        ScriptRefrenceSingleton.instance.uIInputHandler.ClosedMenu();
        ScriptRefrenceSingleton.instance.uIInputHandler.defaultButton = OpenControlsButton.gameObject;
        ScriptRefrenceSingleton.instance.uIInputHandler.OpenedMenu();
    }
}
