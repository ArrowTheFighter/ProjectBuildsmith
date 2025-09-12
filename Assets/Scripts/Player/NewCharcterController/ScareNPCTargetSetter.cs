using UnityEngine;

public class ScareNPCTargetSetter : MonoBehaviour
{
    public Transform targetTransform;
    public float MaxDistanceToPlayer = 5;
    public bool WonderWhenAlone;
    Vector3 wonderPos;
    Vector3 wonderTarget;
    public float wonderDistance;
    float timeAloneCooldown;
    public float wonderCooldown;
    public float wonderSpeed;
    float FleeingSpeed;

    void Start()
    {
        wonderPos = transform.position;
        timeAloneCooldown = Time.time + Random.Range(wonderCooldown - wonderCooldown * 0.2f, wonderCooldown + wonderCooldown * 0.2f);
        if (TryGetComponent(out CharacterMovement characterMovement))
        {
            FleeingSpeed = characterMovement.walkSpeed;
        }
    }

    void FixedUpdate()
    {
        float distanceToPlayer = Vector3.Distance(GameplayUtils.instance.PlayerTransform.position, transform.position);
        if (distanceToPlayer < MaxDistanceToPlayer)
        {
            float playerDistanceLerpValue = Mathf.InverseLerp(MaxDistanceToPlayer, 1.5f, distanceToPlayer);
            if (TryGetComponent(out CharacterMovement characterMovement))
            {
                characterMovement.walkSpeed = Mathf.Lerp(0, FleeingSpeed,playerDistanceLerpValue);
            }


            Vector3 dirToMove = (transform.position - GameplayUtils.instance.PlayerTransform.position).normalized;
            targetTransform.position = transform.position + dirToMove * 2;
            timeAloneCooldown = Time.time + Random.Range(wonderCooldown - wonderCooldown * 0.2f, wonderCooldown + wonderCooldown * 0.2f);
            wonderPos = targetTransform.position;
            wonderTarget = wonderPos;
        }
        else
        {

            targetTransform.position = wonderTarget;
            if (WonderWhenAlone && Time.time > timeAloneCooldown)
            {
                if (TryGetComponent(out CharacterMovement characterMovement))
                {
                    characterMovement.walkSpeed = wonderSpeed;
                }
                timeAloneCooldown = Time.time + Random.Range(wonderCooldown - wonderCooldown * 0.2f, wonderCooldown + wonderCooldown * 0.2f);
                wonderTarget = GetRandomPosNearPoint(wonderPos);
            }
        }

    }

    Vector3 GetRandomPosNearPoint(Vector3 point)
    {
        Vector3 randomDir = Random.insideUnitSphere;
        randomDir.y = 0;
        Vector3 position = point + randomDir * wonderDistance;
        return position;
     }
}