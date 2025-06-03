using TMPro;
using UnityEngine;

public class InventoryItemElement : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI item_name_UI;
    [SerializeField] TextMeshProUGUI item_amount_UI;
    public string item_id;
    public int item_amount;
    public void Set_Details(ItemData itemData)
    {
        item_id = itemData.item_id;

        item_name_UI.text = itemData.item_name;

        item_amount_UI.text = "0";
        item_amount = 0;
    }

    public void Set_Amount(int new_amount)
    {
        item_amount = new_amount;
        item_amount_UI.text = item_amount.ToString();
    }
}
