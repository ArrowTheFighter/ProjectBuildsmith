using TMPro;
using UnityEngine;

public class CraftingItemRecipeScript : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI Item_Name;
    [SerializeField] TextMeshProUGUI Item_Amount;
    string item_id;
    int item_amount;
    [SerializeField] TextMeshProUGUI[] item_requirements_name;
    [SerializeField] TextMeshProUGUI[] item_requirements_amount;
    item_requirement[] required_items;

    public void Set_Recipe_Details(Item_Recipe item_Recipe)
    {
        Item_Name.text = item_Recipe.Item_Name;
        item_amount = item_Recipe.Output_Amount;
        Item_Amount.text = item_Recipe.Output_Amount.ToString();
        item_id = item_Recipe.ID;
        required_items = item_Recipe.item_Requirements;
        for (int i = 0; i < item_Recipe.item_Requirements.Length; i++)
        {
            item_requirements_name[i].transform.parent.gameObject.SetActive(true);
            item_requirements_name[i].text = item_Recipe.item_Requirements[i].item_name;
            item_requirements_amount[i].text = "x" + item_Recipe.item_Requirements[i].item_amount;
        }
    }

    public void Craft_Item()
    {
        ItemData item = ScriptableObject.CreateInstance<ItemData>();
        ItemData[] allItems = Resources.LoadAll<ItemData>("ItemData");
        foreach (var data in allItems)
        {
            if(data.item_id == item_id)
            {
                item = data;
            }
        }
        if(item.item_id != item_id)
        {
            print("Item [" + item.item_name + "] doesn't exist");
            return;
        } 
        //Check if we have the reuired items;
        foreach(item_requirement required_item in required_items)
        {
            if(required_item.item_amount > ScriptRefrenceSingleton.instance.gameplayUtils.get_item_holding_amount(required_item.item_id))
            {
                ScriptRefrenceSingleton.instance.gameplayUtils.ShowCustomNotif("Not enough resources");
                print("Not enough items to craft " + item.item_name);
                return;
            }
        }
        ScriptRefrenceSingleton.instance.gameplayUtils.add_items_to_inventory(item_id,item_amount);
        foreach(item_requirement required_item in required_items)
        {
            ScriptRefrenceSingleton.instance.gameplayUtils.remove_items_from_inventory(required_item.item_id,required_item.item_amount);
        }


    }
}


