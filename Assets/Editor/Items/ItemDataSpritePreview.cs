using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

[CustomEditor(typeof(ItemData))]
public class ItemDataSpritePreview : Editor
{
    ItemData itemData;
    private Type[] abilityTypes;
    private string[] abilityTypeNames;
    private Dictionary<string, bool> foldouts = new Dictionary<string, bool>();
    private int selectedTypeIndex = 0;

    private void OnEnable()
    {
        itemData = target as ItemData;

        abilityTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => typeof(PlayerAbility).IsAssignableFrom(t) && !t.IsAbstract)
            .ToArray();

        abilityTypeNames = abilityTypes.Select(t => t.Name).ToArray();
    }

    public override void OnInspectorGUI()
    {
        //Draw whatever we already have in SO definition
        base.OnInspectorGUI();
        //Guard clause
        if (itemData.item_ui_image == null)
            return;

        //Convert the weaponSprite (see SO script) to Texture
        Texture2D texture = AssetPreview.GetAssetPreview(itemData.item_ui_image);
        //We create empty space 80x80 (you may need to tweak it to scale better your sprite
        //This allows us to place the image JUST UNDER our default inspector
        GUILayout.Label("", GUILayout.Height(80), GUILayout.Width(80));
        //Draws the texture where we have defined our Label (empty space)
        GUI.DrawTexture(GUILayoutUtility.GetLastRect(), texture);

        for (int i = 0; i < itemData.playerAbilities.Count; i++)
        {
            string typeName = itemData.playerAbilities[i];
            Type type = Type.GetType(typeName);

            if (type == null) return;


            //var objective = itemData.playerAbilities[i];

            EditorGUILayout.BeginVertical("box");


            // if (objective == null)
            // {
            //     // EditorGUILayout.LabelField("Missing or null objective", EditorStyles.boldLabel);
            //     // if (GUILayout.Button("Remove", GUILayout.Width(60)))
            //     // {
            //     //     questData.questObjectives.RemoveAt(i);
            //     //     break;
            //     // }
            //     // EditorGUILayout.EndVertical();
            //     continue;
            // }

            // Ensure there's a foldout state for this objective
            if (!foldouts.ContainsKey(typeName))
                foldouts[typeName] = true;

            // Foldout header
            EditorGUILayout.BeginHorizontal();
            foldouts[typeName] = EditorGUILayout.Foldout(foldouts[typeName], type.GetType().Name, true);
            if (GUILayout.Button("Remove", GUILayout.Width(60)))
            {
                itemData.playerAbilities.RemoveAt(i);
                foldouts.Remove(typeName);
                break;
            }
            EditorGUILayout.EndHorizontal();

            // If expanded, draw fields
            if (foldouts[typeName])
            {
                EditorGUILayout.LabelField("Type", type.FullName);
            }

            EditorGUILayout.EndVertical();

        }

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Add New Objective", EditorStyles.boldLabel);
        selectedTypeIndex = EditorGUILayout.Popup("Objective Type", selectedTypeIndex, abilityTypeNames);

        if (GUILayout.Button("Add Objective"))
        {
            //var newObjective = Activator.CreateInstance(abilityTypes[selectedTypeIndex]) as PlayerAbility;
            itemData.playerAbilities.Add(abilityTypes[selectedTypeIndex].AssemblyQualifiedName);
        }

        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(itemData);
    }

    private void DrawFields(object obj)
    {
        var fields = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
        foreach (var field in fields)
        {
            object value = field.GetValue(obj);

            if (field.FieldType == typeof(string))
                field.SetValue(obj, EditorGUILayout.TextField(field.Name, (string)value));
            else if (field.FieldType == typeof(int))
                field.SetValue(obj, EditorGUILayout.IntField(field.Name, (int)value));
            // Add other types as needed...
        }
    }
}
