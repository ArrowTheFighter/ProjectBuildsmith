using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputClass : MonoBehaviour, ICharacterInput
{
    PlayerInput playerInput;
    Camera mainCamera;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        mainCamera = Camera.main;
        print(mainCamera.name);
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

    public bool GetJumpInput()
    {
        return playerInput.actions["Jump"].ReadValue<float>() > 0.1f;
    }

}
