#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class FBXBodyPartPresetBuilder : EditorWindow
{
    private GameObject fbxAsset;

    private List<SkinnedMeshData> meshRenderers = new List<SkinnedMeshData>();
    private Dictionary<SkinnedMeshData, string> partTypeMap = new Dictionary<SkinnedMeshData, string>();

    private string[] bodyParts = new string[] { "Head", "Body", "LeftArm", "RightArm", "LeftLeg", "RightLeg" };
    private string presetRootPath = "Assets/Art/Meshes/Characters/BodyParts";

    [MenuItem("Tools/FBX Body Part Preset Builder")]
    public static void ShowWindow()
    {
        GetWindow<FBXBodyPartPresetBuilder>("FBX Preset Builder");
    }

    private void OnGUI()
    {
        GUILayout.Label("FBX Body Part Preset Builder", EditorStyles.boldLabel);

        GameObject newFbx = (GameObject)EditorGUILayout.ObjectField("FBX Asset", fbxAsset, typeof(GameObject), false);

        if (newFbx != fbxAsset)
        {
            fbxAsset = newFbx;
            meshRenderers.Clear();
            partTypeMap.Clear();
        }

        if (GUILayout.Button("Load Meshes from FBX") && fbxAsset != null)
        {
            LoadMeshRenderers();
        }

        if (meshRenderers.Count > 0)
        {
            EditorGUILayout.Space();
            foreach (var smr in meshRenderers)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(smr.name, GUILayout.Width(150));
                if (!partTypeMap.ContainsKey(smr))
                    partTypeMap[smr] = "Body";

                int selectedIndex = System.Array.IndexOf(bodyParts, partTypeMap[smr]);
                if (selectedIndex < 0) selectedIndex = 0;

                selectedIndex = EditorGUILayout.Popup(selectedIndex, bodyParts);
                partTypeMap[smr] = bodyParts[selectedIndex];
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();
            if (GUILayout.Button("Create Presets"))
            {
                CreatePresets();
            }
        }
    }

    private void LoadMeshRenderers()
    {
        meshRenderers.Clear();
        partTypeMap.Clear();

        if (fbxAsset == null) return;

        GameObject tempInstance = (GameObject)PrefabUtility.InstantiatePrefab(fbxAsset);
        tempInstance.hideFlags = HideFlags.HideAndDontSave;

        var renderers = tempInstance.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (var smr in renderers)
        {
            if (smr.sharedMesh == null) continue;

            var data = new SkinnedMeshData
            {
                name = smr.name,
                mesh = smr.sharedMesh,
                materials = smr.sharedMaterials
            };

            meshRenderers.Add(data);
            partTypeMap[data] = "Body";
        }

        EditorApplication.delayCall += () =>
        {
            if (tempInstance != null)
                GameObject.DestroyImmediate(tempInstance);
        };
    }

    private void CreatePresets()
    {
        foreach (var smr in meshRenderers)
        {
            if (smr == null || smr.mesh == null)
                continue;

            string partType = partTypeMap[smr];
            string folderPath = Path.Combine(presetRootPath, partType);

            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                Directory.CreateDirectory(folderPath);
                AssetDatabase.Refresh();
            }

            MeshMaterialPreset preset = ScriptableObject.CreateInstance<MeshMaterialPreset>();
            preset.mesh = smr.mesh;
            preset.materials = smr.materials;

            string fileName = $"{smr.name}_Preset.asset";
            string assetPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(folderPath, fileName));

            AssetDatabase.CreateAsset(preset, assetPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Done", "Presets created successfully.", "OK");
    }

    [System.Serializable]
    public class SkinnedMeshData
    {
        public string name;
        public Mesh mesh;
        public Material[] materials;
    }
}
#endif
