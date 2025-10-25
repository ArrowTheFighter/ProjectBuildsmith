using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public GameObject MainMenuCam;
    public GameObject MainMenuCanvas;
    public GameObject MainMenuContents;

    [Header("Main Panel")]
    public GameObject MainPanel;
    public GameObject PlayButton;
    public GameObject DiscordButton;

    [Header("Settings Panel")]
    public GameObject SettingsPanel;
    public GameObject settingsBackButton;

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

    [Header("Credits Panel")]
    public GameObject CreditsPanel;
    public GameObject creditsBackButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //ScriptRefrenceSingleton.instance.gameplayUtils.OpenMenu();
        StartCoroutine(freezePlayer());
        //OpenMainPanel();
    }

    IEnumerator freezePlayer()
    {
        yield return null;
        ScriptRefrenceSingleton.instance.gameplayUtils.OpenMenu();
    }

    public void SetGameToPlaying()
    {
        ScriptRefrenceSingleton.instance.gameplayUtils.CloseMenu();
        ScriptRefrenceSingleton.instance.gameplayUtils.SetCanPause(true);
        MainMenuCam.SetActive(false);
        MainMenuContents.GetComponent<CanvasGroup>().alpha = 0;

    }

    public void HideMainMenuCanvas()
    {
        MainMenuCanvas.SetActive(false);
    }

    public void CloseAllPanels()
    {
        MainPanel.SetActive(false);
        SettingsPanel.SetActive(false);
        CreditsPanel.SetActive(false);
        DiscordButton.SetActive(false);
    }

    public void OpenMainPanel()
    {
        CloseAllPanels();
        MainPanel.SetActive(true);
        DiscordButton.SetActive(true);
        ScriptRefrenceSingleton.instance.uIInputHandler.ClosedMenu();
        ScriptRefrenceSingleton.instance.uIInputHandler.defaultButton = PlayButton;
        ScriptRefrenceSingleton.instance.uIInputHandler.OpenedMenu();
    }

    public void OpenSettingsPanel()
    {
        CloseAllPanels();
        SettingsPanel.SetActive(true);
        CloseAllSettingsMenus();
        OpenControlsMenu();
        ScriptRefrenceSingleton.instance.uIInputHandler.ClosedMenu();
        ScriptRefrenceSingleton.instance.uIInputHandler.defaultButton = settingsBackButton;
        ScriptRefrenceSingleton.instance.uIInputHandler.OpenedMenu();
    }

    public void OpenCreditsPanel()
    {
        CloseAllPanels();
        CreditsPanel.SetActive(true);
        ScriptRefrenceSingleton.instance.uIInputHandler.ClosedMenu();
        ScriptRefrenceSingleton.instance.uIInputHandler.defaultButton = creditsBackButton;
        ScriptRefrenceSingleton.instance.uIInputHandler.OpenedMenu();
    }

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
