using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class NoClip : PlayerAbility
{
    [SerializeField] float no_clip_speed = 20;
    bool NoClipActive;
    bool NoClipButtonPressed;
    public override void FixedUpdateAbility()
    {

    }

    public override void ResetAbility()
    {
        //no need to do anything
    }

    public override void UpdateAbility()
    {
        if (GameplayInput.instance.playerInput.actions["NoClip"].ReadValue<float>() > 0)
        {
            if (!NoClipButtonPressed)
            {
                NoClipActive = !NoClipActive;
                characterMovement.MovementControlledByAbility = NoClipActive;
                NoClipButtonPressed = true;
                if (NoClipActive)
                {
                    foreach (PlayerAbility ability in characterMovement.playerAbilities)
                    {
                        ability.ResetAbility();
                     }
                 }
            }
        }
        else
        {
            NoClipButtonPressed = false;
        }

        if (NoClipActive)
        {
            characterMovement.rb.linearVelocity = Vector3.zero;
            
            PlayerInput playerInput = GameplayInput.instance.playerInput;
            Vector2 playerMoveInput = playerInput.actions["Move"].ReadValue<Vector2>();
            Vector3 cam_f = Camera.main.transform.forward.normalized * playerMoveInput.y;
            Vector3 cam_r = Camera.main.transform.right.normalized * playerMoveInput.x;

            if (playerInput.actions["Sprint"].ReadValue<float>() <= 0)
            {
                characterMovement.transform.position += (cam_f + cam_r) * no_clip_speed * Time.deltaTime;
                characterMovement.transform.position += new Vector3(0, playerInput.actions["Jump"].ReadValue<float>(), 0) * no_clip_speed * Time.deltaTime;
            }
            else
            {
                characterMovement.transform.position += (cam_f + cam_r) * no_clip_speed * 10 * Time.deltaTime;
                characterMovement.transform.position += new Vector3(0, playerInput.actions["Jump"].ReadValue<float>(), 0) * no_clip_speed * 10 * Time.deltaTime;
            }
        }
    }

    

}
