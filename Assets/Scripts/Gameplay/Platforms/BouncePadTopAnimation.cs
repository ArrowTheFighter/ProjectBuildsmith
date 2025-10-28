using System.Collections;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class BouncePadTopAnimation : MonoBehaviour
{
    public float startOffset;
    public float topOffset;
    Vector3 startLocalPos;

    public Ease InitalEase;
    public float InitalDuration = 0.1f;

    public Ease ReturnEase;
    public float ReturnDuration = 0.2f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(setupPad());
    }
    
    IEnumerator setupPad()
    {
        yield return null;
        startLocalPos = transform.localPosition;
        transform.position += new Vector3(0, startOffset, 0);
    }

    [Button]
    public void Bounce()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOLocalMoveY(startLocalPos.y + topOffset, InitalDuration).SetEase(InitalEase))
            .Append(transform.DOLocalMoveY(startLocalPos.y + startOffset, ReturnDuration).SetEase(ReturnEase));
    }
}
