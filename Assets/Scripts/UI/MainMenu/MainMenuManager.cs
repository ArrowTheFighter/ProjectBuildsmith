using System.Collections;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public GameObject MainMenuCam;
    public GameObject MainMenuCanvas;
    public GameObject MainMenuContents;

    [Header("Main Panel")]
    public GameObject MainPanel;
    public GameObject PlayButton;

    [Header("Settings Panel")]
    public GameObject SettingsPanel;
    public GameObject settingsBackButton;

    [Header("Credits Panel")]
    public GameObject CreditsPanel;
    public GameObject creditsBackButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //GameplayUtils.instance.OpenMenu();
        StartCoroutine(freezePlayer());
        OpenMainPanel();
    }

    IEnumerator freezePlayer()
    {
        yield return null;
        GameplayUtils.instance.OpenMenu();
    }

    public void SetGameToPlaying()
    {
        GameplayUtils.instance.CloseMenu();
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
    }

    public void OpenMainPanel()
    {
        CloseAllPanels();
        MainPanel.SetActive(true);
        UIInputHandler.instance.ClosedMenu();
        UIInputHandler.instance.defaultButton = PlayButton;
        UIInputHandler.instance.OpenedMenu();
    }

    public void OpenSettingsPanel()
    {
        CloseAllPanels();
        SettingsPanel.SetActive(true);
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
}
