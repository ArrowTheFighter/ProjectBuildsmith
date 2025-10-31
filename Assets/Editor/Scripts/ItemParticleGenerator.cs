using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class ItemParticleGenerator : EditorWindow
{
    [Header("Setup")]
    public GameObject baseParticlePrefab;
    public string outputFolder = "Assets/GeneratedParticles";
    public string itemDataFolder = "Assets/ScriptableObjects/ItemData";
    public ItemRefrenceScriptableObject referenceListSO;

    [MenuItem("Tools/Item Particle Generator")]
    public static void ShowWindow()
    {
        GetWindow<ItemParticleGenerator>("Item Particle Generator");
    }

    void OnGUI()
    {
        GUILayout.Label("Particle Generator Settings", EditorStyles.boldLabel);
        baseParticlePrefab = (GameObject)EditorGUILayout.ObjectField("Base Particle Prefab", baseParticlePrefab, typeof(GameObject), false);
        outputFolder = EditorGUILayout.TextField("Output Folder", outputFolder);
        itemDataFolder = EditorGUILayout.TextField("Item Data Folder", itemDataFolder);
        referenceListSO = (ItemRefrenceScriptableObject)EditorGUILayout.ObjectField("Reference ScriptableObject", referenceListSO, typeof(ItemRefrenceScriptableObject), false);

        EditorGUILayout.Space();

        if (GUILayout.Button("Generate & Update Reference List"))
        {
            GenerateParticlesAndUpdateList();
        }
    }

    void GenerateParticlesAndUpdateList()
    {
        if (baseParticlePrefab == null)
        {
            Debug.LogError("❌ Base particle prefab not assigned!");
            return;
        }

        if (referenceListSO == null)
        {
            Debug.LogError("❌ ItemRefrenceScriptableObject not assigned!");
            return;
        }

        // Ensure output folder exists
        if (!AssetDatabase.IsValidFolder(outputFolder))
        {
            Directory.CreateDirectory(outputFolder);
            AssetDatabase.Refresh();
        }

        // Clear existing list before rebuilding
        Undo.RecordObject(referenceListSO, "Update Item Reference List");
        referenceListSO.objectRefrences = new List<GameObject>();

        string[] guids = AssetDatabase.FindAssets("t:ItemData", new[] { itemDataFolder });
        int count = 0;

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ItemData item = AssetDatabase.LoadAssetAtPath<ItemData>(path);
            if (item == null || item.item_pickup_object == null)
                continue;

            MeshFilter meshFilter = item.item_pickup_object.GetComponentInChildren<MeshFilter>();
            MeshRenderer meshRenderer = item.item_pickup_object.GetComponentInChildren<MeshRenderer>();

            if (meshFilter == null || meshRenderer == null)
            {
                Debug.LogWarning($"⚠️ No mesh or renderer found in pickup object for {item.name}");
                continue;
            }

            // Duplicate base particle
            GameObject newParticle = PrefabUtility.InstantiatePrefab(baseParticlePrefab) as GameObject;
            newParticle.name = $"{item.item_name}_Particle";

            // Replace mesh + material
            MeshFilter particleMeshFilter = newParticle.GetComponentInChildren<MeshFilter>();
            MeshRenderer particleMeshRenderer = newParticle.GetComponentInChildren<MeshRenderer>();

            if (particleMeshFilter != null)
                particleMeshFilter.sharedMesh = meshFilter.sharedMesh;

            if (particleMeshRenderer != null)
                particleMeshRenderer.sharedMaterial = meshRenderer.sharedMaterial;

            // Save as prefab
            string savePath = $"{outputFolder}/{newParticle.name}.prefab";
            GameObject prefabAsset = PrefabUtility.SaveAsPrefabAsset(newParticle, savePath);
            GameObject.DestroyImmediate(newParticle);

            referenceListSO.objectRefrences.Add(prefabAsset);
            count++;
        }

        // Save and refresh
        EditorUtility.SetDirty(referenceListSO);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"✅ Generated {count} particle prefabs and updated {referenceListSO.name}");
    }
}
