using System.Collections.Generic;
using UnityEngine;

public class CraftingTableUIManager : MonoBehaviour
{
    [SerializeField] GameObject RecipePrefab;
    [SerializeField] GameObject RecipeLayoutObject;
    [SerializeField] float recipe_spacing = 20f;

    List<GameObject> Recipe_Objects = new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Item_Recipe[] allRecipies = Resources.LoadAll<Item_Recipe>("Recipes");
        foreach (var data in allRecipies)
        {
            SpawnNewRecipe(data);
            Debug.Log($"Loaded: {data.ID} - {data.Item_Name}");
        }
    }

    [ContextMenu("SpawnRecipe")]
    public void SpawnNewRecipe(Item_Recipe item_Recipe)
    {
        GameObject Recipe_GameObject = Instantiate(RecipePrefab,RecipeLayoutObject.transform);
        Recipe_Objects.Add(Recipe_GameObject);

        RectTransform recipe_layout_rect = RecipeLayoutObject.GetComponent<RectTransform>();
        float horizontal_center = recipe_layout_rect.sizeDelta.x * 0.5f;

        RectTransform recipe_rect = Recipe_GameObject.GetComponent<RectTransform>();

        recipe_rect.anchorMin = new Vector2(0.5f,1);
        recipe_rect.anchorMax = new Vector2(0.5f,1);

        float RecipeHeight = recipe_rect.sizeDelta.y;
        recipe_rect.anchoredPosition = new Vector2(0,-(RecipeHeight * 0.5f + recipe_spacing) + -(RecipeHeight + recipe_spacing) * (Recipe_Objects.Count - 1));
        recipe_layout_rect.sizeDelta = new Vector2(recipe_layout_rect.sizeDelta.x,(RecipeHeight + recipe_spacing) * Recipe_Objects.Count);

        Recipe_GameObject.GetComponent<CraftingItemRecipeScript>().Set_Recipe_Details(item_Recipe);
    }

}


