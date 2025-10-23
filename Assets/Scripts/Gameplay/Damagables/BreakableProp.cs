using System;
using System.Linq;
using UnityEngine;

public class BreakableProps : MonoBehaviour, IDamagable, ISaveable
{
    public float Health;
    public float ItemDropForce = 10;
    public float RespawnTime;
    public float ExtraBounceForce;
    public GameObject HitParticle;
    public GameObject DestoryedParticle;
    public Vector3 DestroyedParticleOffset;
    public LootTable[] Loot;
    [Header("Audio")]
    public AudioCollection[] TakeDamageSoundEffects;
    public AudioClip destroyedSoundFX;
    public float destroyedSoundFXVolume;
    public float destroyedSoundFXPitch;
    public AttackType[] AttackTypes;


    public bool PlayerCanStomp { get; set; }

    bool broken;
    public int unique_id;

    public int Get_Unique_ID { get => unique_id; set { unique_id = value; } }

    public bool Get_Should_Save => broken;

    void OnEnable()
    {
        broken = false;
    }


    public void TakeDamage(float amount, AttackType[] attackTypes, GameObject source)
    {
        if (!hasAttackType(attackTypes)) return;
        PlayParticle();
        Health -= amount;
        SoundFXManager.instance.PlayRandomSoundCollection(transform, TakeDamageSoundEffects);
        if (Health <= 0)
        {
            Die();
        }
    }

    public void TakeDamage(float amount, AttackType[] attackTypes, GameObject source, out float ExtraForce)
    {
        ExtraForce = 0;
        if (!hasAttackType(attackTypes)) return;
        TakeDamage(amount, attackTypes, source);
        ExtraForce = ExtraBounceForce;
    }

    bool hasAttackType(AttackType[] attackTypes)
    {
        if (AttackTypes.Length <= 0) return true;
        foreach (AttackType type in attackTypes)
        {
            if (AttackTypes.Contains(type)) return true;
        }
        return false;
     }

    public void Die()
    {
        if (destroyedSoundFX != null)
        {
            SoundFXManager.instance.PlaySoundFXClip(destroyedSoundFX, transform, destroyedSoundFXVolume, destroyedSoundFXPitch);
        }

        foreach (LootTable lootTable in Loot)
        {
            int amount = lootTable.GetRandomDropAmount();
            ItemData itemData = GameplayUtils.instance.GetItemDataByID(lootTable.itemID);
            for (int i = 0; i < amount; i++)
            {
                GameObject itemDropped = Instantiate(itemData.item_pickup_object, transform.position + Vector3.up, Quaternion.identity);
                ItemPickup itemPickup = itemDropped.GetComponent<ItemPickup>();

                itemPickup.amount = 1;
                itemPickup.respawn_time = -1;
                Rigidbody rigidbody = itemDropped.GetComponent<Rigidbody>();
                rigidbody.useGravity = true;
                Vector3 horDir = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0, UnityEngine.Random.Range(-1f, 1f)).normalized;
                rigidbody.AddForce((horDir + Vector3.up) * ItemDropForce * UnityEngine.Random.Range(1f, 1.25f), ForceMode.Impulse);

                itemDropped.transform.eulerAngles = new Vector3(itemDropped.transform.eulerAngles.x, UnityEngine.Random.Range(0f,360f), itemDropped.transform.eulerAngles.z);
            }

        }
        if (DestoryedParticle != null)
        {
            Instantiate(DestoryedParticle, transform.position + DestroyedParticleOffset, Quaternion.identity);
        }

        if (RespawnTime <= 0)
        {
            broken = true;
            gameObject.SetActive(false);
        }
        else
        {
            broken = true;
            ItemRespawnManager.instance.item_respawns.Add(gameObject, RespawnTime);
            gameObject.SetActive(false);
        }
    }

    void PlayParticle()
    {
        if (HitParticle != null && HitParticle.TryGetComponent(out ParticleSystem particleSystem))
        {
            particleSystem.Play();
         }
    }

    public void TakeDamage(float amount, AttackType[] attackTypes, GameObject source, float knockbackStrength = 1)
    {
        //Does't need to do anything
    }

    public void SaveLoaded(SaveFileStruct saveFileStruct)
    {
        if (RespawnTime > 0)
        {
            ItemRespawnManager.instance.item_respawns.Add(gameObject, RespawnTime);
        }
        broken = true;
        gameObject.SetActive(false);
    }
}



[Serializable]
public class LootTable
{
    public string itemID;
    public int itemDropLow;
    public int ItemDropHigh;

    public int GetRandomDropAmount()
    {
        int amount = UnityEngine.Random.Range(itemDropLow, ItemDropHigh + 1);
        if (itemDropLow == ItemDropHigh) amount = itemDropLow;
        return amount;
    }
}
