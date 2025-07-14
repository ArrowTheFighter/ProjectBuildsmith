using System;
using UnityEngine;

public class NPCFollowTargetInput : MonoBehaviour, ICharacterInput
{
    public Transform target;
    public bool isMoving;
    public bool CanNotTalkWhileMoving;
    public float followDistanceThreshold = 1;
    public float walkingDistanceThreshold = 3;

    public bool IsWalking;
    [Header("Jumping")]
    public bool JumpWhenWallInFront;
    public float DistanceForWallCheck;
    public LayerMask wallCheckLayersIgnore;

    public event Action OnJump;
    public event Action OnDive;

    public bool GetDashInput()
    {
        return false;
    }

    public bool GetJumpInput()
    {
        return false;
    }

    public Vector3 GetMovementInput()
    {
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
                OnJump?.Invoke();
             }
         }
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
                    SetIsMoving(false);
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
                    characterMovement.TurnAround();
                 }
            }
         }
    }

    public void SetIsMoving(bool _isMoving)
    {
        isMoving = _isMoving;
        if (CanNotTalkWhileMoving)
        {
            if(TryGetComponent(out DialogWorker dialogWorker))
            {
                dialogWorker.NPCCanInteract = !isMoving;
            }
        }
    }
}
