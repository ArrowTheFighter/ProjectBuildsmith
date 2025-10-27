using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class NPCFlyingFollow : MonoBehaviour
{
    public Transform TargetTransform;
    public Vector3 TargetOffset;
    public Vector3 TargetPos
    {
        get
        {
            if (TargetTransform != null)
                return TargetTransform.position + TargetOffset;
            return Vector3.zero;
        }
    }
    public float FollowDistance;
    public float FarFollowDistance;
    float velocity;
    float verticalVelocity;
    public float VerticalFollowDistance = 1;
    public float Speed
    {
        get
        {
            return Vector3.Distance(TargetPos, transform.position) < FarFollowDistance ? CloseSpeed : FarSpeed;
        }
    }
    public float CloseSpeed;
    public float FarSpeed;
    [Header("Body Visuals")]
    public Transform BodyVisuals;

    [Header("Animation")]
    public Animator animator;

    [Header("Particles")]
    public List<ParticleSystem> FireParticles;

    void Start()
    {
        if(BodyVisuals != null)
        {
            BodyVisuals.DOLocalMoveY(-0.5f, 2).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(TargetTransform != null)
        {
            
            if (Vector3.Distance(TargetPos, transform.position) > FollowDistance)
            {
                velocity = Mathf.Lerp(velocity, 1, 0.1f);
            }
            else
            {
                velocity = Mathf.Lerp(velocity, 0, 0.1f);
            }
            
            Vector3 dirToTarget = (TargetPos - transform.position).normalized;
            Vector3 horDir = dirToTarget;
            horDir.y = 0;
            
            transform.Translate(horDir * Speed * velocity * Time.deltaTime, Space.World);

            if (Mathf.Abs(transform.position.y - TargetPos.y) > VerticalFollowDistance)
            {
                verticalVelocity = Mathf.Lerp(verticalVelocity, 1, 0.1f);
            }
            else
            {
                verticalVelocity = Mathf.Lerp(verticalVelocity, 0, 0.1f);
            }

            float verticalDir = dirToTarget.y;
            transform.Translate(new Vector3(0, verticalDir, 0) * Speed * verticalVelocity * Time.deltaTime, Space.World);
            
            Vector3 newLookDir = dirToTarget;
            newLookDir.y = 0;
            if (newLookDir.sqrMagnitude > 0.001f)
            {
                Quaternion lookRotation = Quaternion.LookRotation(newLookDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 0.1f);
            }
            if(animator != null)
            {
                animator.SetFloat("SpeedBlend", Mathf.Max(velocity,verticalVelocity));
            }
            
            if(FireParticles.Count > 0)
            {
                foreach(var particle in FireParticles)
                {
                    var emission = particle.emission;
                    emission.rateOverTime = Mathf.Max(velocity, verticalVelocity) * 15;
                }
            }

            
        }
    }
}
