using UnityEngine;
using DG.Tweening;

public class FloatingItem : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] float upwards_amount = 0.5f;
    [SerializeField] float animation_duration_float = 2f;
    [SerializeField] float animation_duration_spin = 2f;
    [SerializeField] Ease ease_type = Ease.InOutQuad;

    [Header("Performance Settings")]
    [SerializeField] float triggerDistance = 100f;
    [SerializeField] float checkDelay = 0.3f;
    [SerializeField] float desyncDelayRange = 0.5f;

    private bool isActive = false;
    private bool wasActive = false;
    private float delayTimer;

    private Transform objectToAnimate;

    private Tween floatTween;
    private Tween spinTween;

    void Start()
    {
        delayTimer = Time.time + checkDelay;

        objectToAnimate = transform;

        float originalY = objectToAnimate.localPosition.y;
        objectToAnimate.localPosition = new Vector3(objectToAnimate.localPosition.x, originalY + Random.Range(-0.1f, 0.1f), objectToAnimate.localPosition.z);

        float randomDelay = Random.Range(0f, desyncDelayRange);

        floatTween = objectToAnimate.DOLocalMoveY(originalY + upwards_amount, animation_duration_float * 0.5f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(ease_type)
            .SetLink(gameObject)
            .SetDelay(randomDelay)
            .Pause();

        spinTween = objectToAnimate.DOLocalRotate(new Vector3(0, 360, 0), animation_duration_spin, RotateMode.LocalAxisAdd)
            .SetLoops(-1)
            .SetEase(Ease.Linear)
            .SetLink(gameObject)
            .SetDelay(randomDelay)
            .Pause();
    }

    void Update()
    {
        if (Time.time > delayTimer)
        {
            delayTimer = Time.time + checkDelay;

            if (objectToAnimate == null || GameplayUtils.instance == null) return;

            float dist = Vector3.Distance(GameplayUtils.instance.PlayerTransform.position, transform.position);
            isActive = dist < triggerDistance;

            if (isActive && !wasActive)
            {
                StartAnimating();
            }
            else if (!isActive && wasActive)
            {
                StopAnimating();
            }

            wasActive = isActive;
        }
    }

    void StartAnimating()
    {
        floatTween.Play();
        spinTween.Play();
    }

    public void StopAnimating()
    {
        floatTween.Pause();
        spinTween.Pause();
    }
}