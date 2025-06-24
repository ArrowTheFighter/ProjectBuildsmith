using UnityEngine;

public class DashAbility : PlayerAbility
{
    bool canDash;
    bool dashPressed;
    bool isDashing;
    Vector3 dashDirection;
    float lastTimeDashed;
    float dontDash;
    [SerializeField] float dashForce = 225;
    [SerializeField] float dashUpForce = 25;
    [SerializeField] float dashUpForceFromGround = 25;
    [SerializeField] float dashMaxSpeed = 15;
    [SerializeField] float extraGravity = 40;

    public override void Initialize(CharacterMovement player)
    {
        base.Initialize(player);
        characterMovement.onDoubleJump += setCooldown;
    }

    public override void FixedUpdateAbility()
    {
        if (isDashing)
        {
            characterMovement.rb.linearDamping = 0;
            characterMovement.tilt_amount = 0;
            characterMovement.ApplyGravity();
            characterMovement.rb.AddForce(dashDirection * dashForce);
            characterMovement.rb.AddForce(Vector3.down * extraGravity);
            Vector3 flatVel = new Vector3(characterMovement.rb.linearVelocity.x, 0f, characterMovement.rb.linearVelocity.z);

            // limit velocity if needed
            if (flatVel.magnitude > dashMaxSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * dashMaxSpeed;
                characterMovement.rb.linearVelocity = new Vector3(limitedVel.x, characterMovement.rb.linearVelocity.y, limitedVel.z);
            }

            if (characterMovement.grounded || characterMovement.OnSteepSlope())
            {
                if (Time.time > lastTimeDashed)
                {
                    isDashing = false;
                    characterMovement.OnDashStop?.Invoke();
                    characterMovement.MovementControlledByAbility = false;
                }
            }
        }
    }

    public override void UpdateAbility()
    {
        if (!canDash && characterMovement.grounded && !characterMovement.OnSteepSlope())
        {
            canDash = true;
         }
        float dashInputValue = characterMovement.characterInput.GetDashInput()? 1:0;
        
        if (dashInputValue > 0 && !isDashing && Time.time > dontDash && canDash)
        {
            Dash();
        }
        else if (dashInputValue <= 0)
        {
            dashPressed = false;
        }
    }

    void Dash()
    {
        characterMovement.OnDash?.Invoke();
        lastTimeDashed = Time.time + 0.2f;
        dashDirection = characterMovement.orientation.forward;
        isDashing = true;
        characterMovement.MovementControlledByAbility = true;
        dashPressed = true;
        canDash = false;

        Vector3 velocity = characterMovement.rb.linearVelocity;
        velocity.y = 0;
        characterMovement.rb.linearVelocity = velocity;
        if (characterMovement.grounded && !characterMovement.OnSteepSlope())
        {
            characterMovement.rb.AddForce(-characterMovement.GravityDir * dashUpForceFromGround, ForceMode.Impulse);
        }
        else
        {
            characterMovement.rb.AddForce(Vector3.up * dashUpForce, ForceMode.Impulse);
        }
    }

    void setCooldown()
    {
        dontDash = Time.time + 0.1f;
     }
    
}
