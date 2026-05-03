using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector.Editor.GettingStarted;
using UnityEditor.Localization.Plugins.XLIFF.V20;
using System.Collections.Generic;

public class EditorReplacerTool : EditorWindow
{
    int objectsReplaced;
    GameObject parentObject;
    List<PrefabReplacementRule> rules = new List<PrefabReplacementRule>();

    [MenuItem("Tools/Prefab Replacer")]
    public static void ShowWindow()
    {
        GetWindow<EditorReplacerTool>("Prefab Replacer");
    }

    void OnGUI()
    {
        GUILayout.Label("Replace Prefabs in Children", EditorStyles.boldLabel);

        parentObject = (GameObject)EditorGUILayout.ObjectField(
            "Parent Object", parentObject,typeof(GameObject),true);

            GUILayout.Space(10);

        for (int i = 0; i < rules.Count; i++)
        {
            GUILayout.BeginVertical("box");

            GUILayout.BeginHorizontal();
            GUILayout.Label($"Rule {i + 1}", EditorStyles.boldLabel);

            if (GUILayout.Button("Remove"))
            {
                rules.RemoveAt(i);
                break;
            }
            GUILayout.EndHorizontal();

            rules[i].prefabToFind = (GameObject)EditorGUILayout.ObjectField(
                "Find", rules[i].prefabToFind, typeof(GameObject), false);

            rules[i].prefabToReplace = (GameObject)EditorGUILayout.ObjectField(
                "Replace", rules[i].prefabToReplace, typeof(GameObject), false);

            GUILayout.EndVertical();
        }
        
        if (GUILayout.Button("Add Rule"))
        {
            rules.Add(new PrefabReplacementRule());
        }

        GUILayout.Space(10);
        Color oldColor = GUI.backgroundColor;

        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Replace All",GUILayout.Height(40)))
        {
            ReplacePrefabs();
        }

        GUI.backgroundColor = oldColor;
    }

    void ReplacePrefabs()
    {
        if(parentObject == null)
        {
            Debug.LogWarning("Missing fields!");
            return;
        }
        objectsReplaced = 0;
        Transform[] children = parentObject.GetComponentsInChildren<Transform>();

        foreach(Transform child in children)
        {
            CheckAndReplace(child.gameObject);
        }
        Debug.Log($"Succesfully replaced {objectsReplaced} prefabs");
    }

    void CheckAndReplace(GameObject obj)
    {
        if(!PrefabUtility.IsPartOfPrefabInstance(obj))
        {
            Debug.Log("Obj wasn't part of a prefab instance");
            return;

        }

        GameObject instanceRoot = PrefabUtility.GetNearestPrefabInstanceRoot(obj);

        GameObject prefabAsset = PrefabUtility.GetCorrespondingObjectFromSource(instanceRoot);

        if(prefabAsset == null)
        {
            Debug.Log("Source prefab was null");
            return;
        }

        foreach(var rule in rules)
        {
            if(rule.prefabToFind == null || rule.prefabToReplace == null)
                continue;

            if(prefabAsset == rule.prefabToFind)
            {
                ReplaceObject(instanceRoot,rule.prefabToReplace);
                break;
            }
        }
    }

    void ReplaceObject(GameObject oldObj, GameObject newPrefab)
    {
        Transform parent = oldObj.transform.parent;
        int siblingIndex = oldObj.transform.GetSiblingIndex();

        GameObject newObj = (GameObject)PrefabUtility.InstantiatePrefab(newPrefab);

        newObj.transform.SetParent(parent);
        newObj.transform.SetSiblingIndex(siblingIndex);

        newObj.transform.position = oldObj.transform.position;
        newObj.transform.rotation = oldObj.transform.rotation;
        newObj.transform.localScale = oldObj.transform.localScale;

        objectsReplaced++;

        Undo.RegisterCreatedObjectUndo(newObj, "Replace Prefab");
        Undo.DestroyObjectImmediate(oldObj);
    }
}

[System.Serializable]
public class PrefabReplacementRule
{
    public GameObject prefabToFind;
    public GameObject prefabToReplace;
}
