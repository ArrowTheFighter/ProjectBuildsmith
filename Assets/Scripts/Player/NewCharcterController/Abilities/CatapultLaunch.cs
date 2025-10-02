
using UnityEngine;

public class CatapultLaunch : PlayerAbility
{
    public float initalCooldown;
    public Vector3 initalVelocity;

    public override void Initialize(CharacterMovement player)
    {
        base.Initialize(player);
        initalCooldown = Time.time + 0.2f;
        characterMovement.MovementControlledByAbility = true;
        characterMovement.rb.linearDamping = 0;
        if (TryGetComponent(out LongFallReset longFallReset))
        {
            longFallReset.CanReset = false;
        }
        characterMovement.playerAnimationController.animator.CrossFade("CatapultRoll", 0.1f);
        PlayerParticlesManager.instance.PlayParticleByID("SpeedLines");
        //initalVelocity = characterMovement.rb.linearVelocity;
    }

    public override void UpdateAbility()
    {
        characterMovement.ApplyGravity(0.25f);


        if (Time.time > initalCooldown && characterMovement.grounded)
        {
            if (TryGetComponent(out LongFallReset longFallReset))
            {
                longFallReset.CanReset = true;
            }
            characterMovement.MovementControlledByAbility = false;
            characterMovement.RemoveAbility<CatapultLaunch>();


            PlayerParticlesManager.instance.GetParticleByID("SpeedLines").Stop(false,ParticleSystemStopBehavior.StopEmittingAndClear);

            characterMovement.playerAnimationController.animator.CrossFade("WalkingBlend", 0.1f);
        }
    }


    public override void FixedUpdateAbility()
    {
        if (characterMovement.characterInput.GetMovementInput() != Vector3.zero)
        {
            Vector3 camForward = Camera.main.transform.forward;
            camForward.y = 0;

            Vector3 horVel = new Vector3(characterMovement.rb.linearVelocity.x, 0, characterMovement.rb.linearVelocity.z);

            float dot = Vector3.Dot(characterMovement.characterInput.GetMovementInput(), horVel.normalized);
            if (dot < -0.4f)
            {
                // Vector3 velocity = characterMovement.rb.linearVelocity;
                // velocity.x *= 0.995f;
                // velocity.z *= 0.995f;
                // characterMovement.rb.linearVelocity = velocity;

                Vector3 Horizontalvelocity = horVel;
                Vector3 initalHorVel = new Vector3(initalVelocity.x, 0, initalVelocity.z);
                Horizontalvelocity = Vector3.Lerp(Horizontalvelocity, initalHorVel * 0.65f, 0.05f);
                Horizontalvelocity.y += characterMovement.rb.linearVelocity.y;
                characterMovement.rb.linearVelocity = Horizontalvelocity;

            }
            else if (dot > 0.4f)
            {
                Vector3 Horizontalvelocity = horVel;
                Vector3 initalHorVel = new Vector3(initalVelocity.x, 0, initalVelocity.z);
                Horizontalvelocity = Vector3.Lerp(Horizontalvelocity, initalHorVel * 1.35f, 0.05f);
                Horizontalvelocity.y += characterMovement.rb.linearVelocity.y;
                characterMovement.rb.linearVelocity = Horizontalvelocity;
            }

            Vector3 rightDir = Vector3.Cross(Vector3.up, horVel.normalized);
            float rightDot = Vector3.Dot(characterMovement.characterInput.GetMovementInput(), rightDir.normalized);
            print(rightDot);
            if (Mathf.Abs(rightDot) > 0.4f)
            {
                characterMovement.rb.AddForce(rightDir.normalized * rightDot * 200 * Time.fixedDeltaTime);
            }



        }
    }

    public void SetInitalVelocity()
    {
        initalVelocity = characterMovement.rb.linearVelocity;
        Vector3 horVel = new Vector3(characterMovement.rb.linearVelocity.x, 0, characterMovement.rb.linearVelocity.z);
        characterMovement.transform.forward = horVel.normalized;
    }

    public override void ResetAbility()
    {
        return;
    }
}
