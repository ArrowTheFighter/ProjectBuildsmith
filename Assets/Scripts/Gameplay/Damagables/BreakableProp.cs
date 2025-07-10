using System;
using UnityEngine;

public class BreakableProps : MonoBehaviour, IDamagable
{
    public int Health;
    public float ItemDropForce = 10;
    public float RespawnTime;
    public float ExtraBounceForce;
    public GameObject HitParticle;
    public GameObject DestoryedParticle;
    public Vector3 DestroyedParticleOffset;
    public LootTable[] Loot;

    public void TakeDamage(int amount, GameObject source)
    {
        PlayParticle();
        Health -= amount;
        if (Health <= 0)
        {
            Die();
        }
    }

    public void TakeDamage(int amount, GameObject source, out float ExtraForce)
    {
        TakeDamage(amount, source);
        ExtraForce = ExtraBounceForce;
    }

    public void Die()
    {
        foreach (LootTable lootTable in Loot)
        {
            int amount = lootTable.GetRandomDropAmount();
            ItemData itemData = GameplayUtils.instance.GetItemDataByID(lootTable.itemID);
            GameObject itemDropped = Instantiate(itemData.item_pickup_object, transform.position + Vector3.up, Quaternion.identity);
            ItemPickup itemPickup = itemDropped.GetComponent<ItemPickup>();

            itemPickup.amount = amount;
            itemPickup.respawn_time = -1;
            Rigidbody rigidbody = itemDropped.GetComponent<Rigidbody>();
            rigidbody.useGravity = true;
            Vector3 horDir = new Vector3(UnityEngine.Random.Range(-1, 1), 0, UnityEngine.Random.Range(-1, 1)).normalized;
            rigidbody.AddForce((horDir + Vector3.up) * ItemDropForce, ForceMode.Impulse);
        }
        if (DestoryedParticle != null)
        {
            Instantiate(DestoryedParticle, transform.position + DestroyedParticleOffset, Quaternion.identity);
        }
        if (RespawnTime <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
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
