using UnityEngine;

public class LongFallReset : MonoBehaviour
{
    public LayerMask groundIgnore;
    public float rayDistance = 25;
    public float FallTime = 3;
    public float WorldBottom = 0;
    float timeOffGround;
    CharacterMovement characterMovement;
    public bool CanReset = true;
    public bool alwaysUseCheckpoints;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (TryGetComponent(out CharacterMovement component))
        {
            characterMovement = component;
            groundIgnore = characterMovement.IgnoreGroundLayerMask;
            characterMovement.onDoubleJump += ResetTime;
            characterMovement.OnDash += ResetTime;
        }
    }

    void ResetTime()
    {
        timeOffGround = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (characterMovement != null)
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
        }
        
        // if (transform.position.y < WorldBottom)
        // {
        //     ResetPosition();
        // }

        if (!CanReset) return;
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
            ResetPosition();
        }
    }

    void ResetPosition()
    {
        if (alwaysUseCheckpoints && TryGetComponent(out PlayerCheckpointPosition playerCheckpointPosition))
        {
            playerCheckpointPosition.SetPlayerToCheckpointPosition();
            timeOffGround = 0;
        }
        else if (TryGetComponent(out PlayerSafeZone playerSafeZone))
        {
            transform.position = playerSafeZone.safePos;
            timeOffGround = 0;

        }

        characterMovement.rb.linearVelocity = Vector3.zero;
        foreach (PlayerAbility ability in characterMovement.playerAbilities)
        {
            ability.ResetAbility();
        }
    }
}
