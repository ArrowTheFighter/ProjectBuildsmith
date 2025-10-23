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
        text = "Assign Unique entity IDs";
        clicked += () => AssignIDsInScene();
    }

    [MenuItem("Tools/Assign Unique entity IDs")]
    private static void AssignIDsInScene()
    {

        bool run = EditorUtility.DisplayDialog(
                "Assign Unique IDs?",
                "Do you want to automatically assign unique IDs to all components with ID 0 in this scene?",
                "Yes",
                "No"
            );


        if (!run) return;

        AssignIDsToSpecialItemPickups();
        AssignIDsToSaveObjectPositions();
        AssignIDsToNPCTriggers();
    }

    private static void AssignIDsToSpecialItemPickups()
    {
        SpecialItemPickup[] pickups = GameObject.FindObjectsByType<SpecialItemPickup>(FindObjectsSortMode.None);
        HashSet<int> usedIDs = new HashSet<int>();

        // Gather existing non-zero IDs
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
                while (usedIDs.Contains(nextID))
                    nextID++;

                pickup.pickup_id = nextID;
                usedIDs.Add(nextID);

                EditorUtility.SetDirty(pickup);
                EditorSceneManager.MarkSceneDirty(pickup.gameObject.scene);
                nextID++;
            }
        }

        Debug.Log($"Assigned unique IDs to {pickups.Length} SpecialItemPickup(s).");
    }

    private static void AssignIDsToSaveObjectPositions()
    {
        SaveObjectPosition[] saveObjects = GameObject.FindObjectsByType<SaveObjectPosition>(FindObjectsSortMode.None);
        HashSet<int> usedIDs = new HashSet<int>();

        // Collect existing IDs
        foreach (var obj in saveObjects)
        {
            if (obj.SaveObjectID != 0)
                usedIDs.Add(obj.SaveObjectID);
        }

        int nextID = 1;

        foreach (var obj in saveObjects)
        {
            if (obj.SaveObjectID == 0)
            {
                while (usedIDs.Contains(nextID))
                    nextID++;

                obj.SaveObjectID = nextID;
                usedIDs.Add(nextID);

                EditorUtility.SetDirty(obj);
                EditorSceneManager.MarkSceneDirty(obj.gameObject.scene);
                nextID++;
            }
        }

        Debug.Log($"Assigned unique IDs to {saveObjects.Length} SaveObjectPositions.");
    }

    private static void AssignIDsToNPCTriggers()
    {
        NPCTriggers[] triggers = GameObject.FindObjectsByType<NPCTriggers>(FindObjectsSortMode.None);
        HashSet<int> usedIDs = new HashSet<int>();

        // Collect existing IDs
        foreach (var obj in triggers)
        {
            if (obj.unique_id != 0)
                usedIDs.Add(obj.unique_id);
        }

        int nextID = 1;

        foreach (var obj in triggers)
        {
            if (obj.unique_id == 0)
            {
                while (usedIDs.Contains(nextID))
                    nextID++;

                obj.unique_id = nextID;
                usedIDs.Add(nextID);

                EditorUtility.SetDirty(obj);
                EditorSceneManager.MarkSceneDirty(obj.gameObject.scene);
                nextID++;
            }
        }

        Debug.Log($"Assigned unique IDs to {triggers.Length} NPCTriggers.");
    }

    private static void AssignIDsToDialogWorkers()
    {
        DialogWorker[] dialogWorkers = GameObject.FindObjectsByType<DialogWorker>(FindObjectsSortMode.None);
        HashSet<int> usedIDs = new HashSet<int>();

        // Collect existing IDs
        foreach (var obj in dialogWorkers)
        {
            if (obj.unique_id != 0)
                usedIDs.Add(obj.unique_id);
        }

        int nextID = 1;

        foreach (var obj in dialogWorkers)
        {
            if (obj.unique_id == 0)
            {
                while (usedIDs.Contains(nextID))
                    nextID++;

                obj.unique_id = nextID;
                usedIDs.Add(nextID);

                EditorUtility.SetDirty(obj);
                EditorSceneManager.MarkSceneDirty(obj.gameObject.scene);
                nextID++;
            }
        }

        Debug.Log($"Assigned unique IDs to {dialogWorkers.Length} DialogWorkers.");
    }

}
