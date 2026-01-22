using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/CraftingRecipe")]
public class CraftingRecipeData : ScriptableObject
{
    public string recipe_id;
    public string recipe_output_id;
    public CraftingStationTypes stationType;
    public List<string> recipe_items = new List<string>(9);

    // =============================
    // Runtime-generated pattern data
    // =============================

    [HideInInspector] public string[,] pattern;
    [HideInInspector] public int width;
    [HideInInspector] public int height;

    /// <summary>
    /// Builds a trimmed pattern from the 3x3 recipe_items list.
    /// Call once at runtime (eg. on game start).
    /// </summary>
    public void BuildPattern()
    {
        if (recipe_items == null || recipe_items.Count != 9)
        {
            if(recipe_items.Count < 9)
            {
               while(recipe_items.Count < 9)
                {
                    recipe_items.Add(string.Empty);
                } 
            }
            if(recipe_items.Count != 9)
            {
                Debug.LogWarning($"Recipe '{name}' encountered an unexpected error with the recipe list size. This recipe had {recipe_items.Count}");

                pattern = null;
                return;
            }
            
        }

        string[,] grid = new string[3, 3];

        // Flat list → 2D grid
        for (int i = 0; i < 9; i++)
        {
            int y = i / 3;
            int x = i % 3;
            grid[y, x] = recipe_items[i];
        }

        // Find bounds of non-empty items
        int minY = 3, maxY = -1, minX = 3, maxX = -1;

        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 3; x++)
            {
                if (!string.IsNullOrEmpty(grid[y, x]))
                {
                    minY = Mathf.Min(minY, y);
                    maxY = Mathf.Max(maxY, y);
                    minX = Mathf.Min(minX, x);
                    maxX = Mathf.Max(maxX, x);
                }
            }
        }

        // Empty recipe guard
        if (maxY < minY || maxX < minX)
        {
            Debug.LogWarning($"Recipe '{name}' is empty.");
            pattern = null;
            width = height = 0;
            return;
        }

        height = maxY - minY + 1;
        width = maxX - minX + 1;

        pattern = new string[height, width];

        // Extract trimmed pattern
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                pattern[y, x] = grid[minY + y, minX + x];
            }
        }
    }
}
