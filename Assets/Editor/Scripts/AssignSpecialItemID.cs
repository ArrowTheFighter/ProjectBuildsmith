using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine.UIElements;
using Paps.UnityToolbarExtenderUIToolkit;
using DS.ScriptableObjects;
using System.Linq;


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

        AssignIDsToISavables();
        AssignIDsToSpecialItemPickups();
        AssignIDsToSaveObjectPositions();
        //AssignIDsToNPCTriggers();
        AssignIDsToDialogWorkers();
        AssignIDsToDialogSO();
        //AssignIDsToSaveEnabledState();
        //AssignIDsToNewCutsceneBuilders();
    }

    private static void AssignIDsToISavables()
    {
        ISaveable[] savablesArray = GameObject
            .FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None) // 'true' = include inactive
            .OfType<ISaveable>()                    // filter by interface
            .ToArray();
        HashSet<int> usedIDs = new HashSet<int>();

        // Gather existing non-zero IDs
        foreach (var saveable in savablesArray)
        {
            if (saveable.Get_Unique_ID != 0)
                usedIDs.Add(saveable.Get_Unique_ID);
        }

        int nextID = 1;

        foreach (var saveable in savablesArray)
        {
            if (saveable.Get_Unique_ID == 0)
            {
                while (usedIDs.Contains(nextID))
                    nextID++;

                saveable.Get_Unique_ID = nextID;
                usedIDs.Add(nextID);

                EditorUtility.SetDirty(saveable as MonoBehaviour);
                EditorSceneManager.MarkSceneDirty((saveable as MonoBehaviour).gameObject.scene);
                nextID++;
            }
        }

        Debug.Log($"Assigned unique IDs to {savablesArray.Length} ISavabales(s).");
    }

    [MenuItem("Tools/Reset ISavableIDs")]
    private static void ResetISavableIDs()
    {
        ISaveable[] savablesArray = GameObject
            .FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None) // 'true' = include inactive
            .OfType<ISaveable>()                    // filter by interface
            .ToArray();
        

        // Gather existing non-zero IDs
        foreach (var saveable in savablesArray)
        {
            saveable.Get_Unique_ID = 0;
                
        }

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

    private static void AssignIDsToSaveEnabledState()
    {
        SaveEnabledState[] saveEnabledStates = GameObject.FindObjectsByType<SaveEnabledState>(FindObjectsSortMode.None);
        HashSet<int> usedIDs = new HashSet<int>();

        // Collect existing IDs
        foreach (var obj in saveEnabledStates)
        {
            if (obj.unique_id != 0)
                usedIDs.Add(obj.unique_id);
        }

        int nextID = 1;

        foreach (var obj in saveEnabledStates)
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

        Debug.Log($"Assigned unique IDs to {saveEnabledStates.Length} SaveEnabledStates.");
    }

    private static void AssignIDsToNewCutsceneBuilders()
    {
        NewCutsceneBuilder[] cutsceneBuilders = GameObject.FindObjectsByType<NewCutsceneBuilder>(FindObjectsSortMode.None);
        HashSet<int> usedIDs = new HashSet<int>();

        // Collect existing IDs
        foreach (var obj in cutsceneBuilders)
        {
            if (obj.unique_id != 0)
                usedIDs.Add(obj.unique_id);
        }

        int nextID = 1;

        foreach (var obj in cutsceneBuilders)
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

        Debug.Log($"Assigned unique IDs to {cutsceneBuilders.Length} SaveEnabledStates.");
    }

    private static void AssignIDsToDialogSO()
    {
        // Load all DSCloseDialogSO and DSDialogSO from Resources folders
        var closeDialogs = Resources.LoadAll<DSCloseDialogSO>("");
        var dialogSOs = Resources.LoadAll<DSDialogueSO>("");

        // Combine into one array
        var allDialogs = new List<ScriptableObject>();
        allDialogs.AddRange(closeDialogs);
        allDialogs.AddRange(dialogSOs);

        HashSet<int> usedIDs = new HashSet<int>();

        // Collect all existing IDs
        foreach (var obj in allDialogs)
        {
            switch (obj)
            {
                case DSDialogueSO dialog when dialog.unique_id != 0:
                    usedIDs.Add(dialog.unique_id);
                    break;

                case DSCloseDialogSO closeDialog when closeDialog.unique_id != 0:
                    usedIDs.Add(closeDialog.unique_id);
                    break;
            }
        }

        int nextID = 1;
        int assignedCount = 0;

        // Assign IDs where missing
        foreach (var obj in allDialogs)
        {
            switch (obj)
            {
                case DSDialogueSO dialog when dialog.unique_id == 0:
                    while (usedIDs.Contains(nextID))
                        nextID++;

                    dialog.unique_id = nextID++;
                    usedIDs.Add(dialog.unique_id);
                    EditorUtility.SetDirty(dialog);
                    assignedCount++;
                    break;

                case DSCloseDialogSO closeDialog when closeDialog.unique_id == 0:
                    while (usedIDs.Contains(nextID))
                        nextID++;

                    closeDialog.unique_id = nextID++;
                    usedIDs.Add(closeDialog.unique_id);
                    EditorUtility.SetDirty(closeDialog);
                    assignedCount++;
                    break;
            }
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"✅ Assigned unique IDs to {assignedCount} Dialog ScriptableObjects ({allDialogs.Count} total).");
    }



}
