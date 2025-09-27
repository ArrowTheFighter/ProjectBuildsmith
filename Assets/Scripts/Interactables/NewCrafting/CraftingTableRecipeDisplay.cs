using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingTableRecipeDisplay : MonoBehaviour
{
    public RecipesBookManager recipesBookManager;
    public CraftingStationTypes craftingStationType;
    public List<InventorySlotComponent> slots = new List<InventorySlotComponent>();
    List<GameObject> SpawnedObjs = new List<GameObject>();
    public List<CraftingRecipeData> craftingRecipes = new List<CraftingRecipeData>();
    int currentRecipe;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameplayUtils.instance.craftingTableRecipeDisplays.Add(this);
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).TryGetComponent(out InventorySlotComponent inventorySlotComponent))
            {
                slots.Add(inventorySlotComponent);
            }
        }
        foreach (CraftingRecipeData recipeData in GameplayUtils.instance.RecipeDatabase.recipes)
        {
            if (recipeData.stationType == craftingStationType)
            {
                craftingRecipes.Add(recipeData);
            }
        }
    }


    public void ShowRecipe(string recipe_id)
    {
        print($"showing recipe {recipe_id}");
        HideRecipe();
        CraftingRecipeData recipeData = GetRecipe(recipe_id);
        if (recipeData == null)
        {
            Debug.LogError("Couldn't find the supplied recipe id");
            return;
        }
        for (int i = 0; i < recipeData.recipe_items.Count; i++)
        {
            if (recipeData.recipe_items[i] == "") continue;
            ItemData itemData = GameplayUtils.instance.GetItemDataByID(recipeData.recipe_items[i]);
            GameObject imageObj = slots[i].transform.GetChild(1).GetComponentInChildren<Image>().gameObject;
            GameObject spawnedObj = Instantiate(imageObj, imageObj.transform.parent);
            spawnedObj.GetComponent<Image>().sprite = itemData.item_ui_image;
            spawnedObj.GetComponent<Image>().color = new Color(1, 1, 1, 0.3f);

            SpawnedObjs.Add(spawnedObj);
        }
    }

    [ContextMenu("Hide Recipe")]
    public void HideRecipe()
    {
        for (int i = SpawnedObjs.Count - 1; i >= 0; i--)
        {
            Destroy(SpawnedObjs[i]);
            SpawnedObjs.RemoveAt(i);
        }
    }

    CraftingRecipeData GetRecipe(string recipe_id)
    {
        foreach (CraftingRecipeData recipeData in craftingRecipes)
        {
            if (recipeData.recipe_id == recipe_id)
            {
                return recipeData;
            }
        }
        return null;
    }

    [ContextMenu("Show Next Recipe")]
    public void ShowNextRecipe()
    {
        if (craftingRecipes.Count - 1 > currentRecipe)
        {
            currentRecipe++;
        }
        else
        {
            currentRecipe = 0;    
        }
        ShowRecipe(craftingRecipes[currentRecipe].recipe_id);
    }

    [ContextMenu("Show Previous Recipe")]
    public void ShowPreviousRecipe()
    {
        if (currentRecipe > 0)
        {
            currentRecipe--;
        }
        else
        {
            currentRecipe = craftingRecipes.Count -1;
        }
        ShowRecipe(craftingRecipes[currentRecipe].recipe_id);
    }
}
