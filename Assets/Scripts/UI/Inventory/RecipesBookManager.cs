using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecipesBookManager : MonoBehaviour
{
    public CraftingStationTypes craftingStationType;
    public CraftingTableRecipeDisplay craftingTableRecipeDisplay;
    public Transform recipeOptionsParent;
    public GameObject recipeOptionPrefab;
    List<string> addedRecipes = new List<string>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Setup();
        HideRecipeBook();
    }

    void Setup()
    {
        foreach (CraftingRecipeData _recipeData in ScriptRefrenceSingleton.instance.gameplayUtils.RecipeDatabase.recipes)
        {
            if (_recipeData.stationType == craftingStationType)
            {
                if (!ScriptRefrenceSingleton.instance.gameplayUtils.inventoryManager.UnlockedRecipes.Contains(_recipeData.recipe_id)) continue;
                GameObject recipeOptionObj = Instantiate(recipeOptionPrefab, recipeOptionsParent);
                RecipeOption recipeOption = recipeOptionObj.GetComponent<RecipeOption>();
                recipeOption.craftingRecipeData = _recipeData;
                ItemData itemData = ScriptRefrenceSingleton.instance.gameplayUtils.GetItemDataByID(_recipeData.recipe_output_id);
                recipeOption.ItemNameTextBox.text = itemData.item_name;
                recipeOption.IconImage.sprite = itemData.item_ui_image;
                recipeOptionObj.GetComponent<Button>().onClick.AddListener(() => { craftingTableRecipeDisplay.ShowRecipe(_recipeData.recipe_id); });

                addedRecipes.Add(_recipeData.recipe_id);
            }
        }
    }

    public void CheckForNewRecipes()
    {
        foreach (CraftingRecipeData _recipeData in ScriptRefrenceSingleton.instance.gameplayUtils.RecipeDatabase.recipes)
        {
            if (_recipeData.stationType == craftingStationType)
            {
                if (!ScriptRefrenceSingleton.instance.gameplayUtils.inventoryManager.UnlockedRecipes.Contains(_recipeData.recipe_id)) continue;
                if (addedRecipes.Contains(_recipeData.recipe_id)) continue;
                GameObject recipeOptionObj = Instantiate(recipeOptionPrefab, recipeOptionsParent);
                RecipeOption recipeOption = recipeOptionObj.GetComponent<RecipeOption>();
                recipeOption.craftingRecipeData = _recipeData;
                ItemData itemData = ScriptRefrenceSingleton.instance.gameplayUtils.GetItemDataByID(_recipeData.recipe_output_id);
                recipeOption.ItemNameTextBox.text = itemData.item_name;
                recipeOption.IconImage.sprite = itemData.item_ui_image;
                recipeOptionObj.GetComponent<Button>().onClick.AddListener(() => { craftingTableRecipeDisplay.ShowRecipe(_recipeData.recipe_id); });

                addedRecipes.Add(_recipeData.recipe_id);
            }
        }
    }

    public void ToggleRecipeBook()
    {
        if (!gameObject.activeInHierarchy)
        {
            ShowRecipeBook();
        }
        else
        {
            HideRecipeBook();    
        }
    }

    public void ShowRecipeBook()
    {
        CheckForNewRecipes();
        gameObject.SetActive(true);
    }

    public void HideRecipeBook()
    {
        gameObject.SetActive(false);
    }
}
