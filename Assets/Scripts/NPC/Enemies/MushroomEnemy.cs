using System.Collections;
using UnityEngine;

public class MushroomEnemy : EnemyBase, IDamagable
{
    [SerializeField] float searchRadius;
    [SerializeField] float attackRadius;
    [SerializeField] float gazeHeight = 0.5f;
    [SerializeField] float attackHeight = 1.5f;
    [SerializeField] float AttackDelay;
    [SerializeField] float AttackAnimationDelay = 0.2f;
    [SerializeField] ParticleSystem AttackRadiusParticles;
    [SerializeField] ParticleSystem AttackParticles;

    float AttackCooldown;

    //bool EnemyActive = true;
    Vector3 startPos;
    Animator animator;

    [Header("Audio")]
    [SerializeField] AudioClip attackSoundFX;
    [SerializeField] float attackSoundFXVolume = 1f;
    [SerializeField] float attackSoundFXPitch = 1f;

    [SerializeField] AudioClip playerSpottedSoundFX;
    [SerializeField] float playerSpottedSoundFXVolume = 1f;
    [SerializeField] float playerSpottedSoundFXPitch = 1f;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPos = transform.position;
        
        animator = GetComponent<Animator>();

        OnPlayerFound += PlayerFound;
    }

    void OnEnable()
    {
        PlayerTransform = null;
        StartCoroutine(CheckForPlayerRoutine());
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerTransform != null)
        {
            transform.position = Vector3.Lerp(transform.position, startPos + Vector3.up * gazeHeight, 0.1f);

            Vector3 dirToPlayer = PlayerTransform.position - transform.position;
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


    void SetAttackCooldown()
    {
        AttackCooldown = Time.time + AttackDelay;
    }

    void PlayerFound()
    {
        if (PlayerTransform == null)
        {
            PlayPlayerSpottedAudio();
            SetAttackCooldown();
        }
    }


    public void AttackDamage()
    {
        if (PlayerTransform != null && PlayerTransform.TryGetComponent(out PlayerHealth playerHealth))
        {
            if (Mathf.Abs(PlayerTransform.position.y - transform.position.y) < attackHeight && Vector3.Distance(PlayerTransform.position, transform.position) < attackRadius)
            {
                playerHealth.TakeDamage(1, new AttackType[] { AttackType.Simple }, gameObject, 1);
            }
        }
    }

    public void ShowAttackParticles()
    {
        if (attackSoundFX != null)
        {
            SoundFXManager.instance.PlaySoundFXClip(attackSoundFX, transform, attackSoundFXVolume, attackSoundFXPitch);
        }

        AttackParticles.Play();
    }

    public void PlayPlayerSpottedAudio()
    {
        SoundFXManager.instance.PlaySoundFXClip(playerSpottedSoundFX, transform, playerSpottedSoundFXVolume, playerSpottedSoundFXPitch);
    }


    
}
