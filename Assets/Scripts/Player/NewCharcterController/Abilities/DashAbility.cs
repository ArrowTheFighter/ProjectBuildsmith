using System.Collections;
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
    [SerializeField] float slideDuration = 0.25f;
    [SerializeField] float slideJumpDuration = 0.35f;
    [SerializeField] float slideTurnAmount = 90f;
    [SerializeField] float maxFallSpeed = 50;

    public override void Initialize(CharacterMovement player)
    {
        base.Initialize(player);
        characterMovement.onDoubleJump += setCooldown;
        characterMovement.characterInput.OnDive += SlideJump;
        characterMovement.characterInput.OnDive += DashAction;
    }

    public override void FixedUpdateAbility()
    {
        if (isDashing)
        {
            //characterMovement.rb.linearDamping = 0;
            characterMovement.tilt_amount = 0;
            characterMovement.ApplyGravity();
            
            characterMovement.rb.AddForce(Vector3.down * extraGravity);

            if (characterMovement.rb.linearVelocity.y < -maxFallSpeed)
            {
                Vector3 characterVel = characterMovement.rb.linearVelocity;
                characterVel.y = -maxFallSpeed;
                characterMovement.rb.linearVelocity = characterVel;
            }
            //Vector3 flatVel = new Vector3(characterMovement.rb.linearVelocity.x, 0f, characterMovement.rb.linearVelocity.z);

            // limit velocity if needed
            // if (flatVel.magnitude > dashMaxSpeed)
            // {
            //     Vector3 limitedVel = flatVel.normalized * dashMaxSpeed;
            //     characterMovement.rb.linearVelocity = new Vector3(limitedVel.x, characterMovement.rb.linearVelocity.y, limitedVel.z);
            // }
            LayerMask layerMask = characterMovement.IgnoreGroundLayerMask;
            RaycastHit raycastHit;
            if (Physics.Raycast(
                characterMovement.transform.position + characterMovement.transform.forward * 0.5f,
                characterMovement.orientation.forward,
                out raycastHit,
                1.5f, ~layerMask, QueryTriggerInteraction.Ignore))
            {
                float maxSlopeDot = Mathf.Cos(characterMovement.maxSlopeAngle * Mathf.Deg2Rad);
                if (Vector3.Dot(raycastHit.normal, Vector3.up) < maxSlopeDot)
                {
                    if (!isBonking && !characterMovement.grounded && !slideJumping)
                    {
                        Bonk();
                    }
                }
                    
            }

            //print("character Grounded = " + characterMovement.grounded + " - OnSteepSlope = " + characterMovement.OnSteepSlope());
            if (characterMovement.grounded || characterMovement.OnSteepSlope())
            {
                //print("Groundsliding = " + groundSliding + " - SlideJumping = " + slideJumping);
                if (!groundSliding && Time.time > lastTimeDashed)
                {
                    //StartCoroutine(SlideCooldown());
                    groundSliding = true;

                    characterMovement.rb.linearDamping = 2;
                }
                if (groundSliding && !slideJumping)
                {
                    //print(characterMovement.rb.linearVelocity.magnitude);
                    Vector3 HorVel = new Vector3(characterMovement.rb.linearVelocity.x, 0, characterMovement.rb.linearVelocity.z);
                    Vector3 platformVel = new Vector3(characterMovement.platformDelta.x, 0, characterMovement.platformDelta.z);
                    Vector3 adjustedVel = HorVel - platformVel;
                    if (adjustedVel.magnitude < 5f && !slideJumping)
                    {
                        if (Time.time - lastTimeDashed > 0.75f)
                        {
                            StopAllCoroutines();
                            EndDive();
                        }
                    }
                    if (characterMovement.characterInput is NPCFollowTargetInput)
                    {
                        NPCFollowTargetInput followTargetInput = (NPCFollowTargetInput)characterMovement.characterInput;
                        if (followTargetInput.ForceSliding)
                        {
                            print("forceSliding");
                            characterMovement.rb.linearDamping = 0;
                        }
                        else
                        {
                            print("not force sliding");
                            characterMovement.rb.linearDamping = 5;
                        }
                     }
                    else if (characterMovement.characterInput.GetMovementInput() != Vector3.zero)
                    {
                        Vector3 camForward = Camera.main.transform.forward;
                        camForward.y = 0;

                        float dot = Vector3.Dot(characterMovement.characterInput.GetMovementInput(), characterMovement.transform.forward);
                        if ( dot < -0.4f)
                        {
                            characterMovement.rb.linearVelocity = Vector3.Lerp(characterMovement.rb.linearVelocity, Vector3.zero, 0.1f);
                        }
                        if(characterMovement.characterInput.GetDashInput())
                        // (dot > 0.4f)
                        {
                            characterMovement.rb.linearDamping = .4f;
                        }
                        else
                        {
                            characterMovement.rb.linearDamping = 3;
                        }
                        if (characterMovement.rb.linearVelocity.magnitude > 0.01)
                        {
                            float turnAmount = 0;
                            float dotRight = Vector3.Dot(characterMovement.characterInput.GetMovementInput(), characterMovement.transform.right);
                            if (dotRight > 0.1f)
                            {
                                turnAmount = slideTurnAmount;
                            }
                            else if (dotRight < -0.1f)
                            {
                                turnAmount = -slideTurnAmount;
                            }
                            if (turnAmount != 0)
                            {
                                Quaternion turn = Quaternion.AngleAxis(turnAmount * Time.fixedDeltaTime, Vector3.up);
                                Vector3 newVelocity = turn * characterMovement.rb.linearVelocity;
                                characterMovement.rb.linearVelocity = newVelocity;
                            }
                        }
                    }
                    if (characterMovement.rb.linearVelocity.magnitude > 0.1f && !isBonking)
                    {
                        characterMovement.orientation.forward = Vector3.Lerp(characterMovement.orientation.forward, characterMovement.rb.linearVelocity.normalized, 0.2f);
                    }


                }

                if (slideJumping && Time.time > lastTimeDashed)
                {
                    EndDive();
                }
                if (isBonking)
                {
                    characterMovement.rb.linearDamping = 5;
                }
                if (isBonking && !bonkLand && Time.time > lastTimeDashed)
                {
                    bonkLand = true;
                    StartCoroutine(BonkCooldown());
                }
            }
            else
            {
                if (isBonking)
                {
                    characterMovement.rb.linearDamping = 0;
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
        lastTimeDashed = Time.time + 0.1f;
        characterMovement.playerAnimationController.animator.SetBool("Bonking", true);

        AudioCollection audioCollection = PlayerAudioManager.instance.GetAudioClipByID("Bonk");
        SoundFXManager.instance.PlaySoundFXClip(audioCollection.audioClip, transform, audioCollection.audioClipVolume, UnityEngine.Random.Range(audioCollection.audioClipPitch * 0.9f, audioCollection.audioClipPitch * 1.1f));
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
            lastTimeDashed = Time.time + 0.1f;
            StartCoroutine(EndDiveAfterDelay(slideJumpDuration));

            AudioCollection audioCollection = PlayerAudioManager.instance.GetAudioClipByID("Dash");
            SoundFXManager.instance.PlaySoundFXClip(audioCollection.audioClip, transform, audioCollection.audioClipVolume, UnityEngine.Random.Range(audioCollection.audioClipPitch * 0.9f, audioCollection.audioClipPitch * 1.1f));
        }
    }

    IEnumerator EndDiveAfterDelay(float duration)
    {
        yield return new WaitForSeconds(duration);
        EndDive();
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

       // Vector3 characterForward = characterMovement.orientation.forward;
        //characterForward.y = 0;
        characterMovement.orientation.localEulerAngles = Vector3.zero;

        groundSliding = false;
        slideJumping = false;
        bonkLand = false;
        isBonking = false;
        StartCoroutine(EndDelay());
        StopCoroutine(BonkCooldown());
        StopCoroutine(SlideCooldown());
        StopCoroutine(EndDiveAfterDelay(slideJumpDuration));
    }

    IEnumerator EndDelay()
    {
        yield return null;
        characterMovement.playerAnimationController.animator.ResetTrigger("Jump");
        characterMovement.orientation.localEulerAngles = Vector3.zero;
    }


    public override void UpdateAbility()
    {
        if (!canDash && characterMovement.grounded && !characterMovement.OnSteepSlope())
        {
            canDash = true;
        }
        float dashInputValue = characterMovement.characterInput.GetDashInput() ? 1 : 0;
        if (dashInputValue > 0.1f)
        {
            if (!dashButtonPressed && !isDashing && Time.time > dontDash && canDash && characterMovement.readyToJump)
            {
                if (!characterMovement.MovementControlledByAbility)
                {
                    Dash();
                    dashButtonPressed = true;
                }
                else
                {

                    foreach (PlayerAbility playerAbility in characterMovement.playerAbilities)
                    {
                        switch (playerAbility)
                        {
                            case ChopSlamAbility slamAbility:
                                if (slamAbility.MovingUp)
                                {
                                    slamAbility.StopAllCoroutines();
                                    slamAbility.StopFall();
                                    Dash();
                                    dashButtonPressed = true;
                                }
                                break;
                        }
                    }
                }
            }
        }
        else if(dashButtonPressed)
        {
            dashButtonPressed = false;
         }
    }

    void DashAction()
    {
        if (!isDashing && Time.time > dontDash && canDash && characterMovement.readyToJump)
        {
            if (!characterMovement.MovementControlledByAbility)
            {
                Dash();
                dashButtonPressed = true;
            }
        }
        else
        {

            foreach (PlayerAbility playerAbility in characterMovement.playerAbilities)
            {
                switch (playerAbility)
                {
                    case ChopSlamAbility slamAbility:
                        if (slamAbility.MovingUp)
                        {
                            slamAbility.StopAllCoroutines();
                            slamAbility.StopFall();
                            Dash();
                            dashButtonPressed = true;
                        }
                        break;
                }
            }
        }
    }

    void Dash()
    {
        if (characterMovement.MovementControlledByAbility) return;
        characterMovement.OnDash?.Invoke();
        lastTimeDashed = Time.time + 0.1f;
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

        AudioCollection audioCollection = PlayerAudioManager.instance.GetAudioClipByID("Dash");
        SoundFXManager.instance.PlaySoundFXClip(audioCollection.audioClip, transform, audioCollection.audioClipVolume, UnityEngine.Random.Range(audioCollection.audioClipPitch * 0.9f, audioCollection.audioClipPitch * 1.1f));
    }

    void setCooldown()
    {
        dontDash = Time.time + 0.1f;
     }

    public override void ResetAbility()
    {
        if (isDashing)
        {
            StopAllCoroutines();
            EndDive();
         }
    }
}
