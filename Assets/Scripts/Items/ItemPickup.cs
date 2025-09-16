using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ItemPickup : MonoBehaviour
{
    [SerializeField] LayerMask player_layer;

    [SerializeField] string item_id;
    [SerializeField] public int amount = 1;
    [SerializeField] bool show_notification = true;
    [SerializeField] bool isSpecialItem;
    [SerializeField] float cantPickupDelay = 0.5f;
    float cantPickupTime;
    [SerializeField] public float respawn_time = -1;
    [SerializeField] public float despawn_time = -1;
    float time_to_despawn;
    bool markedAsDestoryed;
    public LayerMask layersToIgnore;
    public UnityEvent onPickedUp;

    void Awake()
    {
        cantPickupTime = Time.time + cantPickupDelay;
        if (respawn_time == -1 && despawn_time != -1)
        {
            StartCoroutine(DespawnItem());
        }
    }

    IEnumerator DespawnItem()
    {
        yield return new WaitForSeconds(despawn_time);
        if (TryGetComponent(out PlayerCheckpointPosition component))
        {
            component.SetPlayerToCheckpointPosition();
        }
        else
        {
            Destroy(gameObject);
         }
    }

    public void OnTriggerEnter(Collider other)
    {
        //Checks to see if the player enters a pickup's trigger
        //Destroys the pickup
        if ((player_layer.value & (1 << other.gameObject.layer)) != 0)
        {

            // Pickup();

        }
    }

    void OnEnable()
    {
        markedAsDestoryed = false;
    }

    public void Pickup()
    {
        if (markedAsDestoryed)
        {
            return;
        }
        if (cantPickupTime > Time.time) return;
        if (isSpecialItem)
        {
            GameplayUtils.instance.inventoryManager.AddSpecialItem(item_id, amount);
            GameplayUtils.instance.itemPickupNotifcationScript.PlayNotificationSound();
            PickupComplete();
            return;
        }
        int loosePieces = GameplayUtils.instance.add_items_to_inventory(item_id, amount, show_notification,true);
        if (loosePieces == -1) return;
        //GameplayUtils.instance.Play_Audio_On_Player(2, 0.5f);
        if (loosePieces > 0)
        {
            amount = loosePieces;
        }
        else
        {
            PickupComplete();
        }
    }

    void PickupComplete()
    {
        onPickedUp?.Invoke();
        if (respawn_time <= 0)
        {
            markedAsDestoryed = true;
            Destroy(gameObject);
        }
        else
        {
            markedAsDestoryed = true;
            ItemRespawnManager.instance.item_respawns.Add(gameObject, Time.time + respawn_time);
            gameObject.SetActive(false);
        }
    }


    void OnCollisionEnter(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & layersToIgnore) == 0)
        {
            GetComponent<Rigidbody>().linearDamping = 5;
        }
        
    }
}