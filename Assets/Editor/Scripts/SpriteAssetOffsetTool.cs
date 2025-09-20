using UnityEditor;
using UnityEngine;
using TMPro;
using UnityEngine.TextCore;

public class SpriteAssetOffsetTool : EditorWindow
{
    TMP_SpriteAsset spriteAsset;
    float offsetY = -10f;

    [MenuItem("Tools/TMP/Adjust Sprite Asset Offsets")]
    static void Init()
    {
        SpriteAssetOffsetTool window = (SpriteAssetOffsetTool)EditorWindow.GetWindow(typeof(SpriteAssetOffsetTool));
        window.titleContent = new GUIContent("Adjust TMP Sprite Offsets");
        window.Show();
    }

    void OnGUI()
    {
        spriteAsset = (TMP_SpriteAsset)EditorGUILayout.ObjectField("Sprite Asset", spriteAsset, typeof(TMP_SpriteAsset), false);
        offsetY = EditorGUILayout.FloatField("BY Offset", offsetY);

        if (spriteAsset != null && GUILayout.Button("Apply Offset"))
        {
            ApplyOffset();
        }
    }

    void ApplyOffset()
    {
        Undo.RecordObject(spriteAsset, "Adjust Sprite Offsets");

        foreach (var glyph in spriteAsset.spriteGlyphTable)
        {
            var metrics = glyph.metrics;
            metrics = new GlyphMetrics(
                metrics.width,
                metrics.height,
                metrics.horizontalBearingX,
                metrics.horizontalBearingY + offsetY, // shift BY
                metrics.horizontalAdvance
            );
            glyph.metrics = metrics;
        }

        EditorUtility.SetDirty(spriteAsset);
        AssetDatabase.SaveAssets();
        Debug.Log($"Applied Y offset ({offsetY}) to all glyphs in {spriteAsset.name}");
    }
}
