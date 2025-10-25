using UnityEngine;

public class PauseMenuManager : MonoBehaviour
{
    float lastPressedTime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ScriptRefrenceSingleton.instance.gameplayInput.playerInput.actions["Pause"].performed += context => { PauseMenuToggle(); };
        ScriptRefrenceSingleton.instance.gameplayInput.playerInput.actions["ClosePause"].performed += context => { PauseMenuToggle(); };
    }

    void PauseMenuToggle()
    {
        print("trying to pause");
        // Fixes a bug that causes the first time opening the pause menu to close right away
        if (lastPressedTime > Time.realtimeSinceStartup)
        {
            return;
        }
        lastPressedTime = Time.realtimeSinceStartup + 0.15f;
        if (ScriptRefrenceSingleton.instance.cutsceneManager.cutsceneIsRunning)
        {
            ScriptRefrenceSingleton.instance.cutsceneManager.SkipCutscene();
        }
        else
        {
            ScriptRefrenceSingleton.instance.gameplayUtils.Toggle_Pause_Menu();
        }
     }
}
