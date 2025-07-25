using UnityEngine;
using DG.Tweening;
using UnityEditor.ShaderGraph;
using System;

public class FloatingObject : MonoBehaviour, IMoveingPlatform
{
    [SerializeField] Vector3 MoveTo;
    Vector3 startPos;
    Vector3 lastPosition;
    [SerializeField] float duration;
    [SerializeField] float speed;
    [SerializeField] float rotationMultiplier;
    bool reverse;
    Rigidbody rb;

    public event Action<Vector3> OnPlatformMove;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        rb = GetComponent<Rigidbody>();
        lastPosition = transform.position;
        //transform.DOMove(transform.position + MoveTo, duration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuad);
        if (rb != null)
        {
            startPos = transform.position;
            //rb.DOMove(transform.position + MoveTo, duration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuad);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        if (rb == null) return;
        if (!reverse)
        {
            float distanceToEnd = Vector3.Distance(startPos, (startPos + MoveTo));
            Vector3 dir = startPos - (startPos + MoveTo);
            rb.MovePosition(transform.position + dir.normalized * speed);
            if (Vector3.Distance(startPos, rb.position) > distanceToEnd) reverse = true;
        }
        else
        {
            float distanceToEnd = Vector3.Distance(startPos, (startPos + MoveTo));
            Vector3 dir = (startPos + MoveTo) - startPos;
            rb.MovePosition(transform.position + dir.normalized * speed);
            if (Vector3.Distance((startPos + MoveTo), transform.position) > distanceToEnd) reverse = false;
        }
    }

    void LateUpdate()
    {
        if (rb == null) return;
        Vector3 delta = rb.position - lastPosition;

        OnPlatformMove?.Invoke(delta);

        lastPosition = rb.position;

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
                Gizmos.DrawWireMesh(meshFilter.sharedMesh, transform.position + MoveTo,transform.rotation,transform.localScale);
             }
         }
    }
}
