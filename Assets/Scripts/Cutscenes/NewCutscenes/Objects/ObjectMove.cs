using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class ObjectMove : MonoBehaviour, ISkippable, IMoveingPlatform
{
    [Header("Movement Settings")]
    public Vector3 endOffset;
    private Vector3 startPos;
    public Transform endTransform;
    public float duration = 1f;
    public Ease ease = Ease.Linear;

    [Header("Arch Settings (Parabolic)")]
    public bool useArch = false;
    [Tooltip("Height of the arc above midpoint")]
    public float archHeight = 2f;
    [Tooltip("Controls how flat or sharp the top of the arch is. Higher = flatter top.")]
    [Range(0.5f, 5f)]
    public float archSharpness = 2f;

    [Header("Events")]
    public UnityEvent finishedEvent;
    public UnityEvent SkipEvent;

    [Header("Gizmo Display")]
    public GameObject displayObj;

    public event Action OnBeforePlatformMove;

    [SerializeField] bool AutoAddChildComponenets = true;

    void OnEnable()
    {
        startPos = transform.position;
    }

    void Start()
    {
        if (AutoAddChildComponenets)
        {
            Collider[] childColliders = GetComponentsInChildren<Collider>();

            foreach (var collider in childColliders)
            {
                if (collider.transform == transform) continue;
                if (collider.isTrigger) continue;
                GameObject childObj = collider.gameObject;
                MovingPlatformChild movingPlatformChild = childObj.AddComponent<MovingPlatformChild>();

                movingPlatformChild.ResetComponenet();
                movingPlatformChild.ParentTransform = transform;
                movingPlatformChild.SetupComponenet();
            }
        }
    }

    public void Skip()
    {
        DOTween.Kill(transform);
        transform.position = endTransform != null ? endTransform.position : startPos + endOffset;
        SkipEvent?.Invoke();
    }

    public void StartMove()
    {
        Vector3 start = transform.position;
        Vector3 end = endTransform != null ? endTransform.position : start + endOffset;

        if (useArch)
        {
            float t = 0;
            DOTween.To(() => t, x =>
            {
                t = x;
                Vector3 pos = Vector3.Lerp(start, end, t);

                // base parabola shape
                float yOffset = archHeight * (1 - Mathf.Pow(Mathf.Abs(2 * t - 1), archSharpness));
                pos.y += yOffset;

                OnBeforePlatformMove?.Invoke();

                transform.position = pos;
            }, 1f, duration)
            .SetEase(ease)
            .SetUpdate(UpdateType.Fixed)
            .OnComplete(() => finishedEvent?.Invoke());
        }
        else
        {
            float t = 0;
            DOTween.To(() => t, x =>
            {
                t = x;
                Vector3 pos = Vector3.Lerp(start, end, t);

                OnBeforePlatformMove?.Invoke();

                transform.position = pos;
            }, 1f, duration)
            .SetEase(ease)
            .SetUpdate(UpdateType.Fixed)
            .OnComplete(() => finishedEvent?.Invoke());


            // transform.DOMove(end, duration)
            //     .SetEase(ease)
            //     .SetUpdate(UpdateType.Fixed)
            //     .OnComplete(() => finishedEvent?.Invoke());
        }
    }

    void OnDrawGizmosSelected()
    {
        //if (!useArch) return;

        Vector3 start = displayObj != null ? displayObj.transform.position : transform.position;
        Vector3 end = endTransform != null ? endTransform.position : start + endOffset;

        Gizmos.color = Color.yellow;

        const int steps = 20;
        Vector3 prev = start;
        for (int i = 1; i <= steps; i++)
        {
            float t = i / (float)steps;
            Vector3 pos = Vector3.Lerp(start, end, t);
            float yOffset = archHeight * (1 - Mathf.Pow(Mathf.Abs(2 * t - 1), archSharpness));
            pos.y += yOffset;

            Gizmos.DrawLine(prev, pos);
            prev = pos;
        }

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(end, 0.15f);

        Transform checkTransform = transform;
        if (displayObj != null) checkTransform = displayObj.transform;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(checkTransform.position, checkTransform.position + endOffset);
        Gizmos.DrawSphere(checkTransform.position + endOffset, 0.2f);
        if (checkTransform.TryGetComponent(out MeshFilter meshFilter))
        {
            if (meshFilter.sharedMesh != null)
            {
                Gizmos.DrawWireMesh(meshFilter.sharedMesh, checkTransform.position + endOffset, checkTransform.rotation, checkTransform.lossyScale);
            }
        }
    }

    public Transform getInterfaceTransform()
    {
        return transform;
    }
}
