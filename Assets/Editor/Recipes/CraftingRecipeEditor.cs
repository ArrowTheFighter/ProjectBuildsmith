using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CraftingRecipeData))]
public class CraftingRecipeDataEditor : Editor
{
    private const int gridSize = 3; // 3x3

    public override void OnInspectorGUI()
    {
        // Get a reference to the target ScriptableObject
        CraftingRecipeData recipe = (CraftingRecipeData)target;

        // Draw default fields first
        recipe.recipe_id = EditorGUILayout.TextField("Recipe ID", recipe.recipe_id);
        recipe.recipe_output_id = EditorGUILayout.TextField("Output Item ID", recipe.recipe_output_id);
        recipe.stationType = (CraftingStationTypes)EditorGUILayout.EnumPopup("Station Type", recipe.stationType);

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Recipe Grid (3x3)", EditorStyles.boldLabel);

        // Ensure list has exactly 9 elements
        while (recipe.recipe_items.Count < gridSize * gridSize)
            recipe.recipe_items.Add("");
        while (recipe.recipe_items.Count > gridSize * gridSize)
            recipe.recipe_items.RemoveAt(recipe.recipe_items.Count - 1);

        for (int y = 0; y < gridSize; y++)
        {
            EditorGUILayout.BeginHorizontal();

            // Get available width
            float totalWidth = EditorGUIUtility.currentViewWidth - 40f; // minus padding/margins
            float cellWidth = totalWidth / gridSize;

            for (int x = 0; x < gridSize; x++)
            {
                int index = y * gridSize + x;
                recipe.recipe_items[index] = EditorGUILayout.TextField(
                    recipe.recipe_items[index],
                    GUILayout.Width(cellWidth)
                );
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.Space(10);
        if (GUILayout.Button("Clear Grid"))
        {
            for (int i = 0; i < recipe.recipe_items.Count; i++)
                recipe.recipe_items[i] = "";
        }

        // Save changes
        if (GUI.changed)
        {
            EditorUtility.SetDirty(recipe);
        }
    }
}
