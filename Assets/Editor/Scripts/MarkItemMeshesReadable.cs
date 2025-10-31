using UnityEngine;
using UnityEditor;
using System.IO;

public class MarkItemMeshesReadable : EditorWindow
{
    public string itemDataFolder = "Assets/ScriptableObjects/ItemData";

    [MenuItem("Tools/Mark Item Meshes Read/Write")]
    public static void ShowWindow()
    {
        GetWindow<MarkItemMeshesReadable>("Mark Meshes Read/Write");
    }

    void OnGUI()
    {
        GUILayout.Label("Mark Meshes Read/Write", EditorStyles.boldLabel);
        itemDataFolder = EditorGUILayout.TextField("Item Data Folder", itemDataFolder);

        if (GUILayout.Button("Process All ItemData"))
        {
            ProcessAllItemData();
        }
    }

    void ProcessAllItemData()
    {
        string[] guids = AssetDatabase.FindAssets("t:ItemData", new[] { itemDataFolder });
        int count = 0;

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ItemData item = AssetDatabase.LoadAssetAtPath<ItemData>(path);
            if (item == null || item.item_pickup_object == null)
                continue;

            MeshFilter meshFilter = item.item_pickup_object.GetComponentInChildren<MeshFilter>();
            if (meshFilter == null || meshFilter.sharedMesh == null)
            {
                Debug.LogWarning($"⚠️ No MeshFilter found for {item.name}");
                continue;
            }

            Mesh mesh = meshFilter.sharedMesh;
            string meshPath = AssetDatabase.GetAssetPath(mesh);

            if (string.IsNullOrEmpty(meshPath))
            {
                Debug.LogWarning($"⚠️ Mesh for {item.name} is not an asset (might be runtime generated). Skipping.");
                continue;
            }

            ModelImporter importer = AssetImporter.GetAtPath(meshPath) as ModelImporter;
            if (importer != null)
            {
                if (!importer.isReadable)
                {
                    importer.isReadable = true;
                    importer.SaveAndReimport();
                    count++;
                    Debug.Log($"✅ Marked mesh '{mesh.name}' as readable for {item.name}");
                }
            }
            else
            {
                Debug.LogWarning($"⚠️ Mesh importer not found for {mesh.name} (maybe it's a prefab mesh?).");
            }
        }

        Debug.Log($"Done! Updated {count} mesh assets to Read/Write.");
    }
}
