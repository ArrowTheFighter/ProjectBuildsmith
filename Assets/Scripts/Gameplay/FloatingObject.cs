using UnityEngine;
using System;

public class FloatingObject : MonoBehaviour, IMoveingPlatform
{
    [SerializeField] Vector3 MoveTo;
    [SerializeField] Vector3 rotationAmount;
    [SerializeField] float duration;
    [SerializeField] AnimationCurve easing = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public bool IsActive;

    Vector3 startPos;
    Vector3 endPos;
    float elapsed;
    bool reverse;

    public event Action OnBeforePlatformMove;

    public Transform obj_transform;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (obj_transform == null) obj_transform = transform;
        startPos = obj_transform.position;
        endPos = startPos + MoveTo;

        //transform.DOMove(transform.position + MoveTo, duration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuad);
        // if (rb != null)
        // {
        //     startPos = transform.position;
        //     rb.DOMove(transform.position + MoveTo, duration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuad).SetUpdate(UpdateType.Fixed);
        // }
    }


    void FixedUpdate()
    {


        if (!IsActive) return;
        if (elapsed < duration)
        {
            OnBeforePlatformMove?.Invoke();
            elapsed += Time.fixedDeltaTime;


            float t = Mathf.Clamp01(elapsed / duration);
            float easedT = easing.Evaluate(t);
            Vector3 newPos = Vector3.Lerp(reverse ? startPos : endPos, reverse ? endPos : startPos, easedT);
            obj_transform.position = newPos;
            transform.Rotate(rotationAmount);
            //rb.MovePosition(newPos);

        }
        else
        {
            reverse = !reverse;
            elapsed = 0f;
        }
        

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
        Transform checkTransform = obj_transform;
        if (checkTransform == null) checkTransform = transform;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(checkTransform.position, checkTransform.position + MoveTo);
        Gizmos.DrawSphere(checkTransform.position + MoveTo, 0.2f);
        if (TryGetComponent(out MeshFilter meshFilter))
        {
            if (meshFilter.sharedMesh != null)
            {
                Gizmos.DrawWireMesh(meshFilter.sharedMesh, checkTransform.position + MoveTo, checkTransform.rotation, checkTransform.localScale);
             }
         }
    }

    public Transform getInterfaceTransform()
    {
        return obj_transform;
    }
}
