using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    CharacterMovement characterMovement;
    [SerializeField] Animator animator;
    bool diving;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        characterMovement = GetComponent<CharacterMovement>();
        characterMovement.OnJump += PlayerJumped;
        characterMovement.onDoubleJump += PlayerDoubleJumped;
        characterMovement.OnDash += PlayerDived;
        characterMovement.OnDashStop += PlayerStopedDive;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 HorVel = new Vector3(characterMovement.rb.linearVelocity.x, 0, characterMovement.rb.linearVelocity.z);
        float speed_blend = Mathf.InverseLerp(0, characterMovement.maxSpeed, HorVel.magnitude);
        animator.SetFloat("Speed_Blend", speed_blend);
        animator.SetBool("OnGround", characterMovement.grounded && !characterMovement.OnSteepSlope());


    }


    void PlayerDived()
    {
        print("setting dive to true");
        animator.SetBool("Diving", true);
    }

    void PlayerStopedDive()
    {
        animator.SetBool("Diving", false);

    }

    void PlayerJumped()
    {
        animator.SetBool("Jumped", true);
        Invoke("reset_jump", 0.05f);
    }

    void PlayerDoubleJumped()
    {
        animator.SetTrigger("DoubleJump");
    }

    void reset_jump()
    {
        animator.SetBool("Jumped", false);
    }
}
