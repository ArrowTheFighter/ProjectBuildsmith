using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryItems : MonoBehaviour
{
    List<InventoryItemElement> inventoryItems = new List<InventoryItemElement>();
    [SerializeField] GameObject item_entry_holder;
    [SerializeField] GameObject item_entry_prefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Get all the item data
        ItemData[] allItems = Resources.LoadAll<ItemData>("ItemData");
        foreach (var data in allItems)
        {
            SpawnNewItemEntry(data);
        }
    }

    void SpawnNewItemEntry(ItemData itemData)
    {
        GameObject item_entry = Instantiate(item_entry_prefab,item_entry_holder.transform);

        RectTransform rectTransform = item_entry.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f,0.5f);
        rectTransform.anchorMax = new Vector2(0.5f,0.5f);

        rectTransform.anchoredPosition = new Vector2(0,0);

        InventoryItemElement itemElement = item_entry.GetComponent<InventoryItemElement>();
        itemElement.Set_Details(itemData);
        inventoryItems.Add(itemElement);
        item_entry.SetActive(false);

    }

    public void UpdateItemAmount(string item_id,int new_amount)
    {
        foreach(InventoryItemElement element in inventoryItems)
        {
            if(element.item_id == item_id)
            {
                element.Set_Amount(new_amount);

                if (element.item_id != "gold_coin")
                {
                    element.gameObject.SetActive(new_amount > 0);
                 }
            }
        }
    }

    public int GetItemAmount(string item_id)
    {
        foreach(InventoryItemElement element in inventoryItems)
        {
            if(element.item_id == item_id)
            {
                return element.item_amount;
            }
        }
        return -1;
    }

    public void AddToItemAmount(string item_id,int amount)
    {
        int current_amount = GetItemAmount(item_id);
        UpdateItemAmount(item_id,current_amount + amount);
    }



}
