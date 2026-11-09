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
    [SerializeField] LayerMask groundLayers;

    [SerializeField] bool canDamage;
    [SerializeField] GameObject hammerSlamAudioObject;

    void OnEnable()
    {
        
        float distance = 0f;
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitInfo, 10, groundLayers))
        {
            BoxCollider boxCollider = GetComponent<BoxCollider>();
            float height = boxCollider.size.y * 0.5f;
            distance = hitInfo.distance - height;
        }

        startPos = transform.position;
        transform.position += Vector3.up * 3;
        startScale = transform.localScale;
        transform.localScale = Vector3.zero;

        transform.DOScale(startScale,0.2f).SetEase(Ease.OutBack);

        

        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOMoveY(transform.position.y + 2f, moveUpDuration).SetEase(Ease.OutExpo))
        .AppendInterval(pauseAtTopDuration)
        .AppendCallback(() => { canDamage = true; })
        .Append(transform.DOMoveY(startPos.y - distance, moveDownDuration).SetEase(Ease.InExpo).OnComplete(() => { hammerSlamAudioObject.SetActive(true);}))
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
        .Append(transform.DOScale(0,0.2f).SetEase(Ease.OutExpo))
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
        if(collider.isTrigger) return;
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
