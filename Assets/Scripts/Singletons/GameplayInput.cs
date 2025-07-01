using UnityEngine;
using UnityEngine.InputSystem;

public class GameplayInput : MonoBehaviour
{
    public static GameplayInput instance;

    [HideInInspector] public PlayerInput playerInput;

    void Awake()
    {
        if (instance != this)
        {
            Destroy(instance);
        }
        instance = this;
        playerInput = GetComponent<PlayerInput>();
    }

    public void SwitchToUI()
    {
        playerInput.SwitchCurrentActionMap("UI");
    }

    public void SwitchToGameplay()
    {
        playerInput.SwitchCurrentActionMap("Player");
    }

}
