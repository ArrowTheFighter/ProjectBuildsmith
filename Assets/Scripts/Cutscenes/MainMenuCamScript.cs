using DG.Tweening;
using UnityEngine;

public class MainMenuCamScript : MonoBehaviour
{
    public Transform CameraFirstPosition;
    public Transform CameraSecondPosition;

    public float CameraMoveDuration = 6;
    public Ease cameraEase = Ease.InOutSine;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (CameraFirstPosition != null && CameraSecondPosition != null)
        {

            transform.DOMove(CameraSecondPosition.position, CameraMoveDuration).From(CameraFirstPosition.position,false).SetEase(cameraEase).SetLoops(-1, LoopType.Yoyo);
            Quaternion startingLookRotation = Quaternion.LookRotation(CameraFirstPosition.forward, Vector3.up);
            Quaternion finalLookRotation = Quaternion.LookRotation(CameraSecondPosition.forward, Vector3.up);
            transform.DORotate(finalLookRotation.eulerAngles, CameraMoveDuration).From(startingLookRotation.eulerAngles,false,false).SetEase(cameraEase).SetLoops(-1, LoopType.Yoyo);
        }
    }

}
