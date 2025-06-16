using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [SerializeField] LayerMask player_layer;

    [SerializeField] string item_id;
    [SerializeField] int amount = 1;
    [SerializeField] bool show_notification = true;
    [SerializeField] float respawn_time = -1;

    public void OnTriggerEnter(Collider other)
    {
        //Checks to see if the player enters a pickup's trigger
        //Checks to see what type of resource it is and increases the amount of it
        //Destroys the pickup
        if ((player_layer.value & (1 << other.gameObject.layer)) != 0)
        {
            GameplayUtils.instance.add_items_to_inventory(item_id, amount, show_notification);
            GameplayUtils.instance.Play_Audio_On_Player(2, 0.5f);

            if (respawn_time <= 0)
            {
                Destroy(gameObject);
            }
            else
            {
                ItemRespawnManager.instance.item_respawns.Add(gameObject, Time.time + respawn_time);
                gameObject.SetActive(false);
            }
        }
    }
}