using System;
using System.Linq;
using UnityEngine;


public class ToolCraftingTable : CraftingTableBase
{
    bool CheckingForRecipe = true;
    void Start()
    {
        foreach (InventorySlotComponent slotComponent in craftingTableSlots)
        {
            slotComponent.slotEmptied += (context) => { InventoryUpdated(); };
            slotComponent.slotFilled += InventoryUpdated;
        }
        outputSlot.slotEmptied += itemCrafted;
        foreach (CraftingRecipeData recipeData in GameplayUtils.instance.RecipeDatabase.recipes)
        {
            if (recipeData.stationType == craftingStationType)
            {
                craftingRecipeData.Add(recipeData);
            }
        }
    }

    public override void InventoryUpdated()
    {
        if (!CheckingForRecipe) return;
        print("inventory was updated");
        if (IsValidRecipe(out CraftingRecipeData validRecipe))
        {
            ItemData OutputItem = GameplayUtils.instance.GetItemDataByID(validRecipe.recipe_output_id);
            GameplayUtils.instance.inventoryManager.AddItemToSlot(outputSlot.inventorySlot, OutputItem, 1);
            print("found valid recipe!");
        }
        else
        {
            print("no valid recipe!");
            outputSlot.RemoveItemFromSlot(false,false);
        }
    }

    bool IsValidRecipe(out CraftingRecipeData validRecipeData)
    {
        validRecipeData = null;
        foreach (CraftingRecipeData recipeData in craftingRecipeData)
        {

            bool recipeIsValid = true;
            for (int i = 0; i < craftingTableSlots.Length; i++)
            {
                if (string.IsNullOrEmpty(recipeData.recipe_items[i]) && string.IsNullOrEmpty(craftingTableSlots[i].inventorySlot.inventoryItemStack.ID))
                {
                    continue;
                }
                if (recipeData.recipe_items[i] != craftingTableSlots[i].inventorySlot.inventoryItemStack.ID) recipeIsValid = false;
            }
            if (recipeIsValid)
            {
                validRecipeData = recipeData;
                return true;
            }
        }
        return false;
     }

    void itemCrafted(InventoryItemStack inventoryItemStack)
    {
        CheckingForRecipe = false;
        foreach (InventorySlotComponent slotComponent in craftingTableSlots)
        {
            if (slotComponent.inventorySlot.inventoryItemStack.Amount > 1)
            {
                slotComponent.inventorySlot.inventoryItemStack.Amount -= 1;
                slotComponent.SetSlotFilled(slotComponent.inventorySlot.inventoryItemStack.Amount);
            }
            else
            {
                slotComponent.RemoveItemFromSlot(false);
            }
        }
        CheckingForRecipe = true;
        GameplayUtils.instance.AddItemCraftedAmount(inventoryItemStack.ID, inventoryItemStack.Amount);
        InventoryUpdated();
        print("item was crafted");
    }
}