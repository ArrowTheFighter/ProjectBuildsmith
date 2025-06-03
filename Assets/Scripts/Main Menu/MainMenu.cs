using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;
    public GameObject creditsPanel;

    public Button PlayButton;
    public Button CreditBackButton;

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void SettingsMenuOn()
    {
        Application.OpenURL("https://docs.google.com/forms/d/e/1FAIpQLSe5GFDoREVh0tPM4Q06NOxIJRBl5rZ5rYn1AK51UW80t1nvcw/viewform?usp=header");
        //mainMenuPanel.SetActive(false);
        //settingsPanel.SetActive(true);
    }

    public void SettingsMenuOff()
    {
        mainMenuPanel.SetActive(true);
        settingsPanel.SetActive(false);
    }

    public void CreditsMenuOn()
    {
        mainMenuPanel.SetActive(false);
        creditsPanel.SetActive(true);
        CreditBackButton.Select();
    }

    public void CreditsMenuOff()
    {
        mainMenuPanel.SetActive(true);
        creditsPanel.SetActive(false);
        PlayButton.Select();
    }

    public void QuitGame()
    {
        Debug.Log("Game has been quit!");
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }
}
