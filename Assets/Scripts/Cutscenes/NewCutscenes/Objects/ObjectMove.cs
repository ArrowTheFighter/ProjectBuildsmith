using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class ObjectMove : MonoBehaviour, ISkippable
{
    public Vector3 endOffset;
    Vector3 startPos;
    public Transform endTransform;
    public float duration;
    public Ease ease;
    public UnityEvent finishedEvent;

    public GameObject displayObj;

    void OnEnable()
    {
        startPos = transform.position;
    }

    public void Skip()
    {
        DOTween.Kill(transform);
        if (endTransform != null)
        {
            transform.position = endTransform.position;
        }
        else
        {
            transform.position = startPos + endOffset;
        }

        if (finishedEvent != null)
        {
            int count = finishedEvent.GetPersistentEventCount();
            for (int i = 0; i < count; i++)
            {
                var target = finishedEvent.GetPersistentTarget(i) as ISkippable;
                if (target != null)
                {
                    target.Skip();
                }
                else
                {
                    // if not ISkippable, just invoke normally
                    finishedEvent.Invoke();
                }
            }
        }
    }

    public void StartMove()
    {
        if (endTransform == null)
        {
            transform.DOMove(transform.position + endOffset, duration).SetEase(ease).SetUpdate(UpdateType.Fixed).OnComplete(() => { finishedEvent?.Invoke(); });
        }
        else
        {
            transform.DOMove(endTransform.position, duration).SetEase(ease).SetUpdate(UpdateType.Fixed).OnComplete(() => { finishedEvent?.Invoke(); });
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if (endTransform != null)
        {
            if (displayObj != null && displayObj.TryGetComponent(out MeshFilter displayMeshFilter))
            {
                print("has transform");
                Gizmos.DrawLine(displayObj.transform.position, endTransform.position);
                Gizmos.DrawSphere(endTransform.position, 0.2f);
                if (displayMeshFilter.sharedMesh != null)
                {
                    print("Drawing gizmo");
                    Gizmos.DrawWireMesh(displayMeshFilter.sharedMesh, endTransform.position, displayMeshFilter.transform.rotation, displayMeshFilter.transform.lossyScale);
                }
            }
        }   
        else if (displayObj != null && displayObj.TryGetComponent(out MeshFilter displayMeshFilter))
        {
            Gizmos.DrawLine(displayObj.transform.position, displayObj.transform.position + endOffset);
            Gizmos.DrawSphere(displayObj.transform.position + endOffset, 0.2f);
            if (displayMeshFilter.sharedMesh != null)
            {
                print("Drawing gizmo");
                Gizmos.DrawWireMesh(displayMeshFilter.sharedMesh, displayMeshFilter.transform.position + endOffset, displayMeshFilter.transform.rotation, displayMeshFilter.transform.lossyScale);
            }
        }
        else if(TryGetComponent(out MeshFilter meshFilter))
        {
            Gizmos.DrawLine(transform.position, transform.position + endOffset);
            Gizmos.DrawSphere(transform.position + endOffset, 0.2f);
            if (meshFilter.sharedMesh != null)
            {
                Gizmos.DrawWireMesh(meshFilter.sharedMesh, transform.position + endOffset, transform.rotation, transform.localScale);
            }
        }

    }
}
