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
    float startHealth;
    public float Health;
    public float extraBounceForce;
    public ParticleSystem onDamageParticles;
    public GameObject onDeathParticlesPrefab;
    public float RespawnTime;

    [Header("Loot")]
    public LootTable[] Loot;
    public float ItemDropForce = 10;
    public Vector3 spawnOffset;

    [Header("Take Damage Audio")]
    public AudioCollection[] takeDamageCollection;
    public AudioCollection[] DeathAudioCollection;
    public AudioClip takeDamageSoundFX;
    public float takeDamageSoundFXVolume = 1f;
    public float takeDamageSoundFXPitch = 1f;

    public bool CanBeStompedByPlayer;
    public bool PlayerCanStomp { get { return CanBeStompedByPlayer; } set { CanBeStompedByPlayer = value; } }
    public bool CanTakeDamage = true;

    //These are currently used for the tutorial quest for killing the mushroom, but they can probably be used for other quest objectives
    [SerializeField] public string flag_id;
    [SerializeField] public bool is_true;

    void Awake()
    {
        startHealth = Health;
    }

    void OnEnable()
    {
        Health = startHealth;
    }

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

    public virtual void TakeDamage(float amount, AttackType[] attackTypes, GameObject source)
    {
        if (!CanTakeDamage) return;
        Health -= amount;
        if (onDamageParticles != null)
        {
            onDamageParticles.Play();
        }

        

        if (Health <= 0)
        {
            Die();
        }
        else
        {
            ScriptRefrenceSingleton.instance.soundFXManager.PlayRandomSoundCollection(transform, takeDamageCollection);
        }
    }

    public virtual void TakeDamage(float amount, AttackType[] attackTypes, GameObject source, out float ExtraForce)
    {
        ExtraForce = extraBounceForce;
        if (!CanTakeDamage) return;
        TakeDamage(amount, attackTypes, source);
    }

    public virtual void TakeDamage(float amount, AttackType[] attackTypes, GameObject source, float knockbackStrength = 1)
    {
        if (!CanTakeDamage) return;
        return;
    }

    public virtual void Die()
    {
        if (onDeathParticlesPrefab != null)
        {
            Instantiate(onDeathParticlesPrefab, transform.position, Quaternion.identity);
        }
        OnDeath?.Invoke();

        if (flag_id != null)
        {
            is_true = true;
            FlagManager.Set_Flag(flag_id, is_true);
        }

        foreach (LootTable lootTable in Loot)
        {
            int amount = lootTable.GetRandomDropAmount();
            ItemData itemData = ScriptRefrenceSingleton.instance.gameplayUtils.GetItemDataByID(lootTable.itemID);
            for (int i = 0; i < amount; i++)
            {
                GameObject itemDropped = Instantiate(itemData.item_pickup_object, transform.position + Vector3.up + spawnOffset, Quaternion.identity);
                ItemPickup itemPickup = itemDropped.GetComponent<ItemPickup>();

                itemPickup.amount = 1;
                itemPickup.respawn_time = -1;
                Rigidbody rigidbody = itemDropped.GetComponent<Rigidbody>();
                rigidbody.useGravity = true;
                Vector3 horDir = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0, UnityEngine.Random.Range(-1f, 1f)).normalized;
                rigidbody.AddForce((horDir + Vector3.up) * ItemDropForce * UnityEngine.Random.Range(1f, 1.25f), ForceMode.Impulse);
            }

        }

        if (RespawnTime <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            ScriptRefrenceSingleton.instance.itemRespawnManager.item_respawns.Add(gameObject, Time.time + RespawnTime);
            gameObject.SetActive(false);
        }

        ScriptRefrenceSingleton.instance.soundFXManager.PlayRandomSoundCollection(transform, DeathAudioCollection);

    }
}