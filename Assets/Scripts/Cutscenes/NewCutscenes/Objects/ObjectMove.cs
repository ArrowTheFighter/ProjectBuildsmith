using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class ObjectMove : MonoBehaviour
{
    public Vector3 endOffset;
    public float duration;
    public Ease ease;
    public UnityEvent finishedEvent;

    public GameObject displayObj;


    public void StartMove()
    {
        transform.DOMove(transform.position + endOffset, duration).SetEase(ease).OnComplete(() => { finishedEvent?.Invoke(); });
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        

        if (displayObj != null && displayObj.TryGetComponent(out MeshFilter displayMeshFilter))
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
