using UnityEngine;
using DG.Tweening;
using UnityEditor.ShaderGraph;
using System;
using System.Xml.Serialization;

public class FloatingObject : MonoBehaviour, IMoveingPlatform
{
    [SerializeField] Vector3 MoveTo;
    [SerializeField] float duration;
    [SerializeField] AnimationCurve easing = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public bool IsActive;

    Vector3 startPos;
    Vector3 endPos;
    Vector3 lastPosition;
    float elapsed;
    bool reverse;
    Rigidbody rb;

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

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        if (!IsActive) return;
        if (rb == null) return;
        if (elapsed < duration)
        {
            elapsed += Time.fixedDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float easedT = easing.Evaluate(t);
            Vector3 newPos = Vector3.Lerp(reverse ? startPos : endPos, reverse ? endPos : startPos, easedT);
            rb.MovePosition(newPos);

        }
        else
        {
            reverse = !reverse;
            elapsed = 0f;
        }
        

        if (rb == null) return;
        Vector3 delta = rb.position - lastPosition;

        OnPlatformMove?.Invoke(delta);

        lastPosition = rb.position;
    }

    void LateUpdate()
    {
        // if (rb == null) return;
        // Vector3 delta = rb.position - lastPosition;

        // OnPlatformMove?.Invoke(delta);

        // lastPosition = rb.position;

    }

    public void SetActive(bool active)
    {
        IsActive = active;
     }

    public void SetPositionAsStart()
    {
        startPos = transform.position;
        endPos = startPos + MoveTo;
        elapsed = 0f;
        reverse = true;
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
