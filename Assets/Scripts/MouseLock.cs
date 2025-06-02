using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLock : MonoBehaviour
{
    PlayerInput playerInput;
    bool can_capture_mouse;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Capture_Mouse()
    {
        Cursor.lockState = CursorLockMode.Locked;
        can_capture_mouse = true;
    }

    public void Release_Mouse()
    {
        Cursor.lockState = CursorLockMode.None;
        can_capture_mouse = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            if(can_capture_mouse && playerInput.actions["MouseClick"].ReadValue<float>() > 0)
            {
                Capture_Mouse();
            }
        }

    }
}
