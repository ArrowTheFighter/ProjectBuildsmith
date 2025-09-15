using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class BuildVersionPrompt : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        string currentVersion = PlayerSettings.bundleVersion;

        // If not set yet, initialize to 000
        if (string.IsNullOrEmpty(currentVersion))
            currentVersion = "000";

        // Ask if we should bump
        bool increase = EditorUtility.DisplayDialog(
            "Increase Game Version?",
            $"Current version: {currentVersion}\n\nDo you want to increase the build version?",
            "Yes, Increment",
            "No, Keep It"
        );

        if (increase)
        {
            // Parse current number (default 0 if not valid)
            int num = 0;
            int.TryParse(currentVersion, out num);

            num++; // Increment

            // Format with leading zeros (001, 002, …, 010, …)
            string newVersion = $"{num:000}";
            PlayerSettings.bundleVersion = newVersion;

            Debug.Log($"[BuildVersionPrompt] Version bumped to {newVersion}");
        }
    }
}
