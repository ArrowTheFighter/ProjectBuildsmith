using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    CharacterMovement characterMovement;
    [SerializeField] public Animator animator;
    bool diving;
    [HideInInspector] public bool walking = true;
    bool currentlyOnGround;
    float offGroundTime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        characterMovement = GetComponent<CharacterMovement>();
        characterMovement.OnJump += PlayerJumped;
        characterMovement.onDoubleJump += PlayerDoubleJumped;
        characterMovement.OnDash += PlayerDived;
        characterMovement.OnDashStop += PlayerStopedDive;
        characterMovement.OnBasicAttack += PlayerBasicAttacked;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 HorVel = new Vector3(characterMovement.rb.linearVelocity.x, 0, characterMovement.rb.linearVelocity.z);
        float speed_blend = Mathf.InverseLerp(0, characterMovement.maxSpeed, HorVel.magnitude);
        animator.SetFloat("Speed_Blend", speed_blend);
        animator.SetBool("OnGround", characterMovement.grounded && !characterMovement.OnSteepSlope());
        animator.SetFloat("OffGroundTime", offGroundTime);
        if (characterMovement.grounded && !currentlyOnGround)
        {
            currentlyOnGround = true;
            offGroundTime = 0;
        }
        if (currentlyOnGround && !characterMovement.grounded)
        {
            currentlyOnGround = false;
        }
        if (!currentlyOnGround)
        {
            offGroundTime += Time.deltaTime;
        }
        if (characterMovement.grounded && walking) animator.SetLayerWeight(1, 1);
        else animator.SetLayerWeight(1, 0);

    }


    void PlayerDived()
    {
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

    public void PlayerDoubleJumped()
    {
        animator.SetTrigger("DoubleJump");
    }

    void PlayerBasicAttacked()
    {
        animator.SetTrigger("BasicAttack");
     }

    public void PlayerAirChop()
    {
        animator.SetTrigger("DoubleJumpChop");
    }

    public void PlayerChopSlamStart()
    {
        animator.SetTrigger("ChopSlam");
     }

    void reset_jump()
    {
        animator.SetBool("Jumped", false);
    }
}
