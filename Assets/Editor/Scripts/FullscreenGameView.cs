#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;
using System.Reflection;

[InitializeOnLoad]
public static class EditorFullscreenGameView
{
    static EditorWindow fullscreenWindow;
    static Type gameViewType;
    static PropertyInfo showToolbarProperty;

    static EditorFullscreenGameView()
    {
        gameViewType = typeof(EditorWindow).Assembly.GetType("UnityEditor.GameView");
        showToolbarProperty = gameViewType?.GetProperty("showToolbar", BindingFlags.Instance | BindingFlags.NonPublic);
    }

    [MenuItem("Tools/Toggle Fullscreen GameView %#&2")]
    public static void ToggleFullscreen()
    {
        if (fullscreenWindow != null)
        {
            fullscreenWindow.Close();
            fullscreenWindow = null;
            SetGameViewTargetDisplay(0); // back to display 0
        }
        else
        {
            SetGameViewTargetDisplay(1); // hide main Game View

            fullscreenWindow = ScriptableObject.CreateInstance(gameViewType) as EditorWindow;

            if (showToolbarProperty != null)
                showToolbarProperty.SetValue(fullscreenWindow, false);

            var res = Screen.currentResolution;
            Rect fullscreenRect = new Rect(0, 0, res.width, res.height);
            float scale = EditorGUIUtility.pixelsPerPoint;
            fullscreenRect.width /= scale;
            fullscreenRect.height /= scale;


            fullscreenWindow.ShowPopup();
            fullscreenWindow.position = fullscreenRect;
            fullscreenWindow.Focus();
        }
    }

    [MenuItem("Tools/Reset Layout %#&3")]
    public static void ResetLayout()
    {
        EditorApplication.ExecuteMenuItem("Window/Layouts/Default");
        fullscreenWindow = null;
    }

    static void SetGameViewTargetDisplay(int index)
    {
        var gameView = GetMainGameView();
        var method = gameView.GetType().GetMethod("SetTargetDisplay", BindingFlags.Instance | BindingFlags.NonPublic);
        method?.Invoke(gameView, new object[] { index });
    }

    static EditorWindow GetMainGameView()
    {
        return EditorWindow.GetWindow(gameViewType);
    }
}
#endif
