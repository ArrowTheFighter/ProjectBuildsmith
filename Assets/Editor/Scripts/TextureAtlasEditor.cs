using UnityEngine;
using UnityEditor;

public class TextureAtlasEditor : EditorWindow
{
    Texture2D atlasTexture;
    int cols = 8;
    int rows = 4;

    int cellWidth;
    int cellHeight;

    int selectedCellX = -1;
    int selectedCellY = -1;

    Color cellTintColor = Color.white;

    Color[] selectedCellPixels;
    Color[] originalCellPixels;  // Store original unmodified pixels for reset

    Vector2 scrollPos;

    float brightnessMultiplier = 1f;

    [MenuItem("Tools/Texture Atlas Editor")]
    public static void ShowWindow()
    {
        GetWindow<TextureAtlasEditor>("Texture Atlas Editor");
    }

    void OnGUI()
    {
        GUILayout.Label("Texture Atlas Editor (8x4 Grid)", EditorStyles.boldLabel);

        atlasTexture = (Texture2D)EditorGUILayout.ObjectField("Atlas Texture", atlasTexture, typeof(Texture2D), false);

        if (atlasTexture == null)
        {
            EditorGUILayout.HelpBox("Assign a texture atlas.", MessageType.Info);
            return;
        }

        if (atlasTexture.width % cols != 0 || atlasTexture.height % rows != 0)
        {
            EditorGUILayout.HelpBox($"Texture size should be divisible by {cols}x{rows}. Current size: {atlasTexture.width}x{atlasTexture.height}", MessageType.Warning);
        }

        cellWidth = atlasTexture.width / cols;
        cellHeight = atlasTexture.height / rows;

        DrawAtlasGrid();

        if (selectedCellX >= 0 && selectedCellY >= 0)
        {
            EditorGUILayout.Space();
            GUILayout.Label($"Selected Cell: ({selectedCellX}, {selectedCellY})");

            if (selectedCellPixels == null)
            {
                LoadSelectedCellPixels();
            }

            // Show a tint color picker
            cellTintColor = EditorGUILayout.ColorField("Cell Tint Color", cellTintColor);
            brightnessMultiplier = EditorGUILayout.Slider("Brightness Multiplier", brightnessMultiplier, 0f, 2f);
            if (GUILayout.Button("Apply Tint to Cell"))
            {
                ApplyTintToCell();
            }

            if (GUILayout.Button("Reset Tint (Reload Cell)"))
            {
                if (originalCellPixels != null && atlasTexture != null && selectedCellX >= 0 && selectedCellY >= 0)
                {
                    // Restore original pixels to the atlas texture
                    atlasTexture.SetPixels(selectedCellX * cellWidth, selectedCellY * cellHeight, cellWidth, cellHeight, originalCellPixels);
                    atlasTexture.Apply();

                    // Update selectedCellPixels to original as well
                    selectedCellPixels = (Color[])originalCellPixels.Clone();
                    cellTintColor = Color.white;

                    Repaint();
                }
            }

            EditorGUILayout.Space();

            // Show the selected cell pixels zoomed in
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(cellHeight + 20), GUILayout.Width(cellWidth + 20));
            DrawSelectedCellPreview();
            EditorGUILayout.EndScrollView();
        }
    }

    void DrawAtlasGrid()
    {
        float scale = 0.5f; // Scale down to half size

        // Calculate scaled size of atlas
        float scaledWidth = atlasTexture.width * scale;
        float scaledHeight = atlasTexture.height * scale;

        // Draw the texture scaled down
        Rect textureRect = GUILayoutUtility.GetRect(scaledWidth, scaledHeight, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false));
        EditorGUI.DrawPreviewTexture(textureRect, atlasTexture);

        // Draw grid overlay scaled
        Handles.BeginGUI();
        Color prevColor = Handles.color;
        Handles.color = Color.white;

        // Scaled cell size
        float scaledCellWidth = cellWidth * scale;
        float scaledCellHeight = cellHeight * scale;

        // Draw vertical grid lines
        for (int i = 0; i <= cols; i++)
        {
            float x = textureRect.x + i * scaledCellWidth;
            Handles.DrawLine(new Vector3(x, textureRect.y), new Vector3(x, textureRect.y + scaledHeight));
        }

        // Draw horizontal grid lines
        for (int j = 0; j <= rows; j++)
        {
            float y = textureRect.y + j * scaledCellHeight;
            Handles.DrawLine(new Vector3(textureRect.x, y), new Vector3(textureRect.x + scaledWidth, y));
        }

        Handles.color = prevColor;
        Handles.EndGUI();

        // Handle mouse click inside scaled texture
        Event e = Event.current;
        if (e.type == EventType.MouseDown && e.button == 0)
        {
            Vector2 mousePos = e.mousePosition;
            if (textureRect.Contains(mousePos))
            {
                // Calculate local position relative to textureRect
                float localX = mousePos.x - textureRect.x;
                float localY = mousePos.y - textureRect.y;

                // Convert local pos back to original cell indices by dividing by scaled cell size
                int cellX = (int)(localX / scaledCellWidth);
                // Invert Y because texture origin is bottom-left, GUI origin top-left
                int cellY = rows - 1 - (int)(localY / scaledCellHeight);

                if (cellX >= 0 && cellX < cols && cellY >= 0 && cellY < rows)
                {
                    selectedCellX = cellX;
                    selectedCellY = cellY;
                    LoadSelectedCellPixels();
                    cellTintColor = Color.white;
                    Repaint();
                }
            }
        }
    }

    void LoadSelectedCellPixels()
    {
        if (atlasTexture == null || selectedCellX < 0 || selectedCellY < 0)
            return;

        originalCellPixels = atlasTexture.GetPixels(selectedCellX * cellWidth, selectedCellY * cellHeight, cellWidth, cellHeight);
        selectedCellPixels = (Color[])originalCellPixels.Clone();
    }

    void ApplyTintToCell()
    {
        if (selectedCellPixels == null || atlasTexture == null)
            return;

        Color.RGBToHSV(cellTintColor, out float tintH, out float tintS, out float tintV);

        for (int i = 0; i < selectedCellPixels.Length; i++)
        {
            Color pixel = selectedCellPixels[i];

            Color.RGBToHSV(pixel, out float h, out float s, out float v);

            // Adjust brightness by slider
            float newV = Mathf.Clamp01(v * brightnessMultiplier);

            Color tintedPixel = Color.HSVToRGB(tintH, tintS, newV);
            tintedPixel.a = pixel.a;

            selectedCellPixels[i] = tintedPixel;
        }

        atlasTexture.SetPixels(selectedCellX * cellWidth, selectedCellY * cellHeight, cellWidth, cellHeight, selectedCellPixels);
        atlasTexture.Apply();

        Repaint();
    }


    void DrawSelectedCellPreview()
    {
        if (selectedCellPixels == null || selectedCellPixels.Length == 0)
            return;

        // Create a temporary texture to show the selected cell pixels
        Texture2D tempTex = new Texture2D(cellWidth, cellHeight, TextureFormat.RGBA32, false);
        tempTex.SetPixels(selectedCellPixels);
        tempTex.Apply();

        // Draw with zoom (e.g. 0.1x)
        float zoom = 0.1f;
        Rect previewRect = GUILayoutUtility.GetRect(cellWidth * zoom, cellHeight * zoom);
        EditorGUI.DrawPreviewTexture(previewRect, tempTex);

        DestroyImmediate(tempTex);
    }
}
