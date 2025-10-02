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
        //GameplayUtils.instance.OpenMenu();
        StartCoroutine(freezePlayer());
        //OpenMainPanel();
    }

    IEnumerator freezePlayer()
    {
        yield return null;
        GameplayUtils.instance.OpenMenu();
    }

    public void SetGameToPlaying()
    {
        GameplayUtils.instance.CloseMenu();
        GameplayUtils.instance.SetCanPause(true);
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
        UIInputHandler.instance.ClosedMenu();
        UIInputHandler.instance.defaultButton = PlayButton;
        UIInputHandler.instance.OpenedMenu();
    }

    public void OpenSettingsPanel()
    {
        CloseAllPanels();
        SettingsPanel.SetActive(true);
        CloseAllSettingsMenus();
        OpenControlsMenu();
        UIInputHandler.instance.ClosedMenu();
        UIInputHandler.instance.defaultButton = settingsBackButton;
        UIInputHandler.instance.OpenedMenu();
    }

    public void OpenCreditsPanel()
    {
        CloseAllPanels();
        CreditsPanel.SetActive(true);
        UIInputHandler.instance.ClosedMenu();
        UIInputHandler.instance.defaultButton = creditsBackButton;
        UIInputHandler.instance.OpenedMenu();
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
        UIInputHandler.instance.ClosedMenu();
        UIInputHandler.instance.defaultButton = OpenAccessibilityButton.gameObject;
        UIInputHandler.instance.OpenedMenu();
    }

    public void OpenControlsMenu()
    {
        CloseAllSettingsMenus();
        ControlsMenu.SetActive(true);
        OpenControlsButton.interactable = false;
        UIInputHandler.instance.ClosedMenu();
        UIInputHandler.instance.defaultButton = OpenGraphicsButton.gameObject;
        UIInputHandler.instance.OpenedMenu();
    }

    public void OpenGraphicsMenu()
    {
        CloseAllSettingsMenus();
        GraphicsMenu.SetActive(true);
        OpenGraphicsButton.interactable = false;
        UIInputHandler.instance.ClosedMenu();
        UIInputHandler.instance.defaultButton = OpenAudioButton.gameObject;
        UIInputHandler.instance.OpenedMenu();
    }

    public void OpenAccessibilityMenu()
    {
        CloseAllSettingsMenus();
        AccessibilityMenu.SetActive(true);
        OpenAccessibilityButton.interactable = false;
        UIInputHandler.instance.ClosedMenu();
        UIInputHandler.instance.defaultButton = OpenControlsButton.gameObject;
        UIInputHandler.instance.OpenedMenu();
    }
}
