using UnityEngine;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ThumbnailGenerator : MonoBehaviour
{
    public Camera thumbnailCamera;
    public RenderTexture renderTexture;

    public void CaptureThumbnail(GameObject prefab, string savePath)
    {
        // Instantiate in front of camera
        GameObject instance = null;
        if (prefab != null)
        {
            instance = Instantiate(prefab);
            instance.transform.position = Vector3.zero;
            instance.transform.rotation = Quaternion.identity;
        }

        // Ensure prefab layer doesn't affect the scene (optional)
        //instance.layer = LayerMask.NameToLayer("Thumbnail");

        // Set camera to render only the thumbnail layer
        //thumbnailCamera.cullingMask = LayerMask.GetMask("Thumbnail");

        // Render
        thumbnailCamera.targetTexture = renderTexture;
        thumbnailCamera.Render();

        // Read pixels
        RenderTexture.active = renderTexture;
        Texture2D tex = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);
        tex.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        tex.Apply();

        // Save PNG
        byte[] bytes = tex.EncodeToPNG();
        File.WriteAllBytes(savePath, bytes);

#if UNITY_EDITOR
        // Convert to relative path
        print(savePath);
        //string relativePath = savePath.Substring(Application.dataPath.Length);

        // Refresh asset database
        AssetDatabase.ImportAsset(savePath);

        // Set import settings to Sprite
        TextureImporter importer = AssetImporter.GetAtPath(savePath) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.alphaIsTransparency = true;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.SaveAndReimport();
        }
    #endif

        Debug.Log("Saved thumbnail to " + savePath);

        // Clean up
        RenderTexture.active = null;
        DestroyImmediate(tex);
        if (instance != null)
        {
            DestroyImmediate(instance);
        }
    }
}
