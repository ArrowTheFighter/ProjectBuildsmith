using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject MainScreen;
    [SerializeField] GameObject SettingsScreen;
    [SerializeField] Button ResumeButton;
    [SerializeField] Button SettingsBackButton;

    public void OpenMainScreen()
    {
        SettingsScreen.SetActive(false);
        MainScreen.SetActive(true);
        ResumeButton.Select();
    }

    public void OpenSettingsScreen()
    {
        MainScreen.SetActive(false);
        SettingsScreen.SetActive(true);
        SettingsBackButton.Select();
    }
}
