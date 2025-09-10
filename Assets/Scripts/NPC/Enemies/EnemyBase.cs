using UnityEngine;
using System.Collections;
using System;

public abstract class EnemyBase : MonoBehaviour, IDamagable
{
    [Header("Player Search")]
    public float PlayerSearchRadius = 20f;
    public LayerMask playerLayerMask;
    [HideInInspector] public Action OnPlayerFound;
    [HideInInspector] public Action OnPlayerLost;
    [HideInInspector] public Action OnDeath;
    [HideInInspector] public Transform PlayerTransform;

    public bool EnemyActive;
    [Header("Health")]
    public int Health;
    public float extraBounceForce;
    public ParticleSystem onDamageParticles;
    public GameObject onDeathParticlesPrefab;

    [Header("Take Damage Audio")]
    public AudioClip takeDamageSoundFX;
    public float takeDamageSoundFXVolume = 1f;
    public float takeDamageSoundFXPitch = 1f;

    public bool CanBeStompedByPlayer;
    public bool PlayerCanStomp { get { return CanBeStompedByPlayer; } set { CanBeStompedByPlayer = value; } }

    //These are currently used for the tutorial quest for killing the mushroom, but they can probably be used for other quest objectives
    [SerializeField] public string flag_id;
    [SerializeField] public bool is_true;

    public IEnumerator CheckForPlayerRoutine(float checkDelay = 0.5f)
    {
        EnemyActive = true;
        while (EnemyActive)
        {
            CheckForPlayer();
            yield return new WaitForSeconds(checkDelay);
        }
    }

    public virtual void CheckForPlayer()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, PlayerSearchRadius, playerLayerMask);
        bool foundPlayer = false;
        foreach (Collider collider in hits)
        {
            if (collider.transform.parent != null && collider.transform.parent.GetComponent<CharacterMovement>() != null)
            {
                OnPlayerFound?.Invoke();
                
                foundPlayer = true;
                PlayerTransform = collider.transform.parent;
            }
        }
        if (!foundPlayer && PlayerTransform != null)
        {
            PlayerTransform = null;
            OnPlayerLost?.Invoke();
        }

    }

    public virtual void TakeDamage(int amount, AttackType[] attackTypes, GameObject source)
    {
        Health -= amount;
        if (onDamageParticles != null)
        {
            onDamageParticles.Play();
        }

        if (takeDamageSoundFX != null)
        {
            SoundFXManager.instance.PlaySoundFXClip(takeDamageSoundFX, transform, takeDamageSoundFXVolume, takeDamageSoundFXPitch);
        }

        if (Health <= 0)
        {
            Die();
        }
    }

    public virtual void TakeDamage(int amount, AttackType[] attackTypes, GameObject source, out float ExtraForce)
    {
        ExtraForce = extraBounceForce;
        TakeDamage(amount, attackTypes, source);
    }

    public virtual void TakeDamage(int amount, AttackType[] attackTypes, GameObject source, float knockbackStrength = 1)
    {
        return;
    }

    public virtual void Die()
    {
        if (onDeathParticlesPrefab != null)
        {
            Instantiate(onDeathParticlesPrefab, transform.position, Quaternion.identity);
        }
        OnDeath?.Invoke();

        if(flag_id != null)
        {
            is_true = true;
            FlagManager.Set_Flag(flag_id, is_true);
        }

        Destroy(gameObject);
    }
}