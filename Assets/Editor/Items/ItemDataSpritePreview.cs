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
            .Where(t => typeof(AbilityData).IsAssignableFrom(t) && !t.IsAbstract && !t.IsGenericType)
            .ToArray();


        abilityTypeNames = abilityTypes.Select(t => t.Name).ToArray();
    }

    public override void OnInspectorGUI()
    {
        // Draw base inspector
        base.OnInspectorGUI();

        // Show the item preview sprite
        if (itemData.item_ui_image != null)
        {
            Texture2D texture = AssetPreview.GetAssetPreview(itemData.item_ui_image);
            GUILayout.Label("", GUILayout.Height(80), GUILayout.Width(80));
            GUI.DrawTexture(GUILayoutUtility.GetLastRect(), texture);
        }

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Ability Configurations", EditorStyles.boldLabel);

        for (int i = 0; i < itemData.abilityConfigs.Count; i++)
        {
            AbilityData ability = itemData.abilityConfigs[i];
            if (ability == null) continue;

            string key = $"{i}_{ability.GetType().Name}";
            if (!foldouts.ContainsKey(key))
                foldouts[key] = true;

            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.BeginHorizontal();
            foldouts[key] = EditorGUILayout.Foldout(foldouts[key], ability.GetType().Name, true);
            if (GUILayout.Button("Remove", GUILayout.Width(60)))
            {
                itemData.abilityConfigs.RemoveAt(i);
                foldouts.Remove(key);
                break;
            }
            EditorGUILayout.EndHorizontal();

            if (foldouts[key])
            {
                EditorGUI.indentLevel++;
                DrawFields(ability); // 👈 This draws fields like "damage"
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndVertical();
        }


        // Add new ability config
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Add New Ability", EditorStyles.boldLabel);
        selectedTypeIndex = EditorGUILayout.Popup("Ability Type", selectedTypeIndex, abilityTypeNames);

        if (GUILayout.Button("Add Ability"))
        {
            var type = abilityTypes[selectedTypeIndex];
            var newAbility = Activator.CreateInstance(type) as AbilityData;
            itemData.abilityConfigs.Add(newAbility);
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
            Type fieldType = field.FieldType;

            EditorGUI.BeginChangeCheck();
            string label = ObjectNames.NicifyVariableName(field.Name);

            if (fieldType == typeof(string))
            {
                value = EditorGUILayout.TextField(label, (string)value);
            }
            else if (fieldType == typeof(int))
            {
                value = EditorGUILayout.IntField(label, (int)value);
            }
            else if (fieldType == typeof(float))
            {
                value = EditorGUILayout.FloatField(label, (float)value);
            }
            else if (fieldType == typeof(bool))
            {
                value = EditorGUILayout.Toggle(label, (bool)value);
            }
            else if (typeof(UnityEngine.Object).IsAssignableFrom(fieldType))
            {
                value = EditorGUILayout.ObjectField(label, (UnityEngine.Object)value, fieldType, true);
            }
            else if (fieldType.IsEnum)
            {
                value = EditorGUILayout.EnumPopup(label, (Enum)value);
            }
            else if (fieldType.IsArray && fieldType.GetElementType().IsEnum)
            {
                Array array = value as Array ?? Array.CreateInstance(fieldType.GetElementType(), 0);
                Type enumType = fieldType.GetElementType();

                int size = array.Length;
                size = EditorGUILayout.IntField(label + " Size", size);

                Array newArray = Array.CreateInstance(enumType, size);

                for (int i = 0; i < size; i++)
                {
                    Enum element = (i < array.Length) ? (Enum)array.GetValue(i) : (Enum)Enum.GetValues(enumType).GetValue(0);
                    element = EditorGUILayout.EnumPopup($"{label} [{i}]", element);
                    newArray.SetValue(element, i);
                }

                value = newArray;
            }
            else
            {
                EditorGUILayout.LabelField(label, $"Unsupported Type ({fieldType.Name})");
            }

            if (EditorGUI.EndChangeCheck())
            {
                field.SetValue(obj, value);
            }
        }
    }


}
