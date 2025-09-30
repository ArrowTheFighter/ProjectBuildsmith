using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class CutsceneManager : MonoBehaviour
{
    public static CutsceneManager instance;

    public Transform CameraTransform;
    public Camera cutsceneCam;

    CutsceneData currentData;

    public bool cutsceneIsRunning;
    int currentPoint;

    void Awake()
    {
        if (instance != this)
        {
            Destroy(instance);
        }
        instance = this;

    }

    

    public void StartCutscene(CutsceneData cutsceneData)
    {
        print("starting cutscene - " + cutsceneData.CutsceneName);
        currentData = cutsceneData;
        cutsceneCam.gameObject.SetActive(true);
        CameraTransform.position = cutsceneData.startPos.position;
        CameraTransform.forward = cutsceneData.startPos.forward;
        cutsceneIsRunning = true;
        currentPoint = 0;
        if (GameplayUtils.instance.PlayerTransform.TryGetComponent(out CharacterMovement characterMovement))
        {
            characterMovement.MovementControlledByAbility = true;
            characterMovement.rb.isKinematic = true;
        }
        GameplayUtils.instance.HideUI();
        GameplayUtils.instance.SetCanPause(false);
        GoToNextPoint();
    }

    void GoToNextPoint()
    {
        if (!cutsceneIsRunning) return;
        if (currentPoint > currentData.cameraPoints.Length - 1)
        {
            EndCutscene();
            return;
        }
        CutscenePointData pointData = currentData.cameraPoints[currentPoint];
        StartCoroutine(playCutsceneEvent(pointData.cutsceneEvent));
        currentPoint++;
        Quaternion lookRotation = Quaternion.LookRotation(pointData.targetPoint.forward, Vector3.up);
        CameraTransform.DORotate(lookRotation.eulerAngles, pointData.moveDelay).SetUpdate(UpdateType.Fixed).SetEase(pointData.ease);
        CameraTransform.DOMove(pointData.targetPoint.position, pointData.moveDelay).SetUpdate(UpdateType.Fixed).SetEase(pointData.ease).OnComplete(() => { GoToNextPoint(); });
    }

    IEnumerator playCutsceneEvent(CutsceneEvent cutsceneEvent)
    {
        yield return new WaitForSeconds(cutsceneEvent.DelayFromPoint);
        cutsceneEvent.pointEvent?.Invoke();
    }

    void EndCutscene()
    {
        print("ending cutscene");

        if (GameplayUtils.instance.PlayerTransform.TryGetComponent(out CharacterMovement characterMovement))
        {
            characterMovement.MovementControlledByAbility = false;
            characterMovement.rb.isKinematic = false;
        }
        GameplayUtils.instance.ShowUI();
        cutsceneCam.gameObject.SetActive(false);
        cutsceneIsRunning = false;

        GameplayUtils.instance.SetCanPause(true);
        
    }


    public void SkipCutscene()
    {
        if (!cutsceneIsRunning) return;

        // Kill any active tweens
        DOTween.Kill(CameraTransform);

        // Stop any coroutines (delayed events, etc.)
        StopAllCoroutines();

        // Run all cutscene events instantly
        foreach (var point in currentData.cameraPoints)
        {
            if (point.cutsceneEvent?.pointEvent != null)
            {
                int count = point.cutsceneEvent.pointEvent.GetPersistentEventCount();
                for (int i = 0; i < count; i++)
                {
                    var target = point.cutsceneEvent.pointEvent.GetPersistentTarget(i) as ISkippable;
                    if (target != null)
                    {
                        target.Skip();
                    }
                    else
                    {
                        // if not ISkippable, just invoke normally
                        point.cutsceneEvent.pointEvent.Invoke();
                    }
                }
            }
        }

        EndCutscene();
    }


}

[Serializable]
public class CutsceneData
{
    public string CutsceneName;
    public Transform startPos;
    public CutscenePointData[] cameraPoints;
}

[Serializable]
public class CutscenePointData
{
    public Transform targetPoint;
    public Ease ease;
    public float moveDelay;
    public CutsceneEvent cutsceneEvent;
 }

[Serializable]
public class CutsceneEvent
{
    public UnityEvent pointEvent;
    public float DelayFromPoint;
}