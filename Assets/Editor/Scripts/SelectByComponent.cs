using UnityEngine;
using UnityEditor;

public class SelectByComponentName
{
    [MenuItem("Tools/Select All Polybrush Meshes")]
    static void SelectPolybrushMeshes()
    {
        var all = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        var list = new System.Collections.Generic.List<GameObject>();

        foreach (var go in all)
        {
            // Check if it has a component named "PolybrushMesh"
            if (go.GetComponent("PolybrushMesh") != null)
            {
                list.Add(go);
            }
        }

        Selection.objects = list.ToArray();
        Debug.Log("Selected " + list.Count + " objects with PolybrushMesh.");
    }
}
