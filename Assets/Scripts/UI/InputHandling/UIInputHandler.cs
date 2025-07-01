using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections;

public class UIInputHandler : MonoBehaviour
{
    public static UIInputHandler instance;

    public GameObject defaultButton;

    public string currentScheme;
    PlayerInput playerInput;

    void Awake()
    {
        if (instance != this)
        {
            Destroy(instance);
        }
        instance = this;
    }

    void Start()
    {
        playerInput = GameplayInput.instance.playerInput;
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
