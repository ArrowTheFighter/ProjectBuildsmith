using EasyTextEffects.Editor.EditorDocumentation;
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
    [Header("Sensitivity Adjustments")]
    [SerializeField] float SensitivityLow = 0.01f;
    [SerializeField] float SensitivityHigh = 3;
    float X_speed_adjustment = 1;
    float Y_speed_adjustment = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {
        playerInput = GameplayInput.instance.playerInput;
        playerInput.onControlsChanged += OnControlsChanged;

        GameSettings.instance.OnCameraSensativityChanged += set_camera_sensitivity;
        //This gets set in GameSettings instead to save/load the value
        //set_camera_sensitivity(0.25f);
    }


    // Update is called once per frame
    void OnControlsChanged(PlayerInput input)
    {
        string currentScheme = input.currentControlScheme;
        if (currentScheme == "Keyboard&Mouse")
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
        else if (currentScheme == "Gamepad")
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
        }

    }


    public void set_camera_sensitivity(float sensitivity)
    {
        float adjustedValue = Mathf.Lerp(SensitivityLow, SensitivityHigh, Mathf.Pow(sensitivity, 1.75f));

        //float adjustedValue = Mathf.Lerp(SensitivityLow, SensitivityHigh, sensitivity);
        X_speed_adjustment = adjustedValue;
        Y_speed_adjustment = adjustedValue;
        OnControlsChanged(GameplayInput.instance.playerInput);
    }
}