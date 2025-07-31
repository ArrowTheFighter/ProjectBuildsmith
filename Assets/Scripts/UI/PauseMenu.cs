using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject MainScreen;
    [SerializeField] GameObject SettingsScreen;
    [SerializeField] Button ResumeButton;
    [SerializeField] Button SettingsBackButton;

    [SerializeField] GameObject[] settingsTabs;


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

    public void OpenControlsTab()
    {
        DisableAllSettingsPanels();
        settingsTabs[0].SetActive(true);
    }

    public void OpenGraphicsTab()
    {
        DisableAllSettingsPanels();
        settingsTabs[1].SetActive(true);
    }

    public void OpenAudioTab()
    {
        DisableAllSettingsPanels();
        settingsTabs[2].SetActive(true);
    }
    
    public void OpenAccessibilityTab()
    {
        DisableAllSettingsPanels();
        settingsTabs[3].SetActive(true);
    }

    void DisableAllSettingsPanels()
    {
        for (int i = 0; i < settingsTabs.Length; i++)
        {
            settingsTabs[i].SetActive(false);
        }
    }
}
