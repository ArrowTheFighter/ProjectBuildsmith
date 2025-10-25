using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputClass : MonoBehaviour, ICharacterInput
{
    PlayerInput playerInput;
    Camera mainCamera;

    public event Action OnJump;
    public event Action OnDive;

    GameplayInput gameplayInput;

    void Awake()
    {
        ScriptRefrenceSingleton.OnScriptLoaded += SetupBinds;
    }

    void SetupBinds()
    {
        gameplayInput = ScriptRefrenceSingleton.instance.gameplayInput;
        playerInput = gameplayInput.playerInput;

        playerInput.actions["Jump"].performed += Jump;
        playerInput.actions["Sprint"].performed += Dive;
        ScriptRefrenceSingleton.OnScriptLoaded -= SetupBinds;
        ScriptRefrenceSingleton.instance.gameplayUtils.OnStartMoveToMainMenu += OnSceneReload;
    }

    void OnSceneReload()
    {
        playerInput.actions["Jump"].performed -= Jump;
        playerInput.actions["Sprint"].performed -= Dive;
        ScriptRefrenceSingleton.instance.gameplayUtils.OnStartMoveToMainMenu -= OnSceneReload;
    }

    void Start()
    {
        mainCamera = Camera.main;
    }


    public Vector3 GetMovementInput()
    {
        Vector2 input = playerInput.actions["Move"].ReadValue<Vector2>();
        Vector3 CamForward = mainCamera.transform.forward;
        CamForward.y = 0;
        Vector3 forward_move = CamForward.normalized * input.y;
        Vector3 CamRight = mainCamera.transform.right;
        CamRight.y = 0;
        Vector3 right_move = CamRight.normalized * input.x;
        Vector3 direction = forward_move + right_move;

        return direction;
    }

    void Jump(InputAction.CallbackContext context)
    {
        OnJump?.Invoke();
    }

    void Dive(InputAction.CallbackContext context)
    {
        OnDive?.Invoke();
    }


    public bool GetDashInput()
    {
        return playerInput.actions["Sprint"].ReadValue<float>() > 0.1f;
    }

    public bool GetJumpInput()
    {
        return playerInput.actions["Jump"].ReadValue<float>() > 0.1f;
    }


}
