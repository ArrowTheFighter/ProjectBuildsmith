using UnityEngine;

public class BlobAnimationController : MonoBehaviour
{
    CharacterMovement characterMovement;
    public Animator animator;

    void Start()
    {
        characterMovement = GetComponent<CharacterMovement>();
    }

    void FixedUpdate()
    {
        Vector3 HorVel = new Vector3(characterMovement.rb.linearVelocity.x, 0, characterMovement.rb.linearVelocity.z);
        Vector3 platformVel = new Vector3(characterMovement.platformDelta.x, 0, characterMovement.platformDelta.z);
        Vector3 adjustedVel = HorVel - platformVel;
        float speed_blend = Mathf.InverseLerp(0, characterMovement.maxSpeed, adjustedVel.magnitude);
        animator.SetFloat("Speed_Blend", speed_blend);

    }
}