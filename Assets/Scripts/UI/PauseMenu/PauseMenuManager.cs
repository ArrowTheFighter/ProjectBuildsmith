using UnityEngine;

public class PauseMenuManager : MonoBehaviour
{
    float lastPressedTime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameplayInput.instance.playerInput.actions["Pause"].performed += context => { PauseMenuToggle(); };
        GameplayInput.instance.playerInput.actions["ClosePause"].performed += context => { PauseMenuToggle(); };
    }

    void PauseMenuToggle()
    {
        // Fixes a bug that causes the first time opening the pause menu to close right away
        if (lastPressedTime > Time.realtimeSinceStartup)
        {
            return;
        }
        lastPressedTime = Time.realtimeSinceStartup + 0.1f;
        GameplayUtils.instance.Toggle_Pause_Menu();
     }
}
