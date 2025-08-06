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

    bool cutsceneIsRunning;
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
        GoToNextPoint();
    }

    void GoToNextPoint()
    {
        print(currentData.cameraPoints.Length);
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
        cutsceneCam.gameObject.SetActive(false);
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