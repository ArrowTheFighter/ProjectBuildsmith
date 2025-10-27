using UnityEngine;
using System;
using System.Collections;

public class ControllableFloatingObject : MonoBehaviour, IMoveingPlatform
{
    [Header("Movement Settings")]
    [SerializeField] Vector3 MoveTo;
    [SerializeField] float duration = 2f;
    [Tooltip("If set > 0, this duration will be used when returning. Otherwise, it defaults to the main duration.")]
    [SerializeField] float returnDuration = 0f;
    [SerializeField] float returnDelay = 1f;
    [SerializeField] AnimationCurve easing = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] Vector3 rotationAmount;

    public float positionBetweenPoints;

    Vector3 startPos;
    Vector3 endPos;
    Vector3 lastPosition;
    Rigidbody rb;

    Coroutine moveRoutine;

    public event Action<Vector3> OnPlatformMove;
    public event Action OnBeforePlatformMove;
    public event Action OnAfterPlatformMove;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPos = transform.position;
        endPos = startPos + MoveTo;
        lastPosition = transform.position;
    }

    // =========================
    // PUBLIC CONTROL METHODS
    // =========================

    /// <summary>
    /// Moves to the end, waits, then returns to start (using returnDuration if set).
    /// </summary>
    public void ActivateAutoReturn()
    {
        if (moveRoutine != null)
            StopCoroutine(moveRoutine);

        moveRoutine = StartCoroutine(MoveSequenceAutoReturn());
    }

    /// <summary>
    /// Moves to the end and stays there.
    /// </summary>
    public void ActivateStayAtEnd()
    {
        if (moveRoutine != null)
            StopCoroutine(moveRoutine);

        moveRoutine = StartCoroutine(MoveToTarget(1f, duration, () =>
        {
            moveRoutine = null;
        }));
    }

    /// <summary>
    /// Moves from end back to start (using returnDuration if set).
    /// </summary>
    public void ActivateReturnToStart()
    {
        if (moveRoutine != null)
            StopCoroutine(moveRoutine);

        float moveTime = returnDuration > 0f ? returnDuration : duration;

        moveRoutine = StartCoroutine(MoveToTarget(0f, moveTime, () =>
        {
            moveRoutine = null;
        }));
    }

    // =========================
    // INTERNAL COROUTINES
    // =========================

    private IEnumerator MoveSequenceAutoReturn()
    {
        // Move start → end
        yield return MoveToTarget(1f, duration);

        // Wait delay
        yield return new WaitForSeconds(returnDelay);

        // Move end → start (use returnDuration if provided)
        float moveTime = returnDuration > 0f ? returnDuration : duration;
        yield return MoveToTarget(0f, moveTime);

        moveRoutine = null;
    }

    private IEnumerator MoveToTarget(float target, float moveTime, Action onComplete = null)
    {
        float elapsed = 0f;
        float startValue = positionBetweenPoints;

        while (elapsed < moveTime)
        {
            elapsed += Time.fixedDeltaTime;
            float t = Mathf.Clamp01(elapsed / moveTime);
            float easedT = easing.Evaluate(t);

            positionBetweenPoints = Mathf.Lerp(startValue, target, easedT);
            yield return new WaitForFixedUpdate();
        }

        positionBetweenPoints = target;
        onComplete?.Invoke();
    }

    // =========================
    // MOVEMENT LOGIC
    // =========================

    private void FixedUpdate()
    {
        if (moveRoutine == null) return;

        OnBeforePlatformMove?.Invoke();

        Vector3 newPos = Vector3.Lerp(startPos, endPos, Mathf.Clamp01(positionBetweenPoints));
        transform.position = newPos;
        transform.Rotate(rotationAmount);

        if (rb != null)
        {
            Vector3 delta = rb.position - lastPosition;
            OnPlatformMove?.Invoke(delta);
            lastPosition = rb.position;
        }

        OnAfterPlatformMove?.Invoke();
    }

    // =========================
    // UTILITY
    // =========================

    public void SetPositionAsStart()
    {
        startPos = transform.position;
        endPos = startPos + MoveTo;
        positionBetweenPoints = 0f;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + MoveTo);
        Gizmos.DrawSphere(transform.position + MoveTo, 0.2f);

        if (TryGetComponent(out MeshFilter meshFilter) && meshFilter.sharedMesh != null)
        {
            Gizmos.DrawWireMesh(meshFilter.sharedMesh, transform.position + MoveTo, transform.rotation, transform.localScale);
        }
    }

    public Transform getInterfaceTransform()
    {
        return transform;
    }
}
