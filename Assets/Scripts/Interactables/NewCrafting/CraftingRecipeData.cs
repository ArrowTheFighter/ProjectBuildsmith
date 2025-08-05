using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/CraftingRecipe")]
public class CraftingRecipeData : ScriptableObject
{
    public string recipe_id;
    public string recipe_output_id;
    public CraftingStationTypes stationType;
    public List<string> recipe_items = new List<string>();
}
