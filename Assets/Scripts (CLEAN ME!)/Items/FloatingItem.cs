using UnityEngine;
using DG.Tweening;
public class FloatingItem : MonoBehaviour
{
    [SerializeField] float upwards_amount = 0.5f;
    [SerializeField] float animation_duration_float = 2;
    [SerializeField] float animation_duration_spin = 2;
    [SerializeField] Ease ease_type = Ease.InOutQuad;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(transform.childCount <= 0) return;
        transform.GetChild(0).DOLocalMoveY(upwards_amount,animation_duration_float * 0.5f).SetLoops(-1,LoopType.Yoyo).SetEase(ease_type).SetLink(gameObject);
        transform.GetChild(0).DOLocalRotate(new Vector3(0,360,0),animation_duration_spin,RotateMode.LocalAxisAdd).SetLoops(-1).SetEase(Ease.Linear).SetLink(gameObject);
    }

}
