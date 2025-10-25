using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections;
using System;

public class UIInputHandler : MonoBehaviour
{
    public GameObject defaultButton;

    public string currentScheme;
    PlayerInput playerInput;

    public Action<string> OnSchemeChange;

    void Start()
    {
        playerInput = ScriptRefrenceSingleton.instance.gameplayInput.playerInput;
        playerInput.onControlsChanged += ControlsChanged;
        currentScheme = "Keyboard&Mouse";
    }

    void ControlsChanged(PlayerInput input)
    {
        currentScheme = input.currentControlScheme;

        if (currentScheme == "Gamepad" && playerInput.currentActionMap.name == "UI")
        {
            if (defaultButton != null && EventSystem.current.currentSelectedGameObject == null)
            {
                EventSystem.current.SetSelectedGameObject(defaultButton);
            }
        }
        else if (currentScheme == "Keyboard&Mouse")
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
        OnSchemeChange?.Invoke(currentScheme);
    }

    public void OpenedMenu()
    {
        if (currentScheme == "Gamepad" && defaultButton != null && EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(defaultButton);
        }
    }

    public void ClosedMenu()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }

    void Update()
    {
        if (currentScheme == "Keyboard&Mouse")
        {
            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                StartCoroutine(ClearSelectionNextFrame());
            }
        }
    }

    IEnumerator ClearSelectionNextFrame()
    {
        yield return null;
        EventSystem.current.SetSelectedGameObject(null);
     }

}
