using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using System.Linq;
using System.Collections;
using Newtonsoft.Json;
using Unity.Cinemachine;
using DS.ScriptableObjects;

public class SaveLoadManager : MonoBehaviour
{
    private static string FilePath => Path.Combine(Application.persistentDataPath + "/saves/");

    //Info to save
    [HideInInspector]
    public List<int> special_items_collected;
    [Header("Player Safe Zone")]
    public PlayerSafeZone playerSafeZone;
    [Header("Player Camera")]
    [SerializeField] public CinemachineOrbitalFollow OrbitalFollow;

    //SaveObjectPosition List
    [HideInInspector]
    public List<SaveObjectPosition> saveObjectPositions = new List<SaveObjectPosition>();


    //Dialog Workers
    [HideInInspector]
    public List<DialogWorker> DialogWorkers = new List<DialogWorker>();

    //Inventory Data Savers
    public List<InventoryDataSaver> inventoryDataSavers = new List<InventoryDataSaver>();

    //Saved Cutscenes
    [HideInInspector]
    public List<NewCutsceneBuilder> savedCustsceneBuilders = new List<NewCutsceneBuilder>();

    //ISavables
    public List<ISaveable> saveables = new List<ISaveable>();

    Queue<GameObject> ObjectsToDestroy = new Queue<GameObject>();
    bool isDeleting;

    SaveFileStruct saveFileStruct;

    public event Action<SaveFileStruct> OnSaveLoaded;

    void Start()
    {
        saveables = FindObjectsByType<MonoBehaviour>( FindObjectsInactive.Include,FindObjectsSortMode.None)
        .OfType<ISaveable>()
        .ToList();
    }


    public void AddSpecialItemCollected(int id)
    {
        special_items_collected.Add(id);
    }

    public void AddObjectsToDestroy(GameObject _object)
    {
        ObjectsToDestroy.Enqueue(_object);
        if(!isDeleting)
        {
            StartCoroutine(DestroyObjectsLoop());
        }
    }

    IEnumerator DestroyObjectsLoop()
    {
        isDeleting = true;
        int deletionsPerFrame = 15; // you can expose this as a field if you want

        while (ObjectsToDestroy.Count > 0)
        {
            // Delete up to N items per frame
            int count = 0;
            while (ObjectsToDestroy.Count > 0 && count < deletionsPerFrame)
            {
                var obj = ObjectsToDestroy.Dequeue();
                if (obj != null)
                    Destroy(obj);
                count++;
            }

            // Wait 1 frame before continuing
            yield return null;
        }
        isDeleting = false;
        Debug.Log("Finished deleting items");
    }

    [Button]
    public void LoadTest()
    {
        LoadFile("TestFile");
    }

    public void LoadFile(string save_name)
    {
        InsureFilePathExists();
        string SaveFile = Path.Combine(FilePath, save_name + ".txt");
        if (!File.Exists(SaveFile))
        {
            print($"no save file for file name {save_name} found.");
            return;
        }
        string json = File.ReadAllText(SaveFile);
        saveFileStruct = JsonConvert.DeserializeObject<SaveFileStruct>(json);

        ScriptRefrenceSingleton.instance.timeManager.SetTime(saveFileStruct.saved_time);

        special_items_collected = saveFileStruct.special_items_collected.ToList();

        //Load Unlocked Recipes
        ScriptRefrenceSingleton.instance.gameplayUtils.inventoryManager.UnlockedRecipes = saveFileStruct.unlocked_recipes;

        //Quests
        foreach (var quest in saveFileStruct.saved_quests)
        {
            ScriptRefrenceSingleton.instance.gameplayUtils.inventoryManager.LoadSavedQuest(quest);
        } 

        //Flags
        foreach (var flag in saveFileStruct.Flags)
        {
            if (flag.Value) FlagManager.Set_Flag(flag.Key);
        }

        //Player Position
        playerSafeZone.transform.position = saveFileStruct.player_position.ToVector3();
        playerSafeZone.transform.eulerAngles = saveFileStruct.player_rotation.ToVector3();

        //Player Camera
        OrbitalFollow.HorizontalAxis.Value = saveFileStruct.camera_x;
        OrbitalFollow.VerticalAxis.Value = saveFileStruct.camera_y;


        //Load Save_Object_Positions and Rotations
        foreach (var saveObject in saveObjectPositions)
        {
            if (saveFileStruct.SaveObjectPositions.TryGetValue(saveObject.SaveObjectID, out var pos))
                saveObject.transform.position = pos.ToVector3();
            if (saveFileStruct.SaveObjectRotations.TryGetValue(saveObject.SaveObjectID, out var rotation))
                saveObject.transform.eulerAngles = rotation.ToVector3();
        }

        //Load Dialog Workers
        foreach (var worker in DialogWorkers)
        {
            if (saveFileStruct.dialog_worker_current_dialogs.TryGetValue(worker.unique_id, out var dialog_id))
                worker.SetCurrentDialogByID(dialog_id);
            if (saveFileStruct.dialog_worker_has_marker.Contains(worker.unique_id))
                worker.EnableMarker(true);
        }


        //load ISavables
        foreach(var saveable in saveables)
        {
            if (saveFileStruct.saveable_ids.Contains(saveable.Get_Unique_ID))
                saveable.SaveLoaded(saveFileStruct);
        }

        OnSaveLoaded?.Invoke(saveFileStruct);
        print("Loaded save file");

    }

    [Button]
    public void SaveTest()
    {
        SaveFile("TestFile");
    }


    public void SaveFile(string save_name)
    {
        InsureFilePathExists();
        string SaveFile = Path.Combine(FilePath, save_name + ".txt");
        SaveFileStruct saveFile = new SaveFileStruct();

        saveFile.file_name = save_name;

        saveFile.saved_time = ScriptRefrenceSingleton.instance.timeManager.GetTime();

        //Inventory saving
        saveFile.special_items_collected = special_items_collected.ToArray();

        saveFile.special_items = ScriptRefrenceSingleton.instance.gameplayUtils.inventoryManager.specialItems;
        saveFile.inventory_slots = new List<SaveableInventroySlot>();
        foreach (InventorySlot slot in ScriptRefrenceSingleton.instance.gameplayUtils.inventoryManager.inventorySlots)
        {
            saveFile.inventory_slots.Add(new SaveableInventroySlot(slot.isEmpty, slot.slot_id, slot.inventoryItemStack));
        }

        //Unlocked Recipes
        saveFile.unlocked_recipes = ScriptRefrenceSingleton.instance.gameplayUtils.inventoryManager.UnlockedRecipes;

        //Quests
        foreach(var quest in ScriptRefrenceSingleton.instance.gameplayUtils.inventoryManager.activeQuests)
        {

            bool quest_is_pinned = ScriptRefrenceSingleton.instance.gameplayUtils.inventoryManager.activedPinnedQuests
                .Any(item => item.storedQuestData.ID == quest.QuestData.ID);

            SaveableQuestInfo saveableQuestInfo = new SaveableQuestInfo(quest, quest_is_pinned);

            foreach (QuestObjective questObjective in quest.QuestData.questObjectives)
            {
                saveableQuestInfo.completed_objectives.Add(questObjective.isComplete);
            }

            saveFile.saved_quests.Add(saveableQuestInfo);
            
            
        }

        //Player position
        saveFile.player_position = new SerializableVector3(playerSafeZone.safePos);
        saveFile.player_rotation = new SerializableVector3(playerSafeZone.transform.eulerAngles);

        //Player camera
        saveFile.camera_x = OrbitalFollow.HorizontalAxis.Value;
        saveFile.camera_y = OrbitalFollow.VerticalAxis.Value;

        //Save Object Positions
        foreach (SaveObjectPosition saveObjectPosition in saveObjectPositions)
        {
            saveFile.SaveObjectPositions.Add(saveObjectPosition.SaveObjectID, new SerializableVector3(saveObjectPosition.transform.position));
            saveFile.SaveObjectRotations.Add(saveObjectPosition.SaveObjectID, new SerializableVector3(saveObjectPosition.transform.eulerAngles));
        }

        //Flags
        saveFile.Flags = FlagManager.Get_Flag_Dictionary();

        //Dialog Workers
        saveFile = SaveDialogIDs(saveFile);

        //ISaveables
        foreach (var saveable in saveables)
        {
            if (saveable.Get_Should_Save)
            {
                saveFile.saveable_ids.Add(saveable.Get_Unique_ID);
            }
        }

        //Inventory Data Savers
        foreach(var inventory in inventoryDataSavers)
        {
            saveFile.saved_inventory_savers.Add(new SerializableInventory(inventory.unique_id, inventory.savedSlots));
        }

        string json = JsonConvert.SerializeObject(saveFile, Formatting.Indented); // true = pretty print
        File.WriteAllText(SaveFile, json);
        Debug.Log("Saved settings to " + SaveFile);
    }
    
    SaveFileStruct SaveDialogIDs(SaveFileStruct saveFileStruct)
    {
        foreach (var worker in DialogWorkers)
        {
            if (worker.hasMarker) saveFileStruct.dialog_worker_has_marker.Add(worker.unique_id);
            int dialog_id = -1;
            switch (worker.currentDialogSO)
            {
                case DSDialogueSO dSDialogueSO:
                    dialog_id = dSDialogueSO.unique_id;
                    break;
                case DSCloseDialogSO dSCloseDialogSO:
                    dialog_id = dSCloseDialogSO.unique_id;
                    break;
            }
            if (dialog_id != -1)
            {
                saveFileStruct.dialog_worker_current_dialogs.Add(worker.unique_id, dialog_id);
            }
        }
        return saveFileStruct;
    }

    void InsureFilePathExists()
    {
        if (!Directory.Exists(Application.persistentDataPath + "/saves"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/saves");
        }
    }
}

[Serializable]
public class SaveFileStruct
{

    public SaveFileStruct()
    {
        SaveObjectPositions = new Dictionary<int, SerializableVector3>();
        SaveObjectRotations = new Dictionary<int, SerializableVector3>();
        Flags = new Dictionary<string, bool>();
        dialog_worker_current_dialogs = new Dictionary<int, int>();
        dialog_worker_has_marker = new List<int>();
        saveable_ids = new List<int>();
        saved_quests = new List<SaveableQuestInfo>();
        saved_inventory_savers = new List<SerializableInventory>();
        unlocked_recipes = new List<string>();
    }

    public string file_name;
    public int[] special_items_collected;

    public float saved_time;

    //Inventory
    public Dictionary<string, int> special_items;
    public List<SaveableInventroySlot> inventory_slots;
    public List<string> unlocked_recipes;

    //Quests
    public List<SaveableQuestInfo> saved_quests;

    //Player position
    public SerializableVector3 player_position;
    public SerializableVector3 player_rotation;

    //Player Camera
    public float camera_x;
    public float camera_y;

    //SaveObjectPositions
    public Dictionary<int, SerializableVector3> SaveObjectPositions;
    public Dictionary<int, SerializableVector3> SaveObjectRotations;

    //Flags
    public Dictionary<string, bool> Flags;

    //Dialog Workers
    public Dictionary<int, int> dialog_worker_current_dialogs;
    public List<int> dialog_worker_has_marker;


    //SavableIDS
    public List<int> saveable_ids;

    //Inventory Data Savers
    public List<SerializableInventory> saved_inventory_savers;


}

[Serializable]
public struct SerializableVector3
{
    public float x, y, z;

    public SerializableVector3(Vector3 v3)
    {
        x = v3.x;
        y = v3.y;
        z = v3.z;
    }

    public Vector3 ToVector3() => new Vector3(x, y, z);
}

[Serializable]
public struct SaveableQuestInfo
{
    public string quest_id;
    public bool is_pinned;
    public bool is_complete;
    public List<bool> completed_objectives;

    public SaveableQuestInfo(QuestInfo questInfo, bool isComplete = false)
    {
        quest_id = questInfo.QuestData.ID;
        is_complete = questInfo.IsComplete;
        is_pinned = isComplete;
        completed_objectives = new List<bool>();
    }
}

[Serializable]
public class SerializableInventory
{
    public int inventory_id;
    public List<SaveableInventroySlot> saved_inventory_slots;

    public SerializableInventory() {}

    public SerializableInventory(int id, List<InventorySlot> slots)
    {
        inventory_id = id;
        saved_inventory_slots = new List<SaveableInventroySlot>();
        foreach (var slot in slots)
        {
            saved_inventory_slots.Add(new SaveableInventroySlot(slot));
        }
    }

}