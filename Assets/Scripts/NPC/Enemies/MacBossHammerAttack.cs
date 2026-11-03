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

    [SerializeField] LayerMask layersToHit;

    [SerializeField] bool canDamage;

    void OnEnable()
    {
        startPos = transform.position;
        transform.position += Vector3.up * 3;
        startScale = transform.localScale;
        transform.localScale = Vector3.zero;

        transform.DOScale(startScale,0.2f).SetEase(Ease.OutBack);

        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOMoveY(transform.position.y + 2f, moveUpDuration).SetEase(Ease.OutExpo))
        .AppendInterval(pauseAtTopDuration)
        .AppendCallback(() => { canDamage = true; })
        .Append(transform.DOMoveY(startPos.y, moveDownDuration).SetEase(Ease.InExpo))
        .AppendInterval(0.1f)
        .AppendCallback(() => { 
            canDamage = false;
            Collider[] colliders = GetComponents<Collider>();
            foreach(var collider in colliders)
            {
                collider.isTrigger = false;
            }
                })
        .AppendInterval(pauseBeforeDestruction)
        .AppendCallback(() => {Destroy(gameObject);});
    }

    void Update()
    {
        if(canDamage)
        {
            
            BoxCollider boxCollider = GetComponent<BoxCollider>();
            Vector3 size = boxCollider.size * 0.5f;
            Collider[] detectedColliders = Physics.OverlapBox(transform.position, size, transform.rotation, layersToHit);
            foreach (var collider in detectedColliders)
            {
                TryApplyDamage(collider);
            }
        }
        
    }

    
    void TryApplyDamage(Collider collider)
    {
        Debug.Log("the hammer is trying to apply damage");
        if (canDamage)
        {
            if (collider.TryGetComponent(out IDamagable damagable))
            {
                damagable.TakeDamage(1, new AttackType[] { AttackType.Simple }, gameObject);
                canDamage = false;
            }
            if (collider.transform.parent != null && collider.transform.parent.TryGetComponent(out IDamagable parentDamagable))
            {
                parentDamagable.TakeDamage(1, new AttackType[] { AttackType.Simple }, gameObject);
                canDamage = false;

                if (collider.transform.parent.TryGetComponent(out CharacterMovement characterMovement))
                {
                    foreach(PlayerAbility playerAbility in characterMovement.playerAbilities)
                    {
                        DashAbility dashAbility = playerAbility as DashAbility;
                        if(dashAbility != null)
                        {
                            dashAbility.ApplyBonkFromOutside();
                        }
                    }
                }
            }
        }
    }



}
