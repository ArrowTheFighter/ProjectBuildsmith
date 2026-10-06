using System.Collections;
using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;

public class CatapultLaunch : PlayerAbility
{
    [Header("Launch Settings")]
    [SerializeField] private float launchSpeed = 100f;
    [SerializeField] private float arcHeight;
    [SerializeField] private float arcHeightMultiplier = 0.4f;

    private Vector3 launchStart;
    private Vector3 launchDestination;

    private float launchDuration;
    private float launchTimer;

    private bool launchStarted;

    public override void Initialize(CharacterMovement player)
    {
        base.Initialize(player);

        characterMovement.MovementControlledByAbility = true;
        characterMovement.rb.linearDamping = 0f;

        launchTimer = 0f;
        launchStarted = false;

        if (TryGetComponent(out LongFallReset longFallReset))
        {
            longFallReset.CanReset = false;
        }

        characterMovement.playerAnimationController.animator
            .CrossFade("CatapultRoll", 0.1f);

        ScriptRefrenceSingleton.instance.playerParticlesManager
            .PlayParticleByID("SpeedLines");

        CinemachineCamera cam = characterMovement
            .GetComponent<CameraManager>()
            .axisController
            .GetComponent<CinemachineCamera>();

        DOVirtual.Float(
            cam.Lens.FieldOfView,
            70f,
            0.5f,
            value => cam.Lens.FieldOfView = value
        );

        characterMovement.capsuleCollider.enabled = false;

        StartCoroutine(ReEnableCollisions());
    }

    private IEnumerator ReEnableCollisions()
    {
        yield return new WaitForSeconds(0.5f);

        characterMovement.capsuleCollider.enabled = true;
    }

    public void StartLaunch(Vector3 destination)
    {
        launchStart = characterMovement.rb.position;
        launchDestination = destination;

        Vector3 horizontalStart = launchStart;
        Vector3 horizontalDestination = launchDestination;

        Vector3 direction = horizontalDestination - horizontalStart;
        direction.y = 0f;

        if (direction != Vector3.zero)
        {
            characterMovement.transform.rotation = Quaternion.LookRotation(direction);
        }

        horizontalStart.y = 0f;
        horizontalDestination.y = 0f;

        float distance = Vector3.Distance(
            horizontalStart,
            horizontalDestination
        );

        launchDuration = distance / launchSpeed;

        arcHeight = distance * arcHeightMultiplier;

        launchTimer = 0f;
        launchStarted = true;
    }

    public override void UpdateAbility()
    {
    }

    public override void FixedUpdateAbility()
    {
        if (!launchStarted)
            return;

        launchTimer += Time.fixedDeltaTime;

        float t = Mathf.Clamp01(launchTimer / launchDuration);

        Vector3 targetPosition = EvaluateArc(t);

        characterMovement.rb.MovePosition(targetPosition);

        if (t >= 1f)
        {
            FinishLaunch();
        }
    }

    private Vector3 EvaluateArc(float t)
    {
        Vector3 p0 = launchStart;
        Vector3 p2 = launchDestination;

        Vector3 p1 = Vector3.Lerp(p0, p2, 0.5f);
        p1.y += arcHeight;

        float oneMinusT = 1f - t;

        return
            oneMinusT * oneMinusT * p0 +
            2f * oneMinusT * t * p1 +
            t * t * p2;
    }

    private void FinishLaunch()
    {
        launchStarted = false;

        characterMovement.rb.linearVelocity = Vector3.zero;

        characterMovement.rb.MovePosition(launchDestination);

        characterMovement.MovementControlledByAbility = false;

        if (TryGetComponent(out LongFallReset longFallReset))
        {
            longFallReset.CanReset = true;
        }

        ScriptRefrenceSingleton.instance.playerParticlesManager
            .GetParticleByID("SpeedLines")
            .Stop(
                false,
                ParticleSystemStopBehavior.StopEmittingAndClear
            );

        CinemachineCamera cam = characterMovement
            .GetComponent<CameraManager>()
            .axisController
            .GetComponent<CinemachineCamera>();

        DOVirtual.Float(
            cam.Lens.FieldOfView,
            60f,
            0.5f,
            value => cam.Lens.FieldOfView = value
        );

        characterMovement.playerAnimationController.animator
            .CrossFade("WalkingBlend", 0.1f);

        float tweenDuration = 0.15f;

        characterMovement.orientation
            .DOScale(
                new Vector3(1.25f, 0.75f, 1.25f),
                tweenDuration
            )
            .SetLoops(2, LoopType.Yoyo)
            .OnComplete(() =>
            {
                characterMovement.orientation.localScale = Vector3.one;
            });

        characterMovement.orientation
            .DOLocalMoveY(-0.125f, tweenDuration)
            .SetLoops(2, LoopType.Yoyo)
            .OnComplete(() =>
            {
                characterMovement.orientation.localPosition = Vector3.zero;
            });

        characterMovement.RemoveAbility<CatapultLaunch>();
    }

    public override void ResetAbility()
    {
    }
}