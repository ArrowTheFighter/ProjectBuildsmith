#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

public class ThumbnailBatcher : EditorWindow
{
    public ThumbnailGenerator thumbnailGenerator;
    public string prefabFolder = "Assets/Prefabs/Resources"; // Change this as needed
    public string saveFolder = "Assets/Art/ItemThumbnails"; // Output folder for PNGs

    public string singleFileName;
    public RenderTexture previewTexture;

    [MenuItem("Tools/Generate Prefab Thumbnails")]
    public static void ShowWindow()
    {
        GetWindow<ThumbnailBatcher>("Thumbnail Generator");
    }

    void OnGUI()
    {
        GUILayout.Label("Thumbnail Settings", EditorStyles.boldLabel);

        thumbnailGenerator = (ThumbnailGenerator)EditorGUILayout.ObjectField("Thumbnail Generator", thumbnailGenerator, typeof(ThumbnailGenerator), true);
        prefabFolder = EditorGUILayout.TextField("Prefab Folder", prefabFolder);
        saveFolder = EditorGUILayout.TextField("Save Folder", saveFolder);

        if (GUILayout.Button("Auto Generate All Thumbnails"))
        {
            GenerateThumbnails();
        }
        singleFileName = EditorGUILayout.TextField("ItemName", singleFileName);
        if (GUILayout.Button("Generate Single Thumbnail"))
        {
            GenerateSingleThumbnail();
        }
        if (thumbnailGenerator != null && thumbnailGenerator.renderTexture != null && previewTexture == null)
        {
            previewTexture = thumbnailGenerator.renderTexture;
        }
        else
        {
            previewTexture = (RenderTexture)EditorGUILayout.ObjectField("Render Texture", previewTexture, typeof(RenderTexture), true);
        }

        if (previewTexture != null)
        {
            GUILayout.Label("Preview", EditorStyles.boldLabel);
            Rect previewRect = GUILayoutUtility.GetAspectRect(1.0f); // square preview
            EditorGUI.DrawPreviewTexture(previewRect, previewTexture);
        }
    }

    void GenerateSingleThumbnail()
    {
        if (thumbnailGenerator == null)
        {
            Debug.LogError("ThumbnailGenerator reference is missing.");
            return;
        }

        if (!Directory.Exists(saveFolder))
        {
            Directory.CreateDirectory(saveFolder);
        }

        string savePath = Path.Combine(saveFolder, singleFileName + ".png");
        thumbnailGenerator.CaptureThumbnail(null, savePath);
        AssetDatabase.Refresh();
        Debug.Log("Saved Single Thumbnail.");
    }

    void GenerateThumbnails()
    {
        if (thumbnailGenerator == null)
        {
            Debug.LogError("ThumbnailGenerator reference is missing.");
            return;
        }

        // Create the output directory if it doesn't exist
        if (!Directory.Exists(saveFolder))
        {
            Directory.CreateDirectory(saveFolder);
        }

        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { prefabFolder });

        foreach (string guid in prefabGuids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

            if (prefab != null)
            {
                string fileName = Path.GetFileNameWithoutExtension(assetPath) + ".png";
                string savePath = Path.Combine(saveFolder, fileName);

                // Capture thumbnail
                thumbnailGenerator.CaptureThumbnail(prefab, savePath);
            }
        }

        AssetDatabase.Refresh();
        Debug.Log("Finished generating thumbnails.");
    }
}
#endif
