using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class FrogBoss : MonoBehaviour,IDamagable
{
    public List<FrogBossPlatform> bossPlatforms = new List<FrogBossPlatform>();
    List<FrogBossPlatform> availableBossPlatforms = new List<FrogBossPlatform>();
    
    public List<FrogBossPlatform> finalPlatforms = new List<FrogBossPlatform>();
    List<FrogBossPlatform> availableFinalPlatforms = new List<FrogBossPlatform>();

    FrogBossPlatform currentTarget;
    FrogBossPlatform lastFinalPlatform;

    Vector3 startPos;
    Vector3 midPos;
    Vector3 endPos;

    Vector3 lastDirectionToPlayer;

    float progress;
    public float jumpingSpeed;
    public float fallingSpeed;
    public float returningSpeed;
    public float speed;
    public float height = 2;
    public float jumpHeight = 15;
    public float pauseDuration = 0.5f;
    public float playerBounceForce = 10;
    public AnimationCurve animationCurve;
    public AnimationCurve fallingCurve;
    public AnimationCurve returningCurve;

    int platformsQueuedUp;

    enum jumpingState
    {
        BreakingPlatforms,
        WaitingToJump,
        MovingToFinal,
        AttackingPlayer,
        KnockedDown,
        JumpingBackUp
    }

    jumpingState currentState;

    bool canStomp = false;
    bool canTakeDamage = false;
    public bool PlayerCanStomp { get => canStomp; set => canStomp = value; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SwitchState(jumpingState.BreakingPlatforms);
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case jumpingState.BreakingPlatforms:
                if (platformsQueuedUp > 0)
                {
                    UpdateProgress(animationCurve);
                    UpdateRotation();
                    if (progress >= 1)
                    {
                        progress = 0;
                        currentState = jumpingState.WaitingToJump;
                    }
                }
                break;
            case jumpingState.WaitingToJump:
                
                progress += Time.deltaTime;
                if (progress > pauseDuration)
                {
                    currentState = jumpingState.BreakingPlatforms;
                    platformsQueuedUp--;
                    progress = 0;

                    KnockDownPlatform();

                    if (platformsQueuedUp <= 0)
                    {
                        currentState = jumpingState.MovingToFinal;
                        GetFinalPlatformPoint();
                        platformsQueuedUp++;
                    }
                    else
                    {
                        GetNewPoint();
                    }

                }

            break;

            case jumpingState.MovingToFinal:
                if (platformsQueuedUp > 0)
                {
                    UpdateProgress(animationCurve);
                    UpdateRotation();
                    if (progress >= 1)
                    {
                        platformsQueuedUp--;
                        progress = 0;
                        SwitchState(jumpingState.AttackingPlayer);
                    }
                }
                break;

            case jumpingState.AttackingPlayer:

                Vector3 currentDirection = ScriptRefrenceSingleton.instance.gameplayUtils.PlayerTransform.position - transform.position;
                currentDirection.y = 0;
                if(Vector3.Angle(lastDirectionToPlayer.normalized,currentDirection.normalized) > 15)
                {
                    lastDirectionToPlayer = currentDirection;
                }
                Quaternion newRotation = Quaternion.LookRotation(lastDirectionToPlayer.normalized);
                transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, 0.2f);

                break;
            case jumpingState.KnockedDown:
            print("falling");

                UpdateProgress(fallingCurve);
                FacePlatformForward();
                if (progress >= 1)
                {
                    SwitchState(jumpingState.JumpingBackUp);
                }

                break;
            
            case jumpingState.JumpingBackUp:

                UpdateProgress(returningCurve);
                FacePlatformForward();
                if (progress >= 1)
                {
                    SwitchState(jumpingState.BreakingPlatforms);
                }

                break;
        }
    }

    void SwitchState(jumpingState newState)
    {
        switch (newState)
        {
            case jumpingState.BreakingPlatforms:
                print("switching to breaking platforms");
                foreach (var platform in bossPlatforms)
                {
                    platform.ResetPlatform();
                }

                canTakeDamage = false;
                canStomp = false;
                platformsQueuedUp = 3;
                speed = jumpingSpeed;
                progress = 0;
                GetNewPoint();

                break;

            case jumpingState.WaitingToJump:
            
                break;

            case jumpingState.MovingToFinal:

                break;

            case jumpingState.AttackingPlayer:

                Vector3 direction = ScriptRefrenceSingleton.instance.gameplayUtils.PlayerTransform.position - transform.position;
                direction.y = 0;

                lastDirectionToPlayer = direction;
                canTakeDamage = true;
                canStomp = true;
                break;

            case jumpingState.KnockedDown:
                GetFallingPoint();
                progress = 0;
                speed = fallingSpeed;
                break;
            
            case jumpingState.JumpingBackUp:

                GetJumpingBackUpPoint();
                progress = 0;
                speed = returningSpeed;
                break;
        }
        currentState = newState;
    }

    void UpdateProgress(AnimationCurve curve)
    {
        progress += speed * Time.deltaTime;
        float curvedProgress = curve.Evaluate(progress);

        Vector3 pos1 = Vector3.Lerp(startPos, midPos, curvedProgress);
        Vector3 pos2 = Vector3.Lerp(midPos, endPos, curvedProgress);
        transform.position = Vector3.Lerp(pos1, pos2, curvedProgress);
        
    }

    void UpdateRotation()
    {
        Quaternion newRotation = Quaternion.LookRotation((endPos - startPos).normalized);
        transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, 0.2f);
    }

    void FacePlatformForward()
    {
        Quaternion newRotation = Quaternion.LookRotation(currentTarget.getForward());
        transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, 0.2f);
    }

    void GetNewPoint()
    {
        startPos = transform.position;

        availableBossPlatforms.Clear();
        availableBossPlatforms.AddRange(bossPlatforms);
        availableBossPlatforms.Remove(currentTarget);
        foreach(var platform in bossPlatforms)
        {
            if (platform.is_broken)
            {
                availableBossPlatforms.Remove(platform);
            }
        }

        currentTarget = availableBossPlatforms[Random.Range(0, availableBossPlatforms.Count)];
        endPos = currentTarget.getTopPos() + Vector3.up * height;

        midPos = Vector3.Lerp(startPos,endPos,0.5f) + Vector3.up * jumpHeight;
    }

    void GetFallingPoint()
    {
        if (currentTarget == null)
            return;

        startPos = currentTarget.getTopPos() + Vector3.up * height;

        endPos = currentTarget.getTopPos() + - currentTarget.getForward() * 15 + Vector3.down * 15;

        midPos = Vector3.Lerp(startPos, endPos, 0.5f) + Vector3.up * 20;
    }

    void GetJumpingBackUpPoint()
    {
        if (currentTarget == null)
            return;

        startPos = transform.position;

        endPos = transform.position + Vector3.up * 20;

        midPos = Vector3.Lerp(startPos, endPos, 0.5f);
    }

    void GetFinalPlatformPoint()
    {
        startPos = currentTarget.getTopPos() + Vector3.up * height;


        availableFinalPlatforms.Clear();
        availableFinalPlatforms.AddRange(finalPlatforms);
        if (lastFinalPlatform != null)
        {
            availableFinalPlatforms.Remove(lastFinalPlatform);
        }

        currentTarget = availableFinalPlatforms[Random.Range(0, availableFinalPlatforms.Count)];
        lastFinalPlatform = currentTarget;
        endPos = currentTarget.getTopPos() + Vector3.up * height;

        midPos = Vector3.Lerp(startPos, endPos, 0.5f) + Vector3.up * jumpHeight;
    }

    void KnockDownPlatform()
    {
        if (currentTarget != null)
        {
            currentTarget.KnockDown();
        }
    }

    [ContextMenu("Reset")]
    public void Reset()
    {
        platformsQueuedUp = 3;
        progress = 0;
        foreach(var platform in bossPlatforms)
        {
            platform.ResetPlatform();
        }
        currentState = jumpingState.BreakingPlatforms;
        GetNewPoint();
    }

    public void TakeDamage(float amount, AttackType[] attackTypes, GameObject source)
    {
        if(!canTakeDamage) return;
        print("Taking damage!");

        SwitchState(jumpingState.KnockedDown);

    }

    public void TakeDamage(float amount, AttackType[] attackTypes, GameObject source, out float ExtraForce)
    {
        TakeDamage(amount,attackTypes,source);
        ExtraForce = playerBounceForce;
    }

    public void TakeDamage(float amount, AttackType[] attackTypes, GameObject source, float knockbackStrength = 1)
    {
        TakeDamage(amount,attackTypes,source);
    }
}
