using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenuManager : MonoBehaviour
{
    float lastPressedTime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Awake()
    {
        ScriptRefrenceSingleton.OnScriptLoaded += BindInputs;
    }

    void BindInputs()
    {
        ScriptRefrenceSingleton.OnScriptLoaded -= BindInputs;

        ScriptRefrenceSingleton.instance.gameplayInput.playerInput.actions["Pause"].performed += PauseMenuToggle;
        ScriptRefrenceSingleton.instance.gameplayInput.playerInput.actions["ClosePause"].performed += PauseMenuToggle;

        ScriptRefrenceSingleton.instance.gameplayUtils.OnStartMoveToMainMenu += UnBindInputs;
    }

    void UnBindInputs()
    {
        ScriptRefrenceSingleton.instance.gameplayUtils.OnStartMoveToMainMenu -= UnBindInputs;

        ScriptRefrenceSingleton.instance.gameplayInput.playerInput.actions["Pause"].performed -= PauseMenuToggle;
        ScriptRefrenceSingleton.instance.gameplayInput.playerInput.actions["ClosePause"].performed -= PauseMenuToggle;
    }

    void PauseMenuToggle(InputAction.CallbackContext context)
    {
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
