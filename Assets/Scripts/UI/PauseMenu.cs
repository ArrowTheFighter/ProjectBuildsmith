using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject MainScreen;
    [SerializeField] GameObject SettingsScreen;
    [SerializeField] Button ResumeButton;
    [SerializeField] Button SettingsBackButton;
    [SerializeField] GameObject DiscordButton;

    [SerializeField] GameObject[] settingsTabs;


    public void OpenMainScreen()
    {
        SettingsScreen.SetActive(false);
        MainScreen.SetActive(true);
        DiscordButton.SetActive(true);
        ScriptRefrenceSingleton.instance.uIInputHandler.ClosedMenu();
        ScriptRefrenceSingleton.instance.uIInputHandler.defaultButton = ResumeButton.gameObject;
        ScriptRefrenceSingleton.instance.uIInputHandler.OpenedMenu();
    }

    public void OpenSettingsScreen()
    {
        MainScreen.SetActive(false);
        SettingsScreen.SetActive(true);
        DiscordButton.SetActive(false);
        GetComponent<SettingsMenuManager>().OpenControlsMenu();
        ScriptRefrenceSingleton.instance.uIInputHandler.defaultButton = SettingsBackButton.gameObject;
        ScriptRefrenceSingleton.instance.uIInputHandler.ClosedMenu();
        ScriptRefrenceSingleton.instance.uIInputHandler.OpenedMenu();
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
