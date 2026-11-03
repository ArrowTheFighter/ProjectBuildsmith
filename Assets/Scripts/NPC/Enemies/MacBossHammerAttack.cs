using UnityEngine;
using DG.Tweening;

public class MacBossHammerAttack : MonoBehaviour
{
    Vector3 startScale;
    Vector3 startPos;

    [SerializeField] float moveUpDuration = 1;
    [SerializeField] float pauseAtTopDuration = 0.5f;
    [SerializeField] float moveDownDuration = 0.5f;
    [SerializeField] float pauseBeforeDestruction = 1f;

    bool canDamage;

    void OnEnable()
    {
        startPos = transform.position;
        transform.position += Vector3.up * 3;
        startScale = transform.localScale;
        transform.localScale = Vector3.zero;

        transform.DOScale(startScale,0.2f).SetEase(Ease.OutBack);

        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOMoveY(transform.position.y + 2f, moveUpDuration)).SetEase(Ease.OutExpo)
        .AppendInterval(pauseAtTopDuration)
        .AppendCallback(() => { canDamage = true; })
        .Append(transform.DOMoveY(startPos.y, moveDownDuration)).SetEase(Ease.InExpo)
        .AppendInterval(moveDownDuration)
        .AppendCallback(() => { canDamage = false; })
        .AppendInterval(pauseBeforeDestruction)
        .AppendCallback(() => {Destroy(gameObject);});
    }

    void OnTriggerEnter(Collider other)
    {
        if(canDamage)
        {
            if(other.TryGetComponent(out IDamagable damagable))
            {
                damagable.TakeDamage(1, new AttackType[] { AttackType.Simple } ,gameObject,1);
                canDamage = false;
            }
            if (other.transform.parent != null && other.transform.parent.TryGetComponent(out IDamagable parentDamagable))
            {
                parentDamagable.TakeDamage(1, new AttackType[] { AttackType.Simple }, gameObject,1);
                canDamage = false;
            }
        }
    }



}
