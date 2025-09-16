using UnityEngine;

public class PlayerStompAbility : PlayerAbility
{
    bool inAir;
    float playerStompCooldown;
    public override void FixedUpdateAbility()
    {
        return;
    }

    public override void ResetAbility()
    {
        return;
    }

    public override void UpdateAbility()
    {
        if (!characterMovement.grounded && !inAir)
        {
            inAir = true;
        }
        if (inAir && characterMovement.grounded)
        {
            inAir = false;
            if (playerStompCooldown > Time.time) return;
            Collider[] hits = Physics.OverlapSphere(transform.position + Vector3.down * 1.5f, characterMovement.playerRadius);
            foreach (Collider target in hits)
            {
                IDamagable damagable;
                if (target.TryGetComponent(out damagable))
                {
                    if (damagable.PlayerCanStomp)
                    {
                        damagable.TakeDamage(1, new AttackType[] { AttackType.Simple }, gameObject, out float extraForce);
                        {
                            print("Bouncing player");
                            characterMovement.playerAnimationController.PlayerDoubleJumped();
                            Vector3 vel = characterMovement.rb.linearVelocity;
                            characterMovement.rb.linearVelocity = new Vector3(vel.x, 0, vel.z);
                            //characterMovement.rb.linearVelocity = new Vector3(vel.x, 15 + extraForce, vel.z);
                            characterMovement.rb.AddForce(new Vector3(vel.x, 15 + extraForce, vel.z), ForceMode.Impulse);
                            playerStompCooldown = Time.time + 0.2f;
                        }
                    }
                }else if (target.transform.parent != null && target.transform.parent.TryGetComponent(out damagable))
                {
                    if (damagable.PlayerCanStomp)
                    {
                        damagable.TakeDamage(1, new AttackType[] { AttackType.Simple },gameObject, out float extraForce);
                        {
                            characterMovement.playerAnimationController.PlayerDoubleJumped();
                            Vector3 vel = characterMovement.rb.linearVelocity;
                            characterMovement.rb.linearVelocity = new Vector3(vel.x, 0, vel.z);
                            characterMovement.rb.AddForce(new Vector3(vel.x, 15 + extraForce, vel.z), ForceMode.Impulse);
                            //characterMovement.rb.linearVelocity = new Vector3(vel.x, 15 + extraForce, vel.z);
                            playerStompCooldown = Time.time + 0.2f;
                        }
                    }
                }
            }
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
