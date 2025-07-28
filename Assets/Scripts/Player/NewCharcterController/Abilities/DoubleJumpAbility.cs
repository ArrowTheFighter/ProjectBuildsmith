using System;
using UnityEngine;

public class DoubleJumpAbility : PlayerAbility
{
    float lastTimeDoubleJumped;
    int remainingDoubleJumps;
    int maxDoubleJumps = 1;
    bool JumpKeyHeld;

    void Start()
    {
        remainingDoubleJumps = maxDoubleJumps;
        characterMovement.characterInput.OnJump += DoubleJump;
    }

    public override void UpdateAbility()
    {
        float jumpKeyValue = characterMovement.characterInput.GetJumpInput()? 1: 0;
        if (!JumpKeyHeld)
        {
            if (jumpKeyValue > 0)
            {
                JumpKeyHeld = true;
                DoubleJump();
            }
        }
        else if (jumpKeyValue <= 0)
        {
            JumpKeyHeld = false;
        }
        if (characterMovement.grounded && !characterMovement.OnSteepSlope())
        {
            remainingDoubleJumps = maxDoubleJumps;
         }
        
    }

    public void DoubleJump()
    {
        if (!characterMovement.grounded && remainingDoubleJumps > 0 && Time.time > lastTimeDoubleJumped)
        {
            if (!characterMovement.readyToJump) return;
            if (characterMovement.MovementControlledByAbility) return;

            characterMovement.onDoubleJump?.Invoke();
            remainingDoubleJumps--;
            lastTimeDoubleJumped = Time.time + 0.2f;

            Vector3 velocity = characterMovement.rb.linearVelocity;
            velocity = new Vector3(velocity.x, 0, velocity.z);
            characterMovement.rb.linearVelocity = velocity;

            characterMovement.rb.AddForce(Vector3.up * (characterMovement.jumpForce - 3), ForceMode.Impulse);

            AudioCollection audioCollection = PlayerAudioManager.instance.GetAudioClipByID("Jump");
            SoundFXManager.instance.PlaySoundFXClip(audioCollection.audioClip, transform, audioCollection.audioClipVolume, UnityEngine.Random.Range(audioCollection.audioClipPitch * 0.9f, audioCollection.audioClipPitch * 1.1f));
        }
    }

    public override void FixedUpdateAbility()
    {

    }

    public override void ResetAbility()
    {
        //No need to reset anything
    }
}
