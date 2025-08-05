using UnityEngine;

public class LongFallReset : MonoBehaviour
{
    public LayerMask groundIgnore;
    public float rayDistance = 25;
    public float FallTime = 3;
    float timeOffGround;
    CharacterMovement characterMovement;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        characterMovement = GetComponent<CharacterMovement>();
        groundIgnore = characterMovement.IgnoreGroundLayerMask;
        characterMovement.onDoubleJump += ResetTime;
        characterMovement.OnDash += ResetTime;
    }

    void ResetTime()
    {
        timeOffGround = 0;
     }

    // Update is called once per frame
    void FixedUpdate()
    {
        foreach (PlayerAbility playerAbility in characterMovement.playerAbilities)
        {
            switch (playerAbility)
            {
                case NoClip noClip:
                    if (noClip.NoClipActive) return;
                    break;
             }
         }
        if (!Physics.Raycast(transform.position, Vector3.down, rayDistance, ~groundIgnore))
        {
            timeOffGround += Time.deltaTime;
        }
        else
        {
            timeOffGround = 0;
        }
        if (timeOffGround > FallTime)
        {
            if (TryGetComponent(out PlayerSafeZone playerSafeZone))
            {
                print("moving player to safe position");
                transform.position = playerSafeZone.safePos;
                timeOffGround = 0;
                
                characterMovement.rb.linearVelocity = Vector3.zero;
                foreach (PlayerAbility ability in characterMovement.playerAbilities)
                {
                    ability.ResetAbility();
                }
             }
         }
    }
}
