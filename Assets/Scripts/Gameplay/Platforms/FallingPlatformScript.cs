using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;

public class FallingPlatformScript : MonoBehaviour, IMoveingPlatform
{
    public event Action<Vector3> OnPlatformMove;
    public event Action OnBeforePlatformMove;
    public event Action OnAfterPlatformMove;

    Collider platformCollider;
    Vector3 startPos;
    Vector3 startingScale;
    public float fallTime;
    public float fallDistance;

    public float scaleDownTime;
    public AnimationCurve fallCurve;
    bool isFalling;
    Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPos = transform.position;
        startingScale = transform.localScale;
        platformCollider = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator Falling()
    {
        if (isFalling) yield break;
        isFalling = true;
        for (float i = 0; i < fallTime; i += Time.fixedDeltaTime)
        {
            OnBeforePlatformMove?.Invoke();
            float t = Mathf.InverseLerp(0, fallTime, i);
            float distance = Mathf.Lerp(0, fallDistance, fallCurve.Evaluate(t));

            rb.MovePosition(startPos + Vector3.down * distance);
            //transform.position = startPos + Vector3.down * distance;

            OnAfterPlatformMove?.Invoke();
            yield return new WaitForFixedUpdate();
        }
        yield return new WaitForSeconds(1);

        bool isScaling = true;
        transform.DOScale(0, scaleDownTime).SetEase(Ease.InQuad).OnComplete(() =>
        {
            isScaling = false;
        });
        yield return new WaitUntil(() => !isScaling);
        yield return new WaitForSeconds(1);

        transform.position = startPos;
        isScaling = true;
        transform.DOScale(startingScale, 0.5f).SetEase(Ease.InOutQuad).OnComplete(() =>
        {
            isScaling = false;
        });
        yield return new WaitUntil(() => !isScaling);
        isFalling = false;
        //TODO probably should change this when re doing moving platforms
        OnPlatformMove?.Invoke(Vector3.zero);
    }

    void OnCollisionEnter(Collision collision)
    {
        float heightCheck = transform.position.y + platformCollider.bounds.extents.y;
        if (collision.gameObject.tag == "Player" && collision.transform.position.y > heightCheck)
        {
            print("Player landed on us");
            StartCoroutine(Falling());
        }
    }

    public Transform getInterfaceTransform()
    {
        return transform;
    }
}
