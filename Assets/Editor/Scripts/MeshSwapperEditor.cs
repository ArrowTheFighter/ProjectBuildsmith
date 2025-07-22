using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class MeshSwapperEditor : EditorWindow
{
    private GameObject characterPrefab;

    private MeshMaterialPreset[] headPresets;
    private MeshMaterialPreset[] hatPresets;
    private MeshMaterialPreset[] bodyPresets;
    private MeshMaterialPreset[] rightArmPresets;
    private MeshMaterialPreset[] leftArmPresets;
    private MeshMaterialPreset[] rightLegPresets;
    private MeshMaterialPreset[] leftLegPresets;

    private Material[] headOverride;
    private Material[] hatOverride;
    private Material[] bodyOverride;
    private Material[] rightArmOverride;
    private Material[] leftArmOverride;
    private Material[] rightLegOverride;
    private Material[] leftLegOverride;


    private int headIndex,hatIndex, bodyIndex, rightArmIndex, leftArmIndex, rightLegIndex, leftLegIndex;

    private string filePath = "Assets/Art/Meshes/Characters/BodyParts";



    [MenuItem("Tools/Character Mesh Swapper")]
    public static void ShowWindow()
    {
        GetWindow<MeshSwapperEditor>("Mesh Swapper");
    }

    private void OnGUI()
    {
        GUILayout.Label("Character Mesh Swapper", EditorStyles.boldLabel);

        characterPrefab = (GameObject)EditorGUILayout.ObjectField("Character Prefab", characterPrefab, typeof(GameObject), true);

        if (GUILayout.Button("Load Presets"))
        {
            LoadPresets();
        }

        if (characterPrefab != null)
        {
            EditorGUILayout.Space();
            DrawPresetSelector("Hat", ref hatPresets, ref hatIndex);
            hatOverride = DrawMaterialOverride("Hat Material Override", hatOverride);

            EditorGUILayout.Space();
            DrawPresetSelector("Head", ref headPresets, ref headIndex);
            headOverride = DrawMaterialOverride("Head Material Override", headOverride);

            EditorGUILayout.Space();
            DrawPresetSelector("Body", ref bodyPresets, ref bodyIndex);
            bodyOverride = DrawMaterialOverride("Body Material Override", bodyOverride);

            EditorGUILayout.Space();
            DrawPresetSelector("Right Arm", ref rightArmPresets, ref rightArmIndex);
            rightArmOverride = DrawMaterialOverride("Right Arm Material Override", rightArmOverride);

            EditorGUILayout.Space();
            DrawPresetSelector("Left Arm", ref leftArmPresets, ref leftArmIndex);
            leftArmOverride = DrawMaterialOverride("Left Arm Material Override", leftArmOverride);

            EditorGUILayout.Space();
            DrawPresetSelector("Right Leg", ref rightLegPresets, ref rightLegIndex);
            rightLegOverride = DrawMaterialOverride("Right Leg Material Override", rightLegOverride);

            EditorGUILayout.Space();
            DrawPresetSelector("Left Leg", ref leftLegPresets, ref leftLegIndex);
            leftLegOverride = DrawMaterialOverride("Left Leg Material Override", leftLegOverride);


            EditorGUILayout.Space();
            if (GUILayout.Button("Apply Presets"))
            {
                ApplyPresets();
            }
        }

        
    }


    private void LoadPresets()
    {
        headPresets = LoadPresetsFromFolder(filePath + "/Head");
        hatPresets = LoadPresetsFromFolder(filePath + "/Hat");
        bodyPresets = LoadPresetsFromFolder(filePath + "/Body");
        rightArmPresets = LoadPresetsFromFolder(filePath + "/RightArm");
        leftArmPresets = LoadPresetsFromFolder(filePath + "/LeftArm");
        rightLegPresets = LoadPresetsFromFolder(filePath + "/RightLeg");
        leftLegPresets = LoadPresetsFromFolder(filePath + "/LeftLeg");
    }

    private MeshMaterialPreset[] LoadPresetsFromFolder(string folderPath)
    {
        string[] guids = AssetDatabase.FindAssets("t:MeshMaterialPreset", new[] { folderPath });
        return guids.Select(guid =>
            AssetDatabase.LoadAssetAtPath<MeshMaterialPreset>(AssetDatabase.GUIDToAssetPath(guid))
        ).Where(p => p != null).ToArray();
    }

    private Material[] DrawMaterialOverride(string label, Material[] current)
    {
        EditorGUILayout.LabelField(label, EditorStyles.miniBoldLabel);

        int newSize = Mathf.Max(0, EditorGUILayout.IntField("Size", current != null ? current.Length : 0));

        if (current == null || newSize != current.Length)
        {
            Material[] newArray = new Material[newSize];
            if (current != null)
            {
                for (int i = 0; i < Mathf.Min(newSize, current.Length); i++)
                {
                    newArray[i] = current[i];
                }
            }
            current = newArray;
        }

        for (int i = 0; i < current.Length; i++)
        {
            current[i] = (Material)EditorGUILayout.ObjectField($"Element {i}", current[i], typeof(Material), false);
        }

        return current;
    }


    private void DrawPresetSelector(string label, ref MeshMaterialPreset[] presets, ref int index)
    {
        if (presets != null && presets.Length > 0)
        {
            string[] names = presets.Select(p => p != null ? p.name : "null").ToArray();
            index = EditorGUILayout.Popup(label, index, names);

            // Draw preview of selected preset
            if (index >= 0 && index < presets.Length)
            {
                DrawMeshPreview(presets[index]);
            }
        }
        else
        {
            EditorGUILayout.LabelField($"{label}: No presets loaded.");
        }

    }

    

    private void DrawMeshPreview(MeshMaterialPreset preset)
    {
        if (preset == null) return;

        Object previewTarget = preset.mesh ? (Object)preset.mesh : null;

        if (previewTarget != null)
        {
            Texture2D preview = AssetPreview.GetAssetPreview(previewTarget);
            if (preview == null)
            {
                preview = AssetPreview.GetMiniThumbnail(previewTarget);
            }

            if (preview != null)
            {
                GUILayout.Label(preview, GUILayout.Width(100), GUILayout.Height(100));
            }
        }
    }


    private void ApplyPresets()
    {
        ApplyPresetToPart("Head", headPresets, headIndex, headOverride);
        ApplyPresetToPart("Hat", hatPresets, hatIndex, hatOverride);
        ApplyPresetToPart("Body", bodyPresets, bodyIndex, bodyOverride);
        ApplyPresetToPart("RightArm", rightArmPresets, rightArmIndex, rightArmOverride);
        ApplyPresetToPart("LeftArm", leftArmPresets, leftArmIndex, leftArmOverride);
        ApplyPresetToPart("RightLeg", rightLegPresets, rightLegIndex, rightLegOverride);
        ApplyPresetToPart("LeftLeg", leftLegPresets, leftLegIndex, leftLegOverride);


        Debug.Log("Presets applied successfully.");
    }

    private void ApplyPresetToPart(string partName, MeshMaterialPreset[] presets, int index, Material[] materialOverride)
    {
        if (index < 0 || index >= presets.Length) return;

        Transform part = characterPrefab.transform.Find(partName);
        if (part == null)
        {
            Debug.LogWarning($"Part {partName} not found.");
            return;
        }

        SkinnedMeshRenderer smr = part.GetComponent<SkinnedMeshRenderer>();
        if (smr == null)
        {
            Debug.LogWarning($"No SkinnedMeshRenderer on {partName}.");
            return;
        }

        smr.sharedMesh = presets[index].mesh;

        // Use override materials if provided and not empty
        if (materialOverride != null && materialOverride.Length > 0 && materialOverride[0] != null)
        {
            smr.sharedMaterials = materialOverride;
        }
        else
        {
            smr.sharedMaterials = presets[index].materials;
        }
    }

}
