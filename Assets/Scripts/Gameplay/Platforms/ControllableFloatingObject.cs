using UnityEngine;
using System;
using DG.Tweening;

public class ControllableFloatingObject : MonoBehaviour, IMoveingPlatform
{
    [SerializeField] Vector3 MoveTo;
    public float positionBetweenPoints;

    Vector3 startPos;
    Vector3 endPos;
    Vector3 lastPosition;
    Rigidbody rb;
    Tween moveTween;

    public event Action<Vector3> OnPlatformMove;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        rb = GetComponent<Rigidbody>();
        startPos = transform.position;
        endPos = startPos + MoveTo;

        lastPosition = transform.position;
        //transform.DOMove(transform.position + MoveTo, duration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuad);
        // if (rb != null)
        // {
        //     startPos = transform.position;
        //     rb.DOMove(transform.position + MoveTo, duration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuad).SetUpdate(UpdateType.Fixed);
        // }
    }
    public void SetPosition(float position)
    {
        positionBetweenPoints = Mathf.Clamp01(position);
    }

    public void MovePosition(float speed)
    {
        positionBetweenPoints = Mathf.Clamp01(positionBetweenPoints + speed * Time.deltaTime);
    }

    public void MoveToStartPosition(float speed)
    {
        if (moveTween != null && moveTween.IsPlaying()) moveTween.Kill();
        print("starting tween move to start");
        moveTween = DOVirtual.Float(positionBetweenPoints, 0, speed, (value) => { positionBetweenPoints = Mathf.Clamp01(value); })
        .SetSpeedBased(true)
        .SetEase(Ease.Unset);
    }

    public void MoveToEndPosition(float speed)
    {
        if (moveTween != null && moveTween.IsPlaying()) moveTween.Kill();
        print("starting tween move to end");
        moveTween = DOVirtual.Float(positionBetweenPoints, 1, speed, (value) => { positionBetweenPoints = Mathf.Clamp01(value); })
        .SetSpeedBased(true)
        .SetEase(Ease.Unset);
    }

    private void FixedUpdate()
    {
        Vector3 newPos = Vector3.Lerp(startPos, endPos, Mathf.Clamp01(positionBetweenPoints));
        if (transform.position != newPos)
        {
            rb.MovePosition(newPos);
        }

        if (rb == null) return;
        Vector3 delta = rb.position - lastPosition;

        OnPlatformMove?.Invoke(delta);

        lastPosition = rb.position;
    }

    public void SetPositionAsStart()
    {
        startPos = transform.position;
        endPos = startPos + MoveTo;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + MoveTo);
        Gizmos.DrawSphere(transform.position + MoveTo, 0.2f);
        if (TryGetComponent(out MeshFilter meshFilter))
        {
            if (meshFilter.sharedMesh != null)
            {
                Gizmos.DrawWireMesh(meshFilter.sharedMesh, transform.position + MoveTo, transform.rotation, transform.localScale);
            }
        }
    }
}
