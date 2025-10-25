using System;
using UnityEngine;

public class DoubleJumpChopAbility : PlayerAbility
{
    public float upForce = 8;

    float lastTimeJumpedChopped;
    int remainingChopJumps;
    int maxChopJumps = 1;
    bool AbilityKeyHeld;
    Action OnAbility;

    void Start()
    {
        if (characterMovement.playerAnimationController != null)
        {
            OnAbility += characterMovement.playerAnimationController.PlayerAirChop;
        }
        remainingChopJumps = maxChopJumps;   
    }

    public override void UpdateAbility()
    {
        float AbilityKeyValue = ScriptRefrenceSingleton.instance.gameplayInput.playerInput.actions["BasicAttack"].ReadValue<float>();
        if (!AbilityKeyHeld)
        {
            if (AbilityKeyValue > 0)
            {
                AbilityKeyHeld = true;
                if (!characterMovement.grounded && remainingChopJumps > 0 && Time.time > lastTimeJumpedChopped)
                {
                    if (!characterMovement.readyToJump) return;
                    if (characterMovement.MovementControlledByAbility) return;

                    OnAbility?.Invoke();
                    remainingChopJumps--;
                    lastTimeJumpedChopped = Time.time + 0.2f;

                    Vector3 velocity = characterMovement.rb.linearVelocity;
                    velocity = new Vector3(velocity.x, 0, velocity.z);
                    characterMovement.rb.linearVelocity = velocity;

                    characterMovement.rb.AddForce(Vector3.up * upForce, ForceMode.Impulse);
                }
            }
        }
        else if (AbilityKeyValue <= 0)
        {
            AbilityKeyHeld = false;
        }
        if (characterMovement.grounded && !characterMovement.OnSteepSlope())
        {
            remainingChopJumps = maxChopJumps;
         }
        
    }

    public override void FixedUpdateAbility()
    {
        
    }

    public override void ResetAbility()
    {
        //No need to do anything
    }
}
