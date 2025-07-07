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
        UIInputHandler.instance.ClosedMenu();
        UIInputHandler.instance.defaultButton = ResumeButton.gameObject;
        UIInputHandler.instance.OpenedMenu();
    }

    public void OpenSettingsScreen()
    {
        MainScreen.SetActive(false);
        SettingsScreen.SetActive(true);
        UIInputHandler.instance.defaultButton = SettingsBackButton.gameObject;
        UIInputHandler.instance.ClosedMenu();
        UIInputHandler.instance.OpenedMenu();
        //SettingsBackButton.Select();
    }
}
