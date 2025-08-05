using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Recipe Database")]
public class RecipeDatabase : ScriptableObject
{
    public List<CraftingRecipeData> recipes = new List<CraftingRecipeData>();
}
