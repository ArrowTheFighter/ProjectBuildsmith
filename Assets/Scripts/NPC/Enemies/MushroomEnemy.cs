using System.Collections;
using UnityEngine;

public class MushroomEnemy : MonoBehaviour
{
    [SerializeField] float searchRadius;
    [SerializeField] float attackRadius;
    [SerializeField] LayerMask layerMask;
    [SerializeField] float gazeHeight = 0.5f;
    [SerializeField] float attackHeight = 1.5f;
    [SerializeField] float AttackDelay;
    [SerializeField] float AttackAnimationDelay = 0.2f;
    [SerializeField] ParticleSystem AttackRadiusParticles;
    [SerializeField] ParticleSystem AttackParticles;
    float AttackCooldown;

    bool EnemyActive = true;
    Transform playerTransform;
    Vector3 startPos;
    Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPos = transform.position;
        StartCoroutine(CheckForPlayerRoutine());
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerTransform != null)
        {
            transform.position = Vector3.Lerp(transform.position, startPos + Vector3.up * gazeHeight, 0.1f);

            Vector3 dirToPlayer = playerTransform.position - transform.position;
            dirToPlayer.y = 0;
            Quaternion lookQuat = Quaternion.LookRotation(dirToPlayer, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookQuat, 0.1f);

            if (Time.time > AttackCooldown)
            {
                AttackRadiusParticles.Play();
                StartCoroutine(attackDelay());
                SetAttackCooldown();
            }
        }
        else
        {

            transform.position = Vector3.Lerp(transform.position, startPos, 0.1f);
        }
    }

    IEnumerator attackDelay()
    {
        yield return new WaitForSeconds(AttackAnimationDelay);
        animator.SetTrigger("Attack");
    }

    IEnumerator CheckForPlayerRoutine()
    {
        EnemyActive = true;
        while (EnemyActive)
        {
            CheckForPlayer();
            yield return new WaitForSeconds(0.5f);
        }
    }

    void SetAttackCooldown()
    {
        AttackCooldown = Time.time + AttackDelay;
    }

    void CheckForPlayer()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, searchRadius, layerMask);
        bool foundPlayer = false;

        foreach (Collider collider in hits)
        {
            if (collider.transform.parent != null && collider.transform.parent.GetComponent<CharacterMovement>() != null)
            {
                if (playerTransform == null)
                {
                    SetAttackCooldown();
                }
                foundPlayer = true;
                playerTransform = collider.transform.parent;
            }
        }
        if (!foundPlayer)
            playerTransform = null;

    }

    public void AttackDamage()
    {
        if (playerTransform != null && playerTransform.TryGetComponent(out PlayerHealth playerHealth))
        {
            if (Mathf.Abs(playerTransform.position.y - transform.position.y) < attackHeight && Vector3.Distance(playerTransform.position, transform.position) < attackRadius)
            {
                playerHealth.TakeDamage(1, gameObject,1);
            }
        }
    }

    public void ShowAttackParticles()
    {
        AttackParticles.Play();
     }
}
