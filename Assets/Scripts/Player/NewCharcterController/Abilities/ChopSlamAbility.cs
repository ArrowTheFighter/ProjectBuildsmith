using System;
using System.Collections;
using UnityEngine;

public class ChopSlamAbility : PlayerAbility
{
    public int DamageStrength = 3;

    public float upForce = 20;
    public float bounceForce = 12;
    public float downForce = 25;
    public float downDelay = 0.375f;

    float lastTimeJumpedChopped;
    bool AbilityKeyHeld;

    bool AbilityActive;
    bool IsFalling;
    Action OnAbility;

    void Start()
    {
        if (characterMovement.playerAnimationController != null)
        {
            OnAbility += characterMovement.playerAnimationController.PlayerChopSlamStart;
        }
    }

    public override void UpdateAbility()
    {
        float AbilityKeyValue = GameplayInput.instance.playerInput.actions["BasicAttack"].ReadValue<float>();
        if (!AbilityKeyHeld)
        {
            if (AbilityKeyValue > 0)
            {
                AbilityKeyHeld = true;
                if (!characterMovement.grounded && Time.time > lastTimeJumpedChopped)
                {
                    if (characterMovement.MovementControlledByAbility) return;

                    OnAbility?.Invoke();
                    lastTimeJumpedChopped = Time.time + 0.2f;

                    Vector3 velocity = characterMovement.rb.linearVelocity;
                    velocity = new Vector3(0, 0, 0);
                    characterMovement.rb.linearVelocity = velocity;

                    characterMovement.tilt_amount = 0;
                    characterMovement.rb.AddForce(Vector3.up * upForce, ForceMode.Impulse);
                    characterMovement.MovementControlledByAbility = true;
                    AbilityActive = true;
                    StartCoroutine(DownForceDelay());
                }
            }
        }
        else if (AbilityKeyValue <= 0)
        {
            AbilityKeyHeld = false;
        }
        if (AbilityActive)
        {
            characterMovement.ApplyGravity();
            if (IsFalling)
            {
                Vector3 size = new Vector3(1, 0.5f, 2);
                Collider[] hits = Physics.OverlapBox(transform.position + Vector3.down * 1.5f, size * 0.5f, transform.rotation);
                foreach (Collider hit in hits)
                {
                    if (hit.TryGetComponent(out IDamagable damagable))
                    {
                        damagable.TakeDamage(DamageStrength);


                        Vector3 velocity = characterMovement.rb.linearVelocity;
                        velocity = new Vector3(0, 0, 0);
                        characterMovement.rb.linearVelocity = velocity;
                        characterMovement.rb.AddForce(Vector3.up * bounceForce, ForceMode.Impulse);
                        StopFall();
                    }
                }
            }

            if (characterMovement.grounded)
                {
                    StopFall();
                }
         }
        
    }

    void StopFall()
    {
        characterMovement.MovementControlledByAbility = false;
        AbilityActive = false;
        IsFalling = false;
        characterMovement.playerAnimationController.animator.SetBool("ChopFall", false);
    }

    IEnumerator DownForceDelay()
    {
        yield return new WaitForSecondsRealtime(downDelay);

        Vector3 velocity = new Vector3(0, 0, 0);
        characterMovement.rb.linearVelocity = velocity;

        characterMovement.rb.AddForce(Vector3.down * downForce, ForceMode.Impulse);
        characterMovement.playerAnimationController.animator.SetBool("ChopFall", true);
        IsFalling = true;
    }


    public override void FixedUpdateAbility()
    {

    }
}
