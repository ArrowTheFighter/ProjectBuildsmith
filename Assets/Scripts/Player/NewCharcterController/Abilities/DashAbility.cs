using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class DashAbility : PlayerAbility
{
    bool canDash;
    bool isDashing;
    bool groundSliding;
    bool slideJumping;
    bool isBonking;
    bool bonkLand;
    Vector3 dashDirection;
    float lastTimeDashed;
    float dontDash;
    bool dashButtonPressed;
    [SerializeField] float dashForce = 20;
    [SerializeField] float dashUpForce = 25;
    [SerializeField] float slideJumpForce = 22;
    [SerializeField] float slideJumpUpForce = 22;
    [SerializeField] float bonkForce = 12;
    [SerializeField] float bonkUpForce = 12;
    [SerializeField] float bonkEndDelay = .5f;
    [SerializeField] float dashUpForceFromGround = 25;
    //[SerializeField] float dashMaxSpeed = 15;
    [SerializeField] float extraGravity = 40;
    [SerializeField] float slideDuration = 0.5f;

    public override void Initialize(CharacterMovement player)
    {
        base.Initialize(player);
        characterMovement.onDoubleJump += setCooldown;
        characterMovement.characterInput.OnJump += SlideJump;
    }

    public override void FixedUpdateAbility()
    {
        if (isDashing)
        {
            //characterMovement.rb.linearDamping = 0;
            characterMovement.tilt_amount = 0;
            characterMovement.ApplyGravity();
            
            characterMovement.rb.AddForce(Vector3.down * extraGravity);
            //Vector3 flatVel = new Vector3(characterMovement.rb.linearVelocity.x, 0f, characterMovement.rb.linearVelocity.z);

            // limit velocity if needed
            // if (flatVel.magnitude > dashMaxSpeed)
            // {
            //     Vector3 limitedVel = flatVel.normalized * dashMaxSpeed;
            //     characterMovement.rb.linearVelocity = new Vector3(limitedVel.x, characterMovement.rb.linearVelocity.y, limitedVel.z);
            // }
            LayerMask layerMask = characterMovement.IgnoreGroundLayerMask;
            if (Physics.Raycast(
                characterMovement.transform.position + characterMovement.transform.forward * 0.5f,
                characterMovement.orientation.forward,
                1.5f, ~layerMask,QueryTriggerInteraction.Ignore))
            {
                if (!isBonking && !characterMovement.grounded)
                {
                    Bonk();
                 }
             }



            if (characterMovement.grounded || characterMovement.OnSteepSlope())
            {
                if (!groundSliding && Time.time > lastTimeDashed)
                {
                    StartCoroutine(SlideCooldown());
                    groundSliding = true;

                    characterMovement.rb.linearDamping = 2;
                }
                if (groundSliding && characterMovement.rb.linearVelocity.magnitude < 0.2f && !slideJumping)
                {
                    print("player wasn't moving");
                    StopAllCoroutines();
                    EndDive();
                }
                if (slideJumping && Time.time > lastTimeDashed)
                {
                    print("ending slide jump");
                    EndDive();
                }
                if (isBonking && !bonkLand && Time.time > lastTimeDashed)
                {
                    print("Bonking player is on the ground");
                    bonkLand = true;
                    StartCoroutine(BonkCooldown());
                 }
            }
        }
    }

    void Bonk()
    {
        characterMovement.rb.linearDamping = 1;
        characterMovement.OnDashStop?.Invoke();
        characterMovement.rb.linearVelocity = Vector3.zero;

        characterMovement.rb.AddForce(-characterMovement.transform.forward * bonkForce, ForceMode.Impulse);
        characterMovement.rb.AddForce(Vector3.up * bonkUpForce, ForceMode.Impulse);
        isBonking = true;
        lastTimeDashed = Time.time + 0.2f;
        characterMovement.playerAnimationController.animator.SetBool("Bonking", true);
    }

    void SlideJump()
    {
        if (groundSliding && !isBonking && !slideJumping)
        {
            characterMovement.rb.linearDamping = 1;
            characterMovement.OnDashStop?.Invoke();
            characterMovement.playerAnimationController.animator.SetTrigger("Jump");
            characterMovement.rb.linearVelocity = Vector3.zero;
            Vector3 direction = Vector3.ProjectOnPlane(characterMovement.orientation.forward, characterMovement.GravityDir);

            characterMovement.rb.AddForce(direction * slideJumpForce, ForceMode.Impulse);
            characterMovement.rb.AddForce(Vector3.up * slideJumpUpForce, ForceMode.Impulse);
            StopAllCoroutines();
            slideJumping = true;
            lastTimeDashed = Time.time + 0.2f;
        }
    }

    IEnumerator BonkCooldown()
    {
        yield return new WaitForSeconds(bonkEndDelay);
        EndDive();
     }

    IEnumerator SlideCooldown()
    {
        yield return new WaitForSeconds(slideDuration);
        EndDive();
    }

    void EndDive()
    {
        characterMovement.playerAnimationController.walking = true;
        isDashing = false;
        characterMovement.OnDashStop?.Invoke();
        characterMovement.playerAnimationController.animator.SetBool("Bonking", false);
        characterMovement.MovementControlledByAbility = false;
        groundSliding = false;
        slideJumping = false;
        bonkLand = false;
        isBonking = false;
        StartCoroutine(EndDelay());
        StopCoroutine(BonkCooldown());
        StopCoroutine(SlideCooldown());
    }

    IEnumerator EndDelay()
    {
        yield return null;
        characterMovement.playerAnimationController.animator.ResetTrigger("Jump");
    }


    public override void UpdateAbility()
    {
        if (!canDash && characterMovement.grounded && !characterMovement.OnSteepSlope())
        {
            canDash = true;
        }
        float dashInputValue = characterMovement.characterInput.GetDashInput() ? 1 : 0;

        if (dashInputValue > 0)
        {
            if (!dashButtonPressed && !isDashing && Time.time > dontDash && canDash && characterMovement.readyToJump)
            {
                Dash();
                dashButtonPressed = true;
            }
        }
        else
        {
            dashButtonPressed = false;
         }
    }

    void Dash()
    {

        characterMovement.OnDash?.Invoke();
        lastTimeDashed = Time.time + 0.5f;
        dashDirection = Vector3.ProjectOnPlane(characterMovement.orientation.forward, characterMovement.GravityDir);
        isDashing = true;
        characterMovement.MovementControlledByAbility = true;
        canDash = false;

        characterMovement.playerAnimationController.walking = false;

        characterMovement.rb.linearDamping = 0;
        Vector3 velocity = Vector3.zero;
        velocity.y = 0;
        characterMovement.rb.linearVelocity = velocity;

        characterMovement.rb.AddForce(dashDirection * dashForce, ForceMode.Impulse);
        if (characterMovement.grounded && !characterMovement.OnSteepSlope())
        {
            characterMovement.rb.AddForce(Vector3.up * dashUpForceFromGround, ForceMode.Impulse);
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
