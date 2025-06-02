using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    PlayerInput playerInput;
    [SerializeField] CinemachineInputAxisController axisController;
    [SerializeField] float base_speed_X = 200;
    [SerializeField] float base_speed_Y = -80;
    [SerializeField] float controller_percentage = 0.125f;
    float X_speed_adjustment = 1;
    float Y_speed_adjustment = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    // Update is called once per frame
    void Update()
    {
        if(playerInput.currentControlScheme == "Keyboard&Mouse")
        {
            foreach (var c in axisController.Controllers)
            {
                if (c.Name == "Look Orbit X")
                {
                    c.Input.Gain = base_speed_X * X_speed_adjustment * controller_percentage;
                    continue;
                } 
                if (c.Name == "Look Orbit Y")
                {
                    c.Input.Gain = base_speed_Y * Y_speed_adjustment * controller_percentage;
                    continue;
                }
            }

        }else if(playerInput.currentControlScheme == "Gamepad")
        {
            foreach (var c in axisController.Controllers)
            {
                if (c.Name == "Look Orbit X")
                {
                    c.Input.Gain = base_speed_X * X_speed_adjustment;
                    continue;
                }
                if (c.Name == "Look Orbit Y")
                {
                    c.Input.Gain = base_speed_Y * Y_speed_adjustment;
                    continue;
                }
            }
        }

    }

    public void set_camera_sensativity(float sensativity)
    {
        X_speed_adjustment = sensativity;
        Y_speed_adjustment = sensativity;
    }

}
