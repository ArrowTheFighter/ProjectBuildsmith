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
    bool groundLand;
    [HideInInspector] public bool MovingUp;
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
                    MovingUp = true;
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
                Vector3 pos = characterMovement.transform.position + Vector3.down * 1.5f + characterMovement.transform.forward;
                GameplayUtils.DrawDebugBox(pos, size, transform.rotation, Color.red);
                Collider[] hits = Physics.OverlapBox(pos, size * 0.5f, transform.rotation);
                foreach (Collider hit in hits)
                {
                    if (hit.TryGetComponent(out IDamagable damagable))
                    {
                        damagable.TakeDamage(DamageStrength,characterMovement.gameObject);


                        Vector3 velocity = characterMovement.rb.linearVelocity;
                        velocity = new Vector3(0, 0, 0);
                        characterMovement.rb.linearVelocity = velocity;
                        characterMovement.rb.AddForce(Vector3.up * bounceForce, ForceMode.Impulse);
                        StopFall();
                    }
                }
            }
            

            if (characterMovement.grounded && !groundLand)
            {
                Vector3 pos = characterMovement.transform.position + Vector3.down;
                Collider[] hits = Physics.OverlapSphere(pos,2);
                foreach (Collider collider in hits)
                {
                    if (collider.TryGetComponent(out IDamagable damagable))
                    {
                        damagable.TakeDamage(1, characterMovement.gameObject);
                     }
                 }

                print("landed on ground");
                groundLand = true;
                characterMovement.playerAnimationController.animator.SetBool("ChopFall", false);
                characterMovement.playerAnimationController.animator.SetBool("ChopLandGround", true);
                Invoke("StopFall", 0.5f);
                //StopFall();
            }
         }
        
    }

    public void StopFall()
    {
        characterMovement.MovementControlledByAbility = false;
        AbilityActive = false;
        IsFalling = false;
        groundLand = false;
        characterMovement.playerAnimationController.animator.SetBool("ChopFall", false);
        characterMovement.playerAnimationController.animator.SetBool("ChopLandGround", false);
    }

    IEnumerator DownForceDelay()
    {
        yield return new WaitForSecondsRealtime(downDelay);

        Vector3 velocity = new Vector3(0, 0, 0);
        characterMovement.rb.linearVelocity = velocity;

        characterMovement.rb.AddForce(Vector3.down * downForce, ForceMode.Impulse);
        characterMovement.playerAnimationController.animator.SetBool("ChopFall", true);
        IsFalling = true;
        MovingUp = false;
    }


    public override void FixedUpdateAbility()
    {

    }
}
