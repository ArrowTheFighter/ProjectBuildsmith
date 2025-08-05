using UnityEngine;
using UnityEditor;

public class RecipeDatabaseBuilder
{
    [MenuItem("Tools/Build Recipe Database")]
    [InitializeOnEnterPlayMode]
    public static void BuildDatabase()
    {
        string dbPath = "Assets/Resources/Recipes/RecipeDatabase.asset";

        // Create or load the database
        RecipeDatabase database = AssetDatabase.LoadAssetAtPath<RecipeDatabase>(dbPath);
        if (database == null)
        {
            database = ScriptableObject.CreateInstance<RecipeDatabase>();
            AssetDatabase.CreateAsset(database, dbPath);
        }

        // Clear the current list
        database.recipes.Clear();

        // Find all CraftingRecipeData assets
        string[] guids = AssetDatabase.FindAssets("t:CraftingRecipeData");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            CraftingRecipeData recipe = AssetDatabase.LoadAssetAtPath<CraftingRecipeData>(path);
            if (recipe != null && !database.recipes.Contains(recipe))
            {
                database.recipes.Add(recipe);
            }
        }

        EditorUtility.SetDirty(database);
        AssetDatabase.SaveAssets();
        Debug.Log($"Recipe Database built with {database.recipes.Count} recipes.");
    }
}
