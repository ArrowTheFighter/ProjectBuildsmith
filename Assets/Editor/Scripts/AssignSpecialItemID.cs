using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine.UIElements;
using Paps.UnityToolbarExtenderUIToolkit;


[MainToolbarElement(id: "AssignSpecialItemsButton")]
public class AssignSpecialItemID : Button
{

    public void InitializeElement()
    {
        text = "Assign Special Item IDs";
        clicked += () => AssignIDsInScene();
    }

    [MenuItem("Tools/Assign Special IDs")]
    private static void AssignIDsInScene()
    {

        bool run = EditorUtility.DisplayDialog(
                "Assign Unique IDs?",
                "Do you want to automatically assign unique IDs to SpecialItemPickup components with ID 0 in this scene?",
                "Yes",
                "No"
            );


        if (!run) return;
        // Find all SpecialItemPickup components in the active scene
        SpecialItemPickup[] pickups = GameObject.FindObjectsByType<SpecialItemPickup>(FindObjectsSortMode.None);

        HashSet<int> usedIDs = new HashSet<int>();

        // Collect all existing non-zero IDs
        foreach (var pickup in pickups)
        {
            if (pickup.pickup_id != 0)
                usedIDs.Add(pickup.pickup_id);
        }

        int nextID = 1;

        foreach (var pickup in pickups)
        {
            if (pickup.pickup_id == 0)
            {
                // Find the next available ID
                while (usedIDs.Contains(nextID))
                    nextID++;

                pickup.pickup_id = nextID;
                usedIDs.Add(nextID);

                // Mark scene dirty so changes are saved
                EditorUtility.SetDirty(pickup);
                EditorSceneManager.MarkSceneDirty(pickup.gameObject.scene);
                nextID++;
            }
        }

        Debug.Log($"Assigned unique IDs to {pickups.Length} SpecialItemPickup(s) in the scene.");
    }
}
