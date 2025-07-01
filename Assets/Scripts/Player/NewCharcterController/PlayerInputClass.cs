using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputClass : MonoBehaviour, ICharacterInput
{
    PlayerInput playerInput;
    Camera mainCamera;

    public event Action OnJump;
    GameplayInput gameplayInput;

    void Start()
    {
        gameplayInput = GameplayInput.instance;
        playerInput = gameplayInput.playerInput;
        mainCamera = Camera.main;
       
        playerInput.actions["Jump"].performed += Jump;
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


    public bool GetDashInput()
    {
        return playerInput.actions["Sprint"].ReadValue<float>() > 0.1f;
     }

    public bool GetJumpInput()
    {
        return playerInput.actions["Jump"].ReadValue<float>() > 0.1f;
    }


}
