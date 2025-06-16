using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    CharacterMovement characterMovement;
    [SerializeField] Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        characterMovement = GetComponent<CharacterMovement>();
        characterMovement.OnJump += PlayerJumped;
    }

    // Update is called once per frame
    void Update()
    {
        float speed_blend = Mathf.InverseLerp(0, characterMovement.GetMaxSpeed(), characterMovement.GetVelocity().magnitude);
        animator.SetFloat("Speed_Blend", speed_blend);
        animator.SetBool("OnGround", characterMovement.GetOnGround());
    }

    void PlayerJumped()
    {
        animator.SetBool("Jumped", true);
        Invoke("reset_jump", 0.05f);
    }

    void reset_jump()
    {
        animator.SetBool("Jumped", false);
    }
}
