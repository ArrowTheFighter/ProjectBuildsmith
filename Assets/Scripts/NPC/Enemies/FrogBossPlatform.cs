using DG.Tweening;
using UnityEngine;

public class FrogBossPlatform : MonoBehaviour
{
    public Vector3 startPos;
    public Vector3 startScale;

    public float topOffset;

    public bool is_broken;

    void Awake() 
    {
        startPos = transform.position;
        startScale = transform.localScale;
    }    
    

    public Vector3 getTopPos()
    {
        return transform.position + Vector3.up * topOffset;
    }

    public Vector3 getForward()
    {
        return transform.forward;
    }

    public void KnockDown()
    {
        transform.DOScale(Vector3.zero,0.25f).SetEase(Ease.InOutQuad);
        is_broken = true;
    }
    
    public void ResetPlatform()
    {
        if(!is_broken) return;

        transform.DOScale(startScale, 0.25f).SetEase(Ease.InOutQuad);
        is_broken = false;
    }
}
