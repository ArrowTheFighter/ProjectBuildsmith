using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class CutsceneManager : MonoBehaviour
{

    public Transform CameraTransform;
    public Camera cutsceneCam;

    CutsceneData currentData;

    public bool cutsceneIsRunning;
    int currentPoint;


    public void StartCutscene(CutsceneData cutsceneData)
    {
        print("starting cutscene - " + cutsceneData.CutsceneName);
        currentData = cutsceneData;
        cutsceneCam.gameObject.SetActive(true);
        CameraTransform.position = cutsceneData.startPos.position;
        CameraTransform.forward = cutsceneData.startPos.forward;
        cutsceneIsRunning = true;
        currentPoint = 0;
        if (ScriptRefrenceSingleton.instance.gameplayUtils.PlayerTransform.TryGetComponent(out CharacterMovement characterMovement))
        {
            characterMovement.MovementControlledByAbility = true;
            characterMovement.rb.isKinematic = true;
        }
        ScriptRefrenceSingleton.instance.gameplayUtils.HideUI();
        ScriptRefrenceSingleton.instance.gameplayUtils.SetCanPause(false);
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

        if (ScriptRefrenceSingleton.instance.gameplayUtils.PlayerTransform.TryGetComponent(out CharacterMovement characterMovement))
        {
            characterMovement.MovementControlledByAbility = false;
            characterMovement.rb.isKinematic = false;
        }
        ScriptRefrenceSingleton.instance.gameplayUtils.ShowUI();
        cutsceneCam.gameObject.SetActive(false);
        cutsceneIsRunning = false;

        ScriptRefrenceSingleton.instance.gameplayUtils.SetCanPause(true);
        
    }


    public void SkipCutscene()
    {
        if (!cutsceneIsRunning) return;

        // Kill any active tweens
        DOTween.Kill(CameraTransform);

        // Stop any coroutines (delayed events, etc.)
        StopAllCoroutines();

        currentData.OnLoadSkipEvent?.Invoke();
        // Run all cutscene events instantly
        // foreach (var point in currentData.cameraPoints)
        // {
        //     if (point.cutsceneEvent?.pointEvent != null)
        //     {
        //         int count = point.cutsceneEvent.pointEvent.GetPersistentEventCount();
        //         for (int i = 0; i < count; i++)
        //         {
        //             var target = point.cutsceneEvent.pointEvent.GetPersistentTarget(i) as ISkippable;
        //             if (target != null)
        //             {
        //                 target.Skip();
        //             }
        //             else
        //             {
        //                 // if not ISkippable, just invoke normally
        //                 //point.cutsceneEvent.pointEvent.Invoke();
        //             }
        //         }
        //     }
        // }

        EndCutscene();
    }


}

[Serializable]
public class CutsceneData
{
    public string CutsceneName;
    public Transform startPos;
    public CutscenePointData[] cameraPoints;
    public UnityEvent OnLoadSkipEvent;
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