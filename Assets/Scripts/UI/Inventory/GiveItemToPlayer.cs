using UnityEngine;

public class GiveItemToPlayer : MonoBehaviour
{
    [SerializeField] private string item_id;
    [SerializeField] private int amount;
    [SerializeField] private bool show_notification = true;
    public void GiveItem()
    {
        ScriptRefrenceSingleton.instance.gameplayUtils.add_items_to_inventory(item_id,amount, show_notification);
    }
}
