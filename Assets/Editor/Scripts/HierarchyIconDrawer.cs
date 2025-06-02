using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class HierarchyIconDrawer
{
    static HierarchyIconDrawer()
    {
        EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
    }

    static void OnHierarchyGUI(int instanceID, Rect selectionRect)
    {
        GameObject obj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
        if (obj == null) return;

        HierarchyIconTag tag = obj.GetComponent<HierarchyIconTag>();
        if (tag == null || tag.icon == HierarchyIconTag.IconType.None) return;

        Texture2D icon = GetIcon(tag);
        if (icon == null) return;

        Rect iconRect = new Rect(selectionRect.xMax - 20f, selectionRect.y + 1f, 16f, 16f);
        GUI.DrawTexture(iconRect, icon, ScaleMode.ScaleToFit);
    }

    static Texture2D GetIcon(HierarchyIconTag tag)
    {
        switch (tag.icon)
        {
            case HierarchyIconTag.IconType.Star:
                return EditorGUIUtility.IconContent("Favorite").image as Texture2D;
            case HierarchyIconTag.IconType.Warning:
                return EditorGUIUtility.IconContent("console.warnicon").image as Texture2D;
            case HierarchyIconTag.IconType.Skull:
                return EditorGUIUtility.IconContent("console.erroricon").image as Texture2D;
            case HierarchyIconTag.IconType.Custom:
                return tag.customIcon;
            default:
                return null;
        }
    }
}
