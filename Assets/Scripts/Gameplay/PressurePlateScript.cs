using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class PressurePlateScript : MonoBehaviour
{
    public UnityEvent ActivatedEvent;
    public UnityEvent DeactivatedEvent;
    bool isActive;
    Collider col;

    Tween PlateTween;
    public Transform PlateVisual;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        col = GetComponent<Collider>();
        ActivatedEvent.AddListener(MovePlateDown);
        DeactivatedEvent.AddListener(MovePlatUp);
    }

    public void MovePlateDown()
    {
        if (PlateVisual != null)
        {
            if (PlateTween != null && PlateTween.IsPlaying())
            {
                PlateTween.Kill();
            }
            PlateTween = PlateVisual.DOLocalMoveY(-0.2f,0.5f).SetEase(Ease.OutQuad);
        }
    }

    public void MovePlatUp()
    {
        if (PlateVisual != null)
        {
            if (PlateTween != null && PlateTween.IsPlaying())
            {
                PlateTween.Kill();
            }
            PlateTween = PlateVisual.DOLocalMoveY(0, 0.5f).SetEase(Ease.OutQuad);
        }
    }

    void Activated()
    {
        ActivatedEvent?.Invoke();
    }

    void Deactivated()
    {
        DeactivatedEvent?.Invoke();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            print("Player landed on pressure plate");
            if (!isActive)
            {
                Activated();
                isActive = true;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            print("Player left the pressure plate");
            if (isActive)
            {
                Deactivated();
                isActive = false;
            }
        }
    }
}
