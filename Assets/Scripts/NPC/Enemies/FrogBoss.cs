using System.Collections.Generic;
using UnityEngine;

public class FrogBoss : MonoBehaviour
{
    public List<FrogBossPlatform> bossPlatforms = new List<FrogBossPlatform>();
    List<FrogBossPlatform> availableBossPlatforms = new List<FrogBossPlatform>();
    
    public List<FrogBossPlatform> finalPlatforms = new List<FrogBossPlatform>();

    FrogBossPlatform currentTarget;

    Vector3 startPos;
    Vector3 midPos;
    Vector3 endPos;

    Vector3 lastDirectionToPlayer;

    float progress;
    public float speed;
    public float height = 2;
    public float jumpHeight = 15;
    public float pauseDuration = 0.5f;
    public AnimationCurve animationCurve;

    int platformsQueuedUp;

    enum jumpingState
    {
        BreakingPlatforms,
        WaitingToJump,
        MovingToFinal,
        AttackingPlayer
    }

    jumpingState currentState;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentState = jumpingState.BreakingPlatforms;
        platformsQueuedUp = 3;
        GetNewPoint();
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case jumpingState.BreakingPlatforms:
                if (platformsQueuedUp > 0)
                {
                    UpdateProgress();
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
                    UpdateProgress();
                    if (progress >= 1)
                    {
                        platformsQueuedUp--;
                        progress = 0;
                        currentState = jumpingState.AttackingPlayer;
                    }
                }
                break;

            case jumpingState.AttackingPlayer:

                Vector3 direction = ScriptRefrenceSingleton.instance.gameplayUtils.PlayerTransform.position - transform.position;
                direction.y = 0;
                Quaternion newRotation = Quaternion.LookRotation(direction.normalized);
                transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, 0.2f);

                break;
        }
    }

    void SwitchState(jumpingState newState)
    {
        switch (newState)
        {
            case jumpingState.BreakingPlatforms:

            break;

            case jumpingState.WaitingToJump:
            
            break;

            case jumpingState.MovingToFinal:

            break;

            case jumpingState.AttackingPlayer:

                Vector3 direction = ScriptRefrenceSingleton.instance.gameplayUtils.PlayerTransform.position - transform.position;
                direction.y = 0;

                lastDirectionToPlayer = direction;
            break;
        }
        currentState = newState;
    }

    void UpdateProgress()
    {
        progress += speed * Time.deltaTime;
        float curvedProgress = animationCurve.Evaluate(progress);

        Vector3 pos1 = Vector3.Lerp(startPos, midPos, curvedProgress);
        Vector3 pos2 = Vector3.Lerp(midPos, endPos, curvedProgress);
        transform.position = Vector3.Lerp(pos1, pos2, curvedProgress);

        Quaternion newRotation = Quaternion.LookRotation((endPos - startPos).normalized);
        transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, 0.2f);
    }

    void GetNewPoint()
    {
        if (currentTarget == null)
            currentTarget = bossPlatforms[Random.Range(0, bossPlatforms.Count)];

        startPos = currentTarget.getTopPos() + Vector3.up * height;

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

    void GetFinalPlatformPoint()
    {
        startPos = currentTarget.getTopPos() + Vector3.up * height;

        currentTarget = finalPlatforms[Random.Range(0,finalPlatforms.Count)];
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
}
