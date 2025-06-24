using UnityEngine;

public class PlayerThirdPersonCamera : MonoBehaviour
{
    [Header("Refrences")]
    public Transform playerVisuals;
    public Transform playerTilt;
    public CharacterMovement characterMovement;


    float tilt_amount;
    float current_tilt;

    public float rotationSpeed;

    void Update()
    {


        // if (characterMovement.characterInput.GetMovementInput() != Vector3.zero)
        // {
        //     playerVisuals.forward = Vector3.Slerp(playerVisuals.forward, characterMovement.characterInput.GetMovementInput(), Time.deltaTime * rotationSpeed);
        // }
        // Quaternion old_rotation = transform.rotation;
        // Vector3 old_forward = transform.forward;
        // Vector3 horizontalVelocity = new Vector3(direction.x, 0, direction.z);
        // if (horizontalVelocity == Vector3.zero)
        // {
        //     tilt_amount = 0;
        //     return;
        // }
        // Quaternion forwardTarget = Quaternion.LookRotation(horizontalVelocity, Vector3.up);
        // if (forwardTarget == Quaternion.identity && horizontalVelocity.magnitude < 0.1f)
        // {
        //     tilt_amount = 0;
        //     return;
        // }
        // transform.rotation = Quaternion.Slerp(transform.rotation, forwardTarget, 0.1f);
        // int rot_direction = 1;
        // if (Vector3.Cross(old_forward, transform.forward).y > 0)
        // {
        //     rot_direction = -1;
        // }
        // tilt_amount = Quaternion.Angle(old_rotation, transform.rotation) * 3 * rot_direction;
    }

    void ApplyTilt()
    {
        current_tilt = Mathf.Lerp(current_tilt, tilt_amount, 0.1f);
        playerTilt.eulerAngles = new Vector3(playerTilt.eulerAngles.x, playerTilt.eulerAngles.y, current_tilt);
    }

}
