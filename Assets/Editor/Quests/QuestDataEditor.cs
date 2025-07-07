using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

[CustomEditor(typeof(QuestData))]
public class QuestDataEditor : Editor
{
    private QuestData questData;
    private Type[] objectiveTypes;
    private string[] objectiveTypeNames;
    private int selectedTypeIndex = 0;
    private Dictionary<QuestObjective, bool> foldouts = new Dictionary<QuestObjective, bool>();

    private void OnEnable()
    {
        questData = (QuestData)target;

        objectiveTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => typeof(QuestObjective).IsAssignableFrom(t) && !t.IsAbstract)
            .ToArray();

        objectiveTypeNames = objectiveTypes.Select(t => t.Name).ToArray();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Draw QuestData fields
        EditorGUILayout.LabelField("Quest Info", EditorStyles.boldLabel);
        questData.ID = EditorGUILayout.TextField("ID", questData.ID);
        questData.Name = EditorGUILayout.TextField("Name", questData.Name);
        questData.Description = EditorGUILayout.TextField("Description", questData.Description);

        GUILayout.Space(10);

        // Draw all objectives
        for (int i = 0; i < questData.questObjectives.Count; i++)
        {
            var objective = questData.questObjectives[i];

            EditorGUILayout.BeginVertical("box");

            if (objective == null)
            {
                // EditorGUILayout.LabelField("Missing or null objective", EditorStyles.boldLabel);
                // if (GUILayout.Button("Remove", GUILayout.Width(60)))
                // {
                //     questData.questObjectives.RemoveAt(i);
                //     break;
                // }
                // EditorGUILayout.EndVertical();
                continue;
            }

            // Ensure there's a foldout state for this objective
            if (!foldouts.ContainsKey(objective))
                foldouts[objective] = true;

            // Foldout header
            EditorGUILayout.BeginHorizontal();
            foldouts[objective] = EditorGUILayout.Foldout(foldouts[objective], objective.GetType().Name, true);
            if (GUILayout.Button("Remove", GUILayout.Width(60)))
            {
                questData.questObjectives.RemoveAt(i);
                foldouts.Remove(objective);
                break;
            }
            EditorGUILayout.EndHorizontal();

            // If expanded, draw fields
            if (foldouts[objective])
            {
                DrawFields(objective);
            }

            EditorGUILayout.EndVertical();

        }

            GUILayout.Space(10);
        EditorGUILayout.LabelField("Add New Objective", EditorStyles.boldLabel);
        selectedTypeIndex = EditorGUILayout.Popup("Objective Type", selectedTypeIndex, objectiveTypeNames);

        if (GUILayout.Button("Add Objective"))
        {
            var newObjective = Activator.CreateInstance(objectiveTypes[selectedTypeIndex]) as QuestObjective;
            questData.questObjectives.Add(newObjective);
        }

        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(questData);
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
