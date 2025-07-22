using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class MeshSwapperEditor : EditorWindow
{
    private GameObject characterPrefab;

    private MeshMaterialPreset[] headPresets;
    private MeshMaterialPreset[] bodyPresets;
    private MeshMaterialPreset[] rightArmPresets;
    private MeshMaterialPreset[] leftArmPresets;
    private MeshMaterialPreset[] rightLegPresets;
    private MeshMaterialPreset[] leftLegPresets;

    private Material[] headOverride;
    private Material[] bodyOverride;
    private Material[] rightArmOverride;
    private Material[] leftArmOverride;
    private Material[] rightLegOverride;
    private Material[] leftLegOverride;


    private int headIndex,hatIndex, bodyIndex, rightArmIndex, leftArmIndex, rightLegIndex, leftLegIndex;

    private string filePath = "Assets/Art/Meshes/Characters/BodyParts";

    private Vector2 scrollPos;

    private bool linkArms = false;
    private bool linkLegs = false;



    [MenuItem("Tools/Character Mesh Swapper")]
    public static void ShowWindow()
    {
        GetWindow<MeshSwapperEditor>("Mesh Swapper");
    }

    private void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        GUILayout.Label("Character Mesh Swapper", EditorStyles.boldLabel);

        characterPrefab = (GameObject)EditorGUILayout.ObjectField("Character Prefab", characterPrefab, typeof(GameObject), true);

        if (GUILayout.Button("Load Presets"))
        {
            LoadPresets();
        }

        if (characterPrefab != null)
        {
            GUILayout.Space(10f);
            Rect rect = EditorGUILayout.GetControlRect(false, 6);
            EditorGUI.DrawRect(rect, new Color(0.6f, 0.6f, 0.6f)); // light gray
            GUILayout.Space(10f);

            bool changed;
            bool headChanged, bodyChanged, rightArmChanged, leftArmChanged = false, rightChanged, leftChanged = false, rightLegChanged, leftLegChanged = false;


            headIndex = DrawPresetSelector("Head", headPresets, headIndex, out headChanged);
            headOverride = DrawMaterialOverride("Head Material Override", headOverride, out changed);
            if (changed) ApplyPresets();

            GUILayout.Space(10f);
            Rect rect2 = EditorGUILayout.GetControlRect(false, 6);
            EditorGUI.DrawRect(rect2, new Color(0.6f, 0.6f, 0.6f)); // light gray
            GUILayout.Space(10f);

            bodyIndex = DrawPresetSelector("Body", bodyPresets, bodyIndex,out bodyChanged);
            bodyOverride = DrawMaterialOverride("Body Material Override", bodyOverride, out changed);
            if (changed) ApplyPresets();

            GUILayout.Space(10f);
            Rect rect3 = EditorGUILayout.GetControlRect(false, 6);
            EditorGUI.DrawRect(rect3, new Color(0.6f, 0.6f, 0.6f)); // light gray
            GUILayout.Space(10f);

            linkArms = EditorGUILayout.Toggle("Link Arms", linkArms);

            rightArmIndex = DrawPresetSelector("Right Arm", rightArmPresets, rightArmIndex, out rightArmChanged);
            rightArmOverride = DrawMaterialOverride("Right Arm Material Override", rightArmOverride, out rightChanged);
            if (linkArms)
            {
                if (rightArmOverride != null)
                {
                    leftArmOverride = new Material[rightArmOverride.Length];
                    for (int i = 0; i < rightArmOverride.Length; i++)
                    {
                        leftArmOverride[i] = rightArmOverride[i];
                    }
                }
                else
                {
                    leftArmOverride = null;
                }
            }
            if (rightChanged) ApplyPresets();

            bool skipLeftSelector = false;

            if (linkArms && rightArmPresets != null && leftArmPresets != null)
            {
                string rightName = rightArmPresets[rightArmIndex]?.name;
                if (!string.IsNullOrEmpty(rightName))
                {
                    string prefix = rightName.Split('_')[0];

                    int matchingIndex = System.Array.FindIndex(leftArmPresets, p => p != null && p.name.StartsWith(prefix + "_"));
                    if (matchingIndex != -1 && matchingIndex != leftArmIndex)
                    {
                        leftArmIndex = matchingIndex;
                        ApplyPresets();  // <- Apply immediately when linked index changes
                    }
                }
                skipLeftSelector = true;
            }

            if (!skipLeftSelector)
            {
                leftArmIndex = DrawPresetSelector("Left Arm", leftArmPresets, leftArmIndex, out leftArmChanged);
                leftArmOverride = DrawMaterialOverride("Left Arm Material Override", leftArmOverride, out leftChanged);
                if (leftChanged) ApplyPresets();
            }
            else
            {
                // Show label so user still sees something
                EditorGUILayout.LabelField("Left Arm linked to Right Arm.");
            }
            


            GUILayout.Space(10f);
            Rect rect5 = EditorGUILayout.GetControlRect(false, 6);
            EditorGUI.DrawRect(rect5, new Color(0.6f, 0.6f, 0.6f)); // light gray
            GUILayout.Space(10f);

            linkLegs = EditorGUILayout.Toggle("Link Legs", linkLegs);

            bool leftLegMeshChanged, rightLegMeshChanged;

            rightLegIndex = DrawPresetSelector("Right Leg", rightLegPresets, rightLegIndex, out rightLegChanged);
            rightLegOverride = DrawMaterialOverride("Right Leg Material Override", rightLegOverride, out rightLegMeshChanged);

            if (linkLegs)
            {
                if (rightLegOverride != null)
                {
                    leftLegOverride = new Material[rightLegOverride.Length];
                    for (int i = 0; i < rightLegOverride.Length; i++)
                    {
                        leftLegOverride[i] = rightLegOverride[i];
                    }
                }
                else
                {
                    leftLegOverride = null;
                }
            }
            if (rightLegMeshChanged)
            {
                
                ApplyPresets();
            } 

            bool skipLeftLegSelector = false;

            if (linkLegs && rightLegPresets != null && leftLegPresets != null)
            {
                string rightLegName = rightLegPresets[rightLegIndex]?.name;
                if (!string.IsNullOrEmpty(rightLegName))
                {
                    string prefix = rightLegName.Split('_')[0];

                    int matchingIndex = System.Array.FindIndex(leftLegPresets, p => p != null && p.name.StartsWith(prefix + "_"));
                    if (matchingIndex != -1 && matchingIndex != leftLegIndex)
                    {
                        leftLegIndex = matchingIndex;

                        

                        ApplyPresets();  // Apply immediately when linked
                    }
                }

                skipLeftLegSelector = true;
            }
            Debug.Log(skipLeftLegSelector);
            if (!skipLeftLegSelector)
            {
                leftLegIndex = DrawPresetSelector("Left Leg", leftLegPresets, leftLegIndex, out leftLegChanged);
                leftLegOverride = DrawMaterialOverride("Left Leg Material Override", leftLegOverride, out leftLegMeshChanged);
                if (leftLegMeshChanged) ApplyPresets();
            }
            else
            {
                EditorGUILayout.LabelField("Left Leg linked to Right Leg.");
            }


            GUILayout.Space(10f);
            Rect rect7 = EditorGUILayout.GetControlRect(false, 6);
            EditorGUI.DrawRect(rect7, new Color(0.6f, 0.6f, 0.6f)); // light gray
            GUILayout.Space(10f);

            if (headChanged || bodyChanged || leftArmChanged|| rightArmChanged|| rightChanged || leftChanged || rightLegChanged || leftLegChanged)
            {
                ApplyPresets();
                Repaint();
            }


            if (GUILayout.Button("Apply Presets"))
            {
                ApplyPresets();
            }
        }
        EditorGUILayout.EndScrollView();  // ✅ End scroll view

    }


    private void LoadPresets()
    {
        headPresets = LoadPresetsFromFolder(filePath + "/Head");
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

    private Material[] DrawMaterialOverride(string label, Material[] current, out bool changed)
    {
        changed = false;

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
            changed = true;
        }

        for (int i = 0; i < current.Length; i++)
        {
            Material original = current[i];
            current[i] = (Material)EditorGUILayout.ObjectField($"Element {i}", current[i], typeof(Material), false);
            if (current[i] != original)
            {
                changed = true;
            }
        }

        return current;
    }



    private void DrawPresetSelector(string label, ref MeshMaterialPreset[] presets, ref int index)
    {
        if (presets != null && presets.Length > 0)
        {
            string[] names = presets.Select(p => p != null ? p.name : "null").ToArray();

            EditorGUILayout.BeginHorizontal();

            int newIndex = EditorGUILayout.Popup(label, index, names);
            if (newIndex != index)
            {
                index = newIndex;
                ApplyPresets();  // ← Apply immediately when selection changes
            }

            if (GUILayout.Button("Random", GUILayout.Width(60)))
            {
                index = Random.Range(0, presets.Length);
                ApplyPresets();  // ← Also apply when randomized
                Repaint();
            }

            EditorGUILayout.EndHorizontal();

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

    private int DrawPresetSelector(string label, MeshMaterialPreset[] presets, int index, out bool changed)
    {
        changed = false;

        if (presets != null && presets.Length > 0)
        {
            string[] names = presets.Select(p => p != null ? p.name : "null").ToArray();

            EditorGUILayout.BeginHorizontal();

            int newIndex = EditorGUILayout.Popup(label, index, names);
            if (newIndex != index)
            {
                changed = true;
                index = newIndex;
            }

            if (GUILayout.Button("Random", GUILayout.Width(60)))
            {
                index = Random.Range(0, presets.Length);
                changed = true;
                Repaint();
            }

            EditorGUILayout.EndHorizontal();

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

        return index;
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
