using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;


public class ToolCraftingTable : CraftingTableBase
{
    bool CheckingForRecipe = true;
    public UnityEvent OnItemCraftedEvent;
    void Start()
    {
        foreach (InventorySlotComponent slotComponent in craftingTableSlots)
        {
            slotComponent.slotEmptied += (context) => { InventoryUpdated(); };
            slotComponent.slotFilled += InventoryUpdated;
        }
        outputSlot.slotEmptied += itemCrafted;
        foreach (CraftingRecipeData recipeData in ScriptRefrenceSingleton.instance.gameplayUtils.RecipeDatabase.recipes)
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
            ItemData OutputItem = ScriptRefrenceSingleton.instance.gameplayUtils.GetItemDataByID(validRecipe.recipe_output_id);
            ScriptRefrenceSingleton.instance.gameplayUtils.inventoryManager.AddItemToSlot(outputSlot.inventorySlot, OutputItem, 1);
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

        // Build 3x3 grid from crafting table
        string[,] grid = new string[3, 3];

        for (int i = 0; i < 9; i++)
        {
            int y = i / 3;
            int x = i % 3;
            grid[y, x] = craftingTableSlots[i]
                .inventorySlot.inventoryItemStack.ID;
        }

        foreach (CraftingRecipeData recipe in craftingRecipeData)
        {
            // Make sure pattern exists
            if (recipe.pattern == null)
                continue;

            int maxY = 3 - recipe.height;
            int maxX = 3 - recipe.width;

            for (int y = 0; y <= maxY; y++)
            {
                for (int x = 0; x <= maxX; x++)
                {
                    if (PatternMatches(grid, recipe, y, x) &&
                        !HasExtraItems(grid, recipe, y, x))
                    {
                        validRecipeData = recipe;
                        return true;
                    }
                }
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
        ScriptRefrenceSingleton.instance.gameplayUtils.AddItemCraftedAmount(inventoryItemStack.ID, inventoryItemStack.Amount);
        InventoryUpdated();
        OnItemCraftedEvent?.Invoke();
        print("item was crafted");
    }

    bool PatternMatches(
    string[,] grid,
    CraftingRecipeData recipe,
    int startY,
    int startX)
    {
        for (int y = 0; y < recipe.height; y++)
        {
            for (int x = 0; x < recipe.width; x++)
            {
                string recipeItem = recipe.pattern[y, x];
                string gridItem = grid[startY + y, startX + x];

                if (string.IsNullOrEmpty(recipeItem))
                {
                    if (!string.IsNullOrEmpty(gridItem))
                        return false;
                }
                else if (recipeItem != gridItem)
                {
                    return false;
                }
            }
        }

        return true;
    }

    bool HasExtraItems(
    string[,] grid,
    CraftingRecipeData recipe,
    int startY,
    int startX)
    {
        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 3; x++)
            {
                bool inside =
                    y >= startY && y < startY + recipe.height &&
                    x >= startX && x < startX + recipe.width;

                if (!inside && !string.IsNullOrEmpty(grid[y, x]))
                    return true;
            }
        }
        return false;
    }


}