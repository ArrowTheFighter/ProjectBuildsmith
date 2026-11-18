
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class MaterialReplacerWindow : EditorWindow
{
    private Material sourceMaterial;
    private Material replacementMaterial;
    private bool matchByName;
    private string sourceName = "";

    [MenuItem("Tools/Material Replacer")]
    public static void ShowWindow()
    {
        GetWindow<MaterialReplacerWindow>("Material Replacer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Material Replacer",
            EditorStyles.boldLabel);

        sourceMaterial = (Material)EditorGUILayout.ObjectField(
            "Source Material",
            sourceMaterial,
            typeof(Material),
            false
        );

        replacementMaterial = (Material)EditorGUILayout.ObjectField(
            "Replacement Material",
            replacementMaterial,
            typeof(Material),
            false
        );

        matchByName = EditorGUILayout.Toggle(
            "Match By Name",
            matchByName
        );

        if (matchByName)
        {
            sourceName = EditorGUILayout.TextField(
                "Material Name",
                sourceName
            );
        }
        else if (sourceMaterial != null)
        {
            sourceName = sourceMaterial.name;
        }

        EditorGUILayout.Space();

        GUI.enabled = replacementMaterial != null &&
            ((!matchByName && sourceMaterial != null) ||
             (matchByName && !string.IsNullOrWhiteSpace(sourceName)));

        if (GUILayout.Button("Replace Materials"))
        {
            ReplaceMaterials();
        }

        GUI.enabled = true;
    }

    private void ReplaceMaterials()
    {
        int replacedCount = 0;

        Renderer[] renderers =
            Resources.FindObjectsOfTypeAll<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            if (!renderer.gameObject.scene.IsValid() ||
                !renderer.gameObject.scene.isLoaded)
                continue;

            Material[] materials = renderer.sharedMaterials;
            bool changed = false;

            for (int i = 0; i < materials.Length; i++)
            {
                if (materials[i] == null)
                    continue;

                bool matches = matchByName
                    ? materials[i].name == sourceName
                    : materials[i] == sourceMaterial;

                if (matches)
                {
                    materials[i] = replacementMaterial;
                    replacedCount++;
                    changed = true;
                }
            }

            if (changed)
            {
                Undo.RecordObject(renderer, "Replace Materials");

                renderer.sharedMaterials = materials;

                PrefabUtility.RecordPrefabInstancePropertyModifications(
                    renderer
                );

                EditorUtility.SetDirty(renderer);

                EditorSceneManager.MarkSceneDirty(
                    renderer.gameObject.scene
                );
            }
        }

        Debug.Log(
            $"Material Replacer: Replaced {replacedCount} material references."
        );
    }
}
