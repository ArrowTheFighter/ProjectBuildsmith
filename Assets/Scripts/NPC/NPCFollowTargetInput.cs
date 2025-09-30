using System;
using Unity.VisualScripting;
using UnityEngine;

public class NPCFollowTargetInput : MonoBehaviour, ICharacterInput
{
    public string NPCType;
    public Transform target;
    public bool isMoving;
    public bool CanNotTalkWhileMoving;
    public float followDistanceThreshold = 1;
    public float walkingDistanceThreshold = 3;
    float jumpCooldown;
    public float jumpCooldownLength = 0.5f;

    public bool IsWalking;
    [HideInInspector] public bool ForceSliding;
    [Header("Jumping")]
    public bool JumpWhenWallInFront;
    public float DistanceForWallCheck;
    public bool JumpWhenEmptySpaceInFront;
    public float emptySpaceForwardDistance = 2;
    public float emptySpaceDownDistance = 3;
    public LayerMask wallCheckLayersIgnore;

    [HideInInspector]
    public Vector3 forwardDir;

    bool canJump = true;
    bool checkingForTriggers = true;

    public event Action OnJump;
    public event Action OnDive;

    void Start()
    {
        forwardDir = transform.forward;
    }

    public bool GetDashInput()
    {
        return ForceSliding;
    }

    public bool GetJumpInput()
    {
        return false;
    }

    public Vector3 GetMovementInput()
    {
        if (target == null) return Vector3.zero;
        Vector3 character2DPosition = transform.position; character2DPosition.y = 0;
        Vector3 target2DPosition = target.position; target2DPosition.y = 0;
        if (isMoving && Vector3.Distance(character2DPosition, target2DPosition) > followDistanceThreshold)
        {
            if (Vector3.Distance(character2DPosition, target2DPosition) < walkingDistanceThreshold)
            {
                IsWalking = true;
            }
            else IsWalking = false;

            Vector3 direction = target2DPosition - character2DPosition;
            return direction.normalized;
        }
        return Vector3.zero;
    }

    void Update()
    {
        if (JumpWhenWallInFront)
        {
            if (Physics.Raycast(transform.position, transform.forward, DistanceForWallCheck, ~wallCheckLayersIgnore))
            {
                Jump();
            }
        }
        if (JumpWhenEmptySpaceInFront)
        {
            if (!Physics.Raycast(transform.position + transform.forward * emptySpaceForwardDistance, Vector3.down, emptySpaceDownDistance, ~wallCheckLayersIgnore))
            {
                Jump();
            }
        }
    }

    void Jump()
    {
        if (jumpCooldown > Time.time) return;
        if (!canJump) return;
        jumpCooldown = Time.time + jumpCooldownLength;
        OnJump?.Invoke();
    }

    public void SetCheckingForTriggers(bool set)
    {
        checkingForTriggers = set;
    }

    void OnTriggerEnter(Collider other)
    {

        if (other.TryGetComponent(out NPCTriggers trigger))
        {
            if (trigger.Activated) return;
            switch (trigger.TriggerType)
            {
                case NPCTriggers.NPCTriggerTypes.Jump:
                    OnJump?.Invoke();
                    break;
                case NPCTriggers.NPCTriggerTypes.Dive:
                    OnDive?.Invoke();
                    break;
                case NPCTriggers.NPCTriggerTypes.Stop:
                    if (!checkingForTriggers) return;
                    SetIsMoving(false);
                    break;
                case NPCTriggers.NPCTriggerTypes.DontJump:
                    canJump = false;
                    break;
                case NPCTriggers.NPCTriggerTypes.ForceSlide:
                    ForceSliding = true;
                    break;
                case NPCTriggers.NPCTriggerTypes.EnableTriggers:
                    checkingForTriggers = true;
                    break;
            }
            if (trigger.activateTrigger != null)
            {
                trigger.activateTrigger.isActive = true;
            }
            if (trigger.OnlyOnce) trigger.Activated = true;
            if (trigger.TurnAroundOnNPC)
            {
                if (TryGetComponent(out CharacterMovement characterMovement))
                {
                    forwardDir = -transform.forward;
                    characterMovement.TurnAround();
                }
            }
            if (trigger.EnableQuestMarker)
            {
                if (TryGetComponent(out DialogWorker dialogWorker))
                {
                    dialogWorker.EnableMarker(true);
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out NPCTriggers trigger))
        {
            if (trigger.Activated) return;
            switch (trigger.TriggerType)
            {
                case NPCTriggers.NPCTriggerTypes.DontJump:
                    canJump = true;
                    break;
                case NPCTriggers.NPCTriggerTypes.ForceSlide:
                    ForceSliding = false;
                    break;
            }
        }
    }




    public void SetIsMoving(bool _isMoving)
    {
        isMoving = _isMoving;
        if (CanNotTalkWhileMoving)
        {
            if (TryGetComponent(out DialogWorker dialogWorker))
            {
                dialogWorker.NPCCanInteract = !isMoving;
            }
        }
    }

    public void SetTargetTransform(Transform newTarget)
    {
        target = newTarget;    
    }
}
