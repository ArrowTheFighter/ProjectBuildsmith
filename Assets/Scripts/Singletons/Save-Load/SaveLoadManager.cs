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
    public static SaveLoadManager instance;
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

    //NPC Triggers
    // [HideInInspector]
    // public List<NPCTriggers> NPCTriggers = new List<NPCTriggers>();

    //Dialog Workers
    [HideInInspector]
    public List<DialogWorker> DialogWorkers = new List<DialogWorker>();

    //SaveEnabledStates
    // [HideInInspector]
    // public List<SaveEnabledState> saveEnabledStates = new List<SaveEnabledState>();

    //Saved Cutscenes
    [HideInInspector]
    public List<NewCutsceneBuilder> savedCustsceneBuilders = new List<NewCutsceneBuilder>();

    //ISavables
    public List<ISaveable> saveables = new List<ISaveable>();

    Queue<GameObject> ObjectsToDestroy = new Queue<GameObject>();
    bool isDeleting;

    SaveFileStruct saveFileStruct;

    public event Action<SaveFileStruct> OnSaveLoaded;

    void Awake()
    {
        if (instance != this)
        {
            Destroy(instance);
        }
        instance = this;
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
        special_items_collected = saveFileStruct.special_items_collected.ToList();

        //Load SaveEnabledStates
        // foreach (var state in saveEnabledStates)
        // {
        //     state.gameObject.SetActive(!saveFileStruct.save_enabled_state_ids.Contains(state.unique_id));
        // }

        //Flags
        foreach (var flag in saveFileStruct.Flags)
        {
            if (flag.Value) FlagManager.Set_Flag(flag.Key);
        }

        //NPC Triggers
        // foreach (var trigger in NPCTriggers)
        // {
        //     if (saveFileStruct.activated_trigers.Contains(trigger.unique_id))
        //         trigger.Activated = true;

        // }

        //Player Position
        playerSafeZone.transform.position = saveFileStruct.player_position.ToVector3();

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



        //load Saved Cutscenes
        // foreach (var cutscene in savedCustsceneBuilders)
        // {
        //     if (saveFileStruct.saved_cutscene_ids.Contains(cutscene.unique_id))
        //     {
        //         cutscene.PlayCutscene();
        //         CutsceneManager.instance.SkipCutscene();
        //     }
        // }

        //load ISavables
        foreach(var saveable in saveables)
        {
            if (saveFileStruct.saveable_ids.Contains(saveable.Get_Unique_ID))
                saveable.SaveLoaded(saveFileStruct);
        }

        OnSaveLoaded?.Invoke(saveFileStruct);
        print("Loaded save file");
        print(saveFileStruct.special_items_collected.Length);

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
        saveFile.special_items_collected = special_items_collected.ToArray();
        //Inventory saving
        saveFile.special_items = GameplayUtils.instance.inventoryManager.specialItems;
        saveFile.inventory_slots = new List<SaveableInventroySlot>();
        foreach (InventorySlot slot in GameplayUtils.instance.inventoryManager.inventorySlots)
        {
            saveFile.inventory_slots.Add(new SaveableInventroySlot(slot.isEmpty, slot.slot_id, slot.inventoryItemStack));
        }

        //Player position
        saveFile.player_position = new SerializableVector3(playerSafeZone.safePos);

        //Player camera
        saveFile.camera_x = OrbitalFollow.HorizontalAxis.Value;
        saveFile.camera_y = OrbitalFollow.VerticalAxis.Value;

        //Save Object Positions
        foreach (SaveObjectPosition saveObjectPosition in saveObjectPositions)
        {
            saveFile.SaveObjectPositions.Add(saveObjectPosition.SaveObjectID, new SerializableVector3(saveObjectPosition.transform.position));
            saveFile.SaveObjectRotations.Add(saveObjectPosition.SaveObjectID, new SerializableVector3(saveObjectPosition.transform.eulerAngles));
        }

        //NPC Triggers
        // foreach (var trigger in NPCTriggers)
        // {
        //     if (trigger.Activated) saveFile.activated_trigers.Add(trigger.unique_id);
        // }

        //Flags
        saveFile.Flags = FlagManager.Get_Flag_Dictionary();

        //Dialog Workers
        saveFile = SaveDialogIDs(saveFile);

        //SaveEnableStates
        // foreach (var enableState in saveEnabledStates)
        // {
        //     if (!enableState.gameObject.activeInHierarchy)
        //     {
        //         saveFile.save_enabled_state_ids.Add(enableState.unique_id);
        //     }
        // }

        //Save Played Cutscenes
        // foreach (var cutscene in savedCustsceneBuilders)
        // {
        //     if (cutscene.save_played && cutscene.has_played)
        //     {
        //         saveFile.saved_cutscene_ids.Add(cutscene.unique_id);
        //     }
        // }
        //Save ISavables
        foreach(var saveable in saveables)
        {
            if(saveable.Get_Should_Save)
            {
                saveFile.saveable_ids.Add(saveable.Get_Unique_ID);
            }
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
        //activated_trigers = new List<int>();
        Flags = new Dictionary<string, bool>();
        dialog_worker_current_dialogs = new Dictionary<int, int>();
        dialog_worker_has_marker = new List<int>();
        //save_enabled_state_ids = new List<int>();
        saveable_ids = new List<int>();
    }

    public string file_name;
    public int[] special_items_collected;

    //Inventory
    public Dictionary<string, int> special_items;
    public List<SaveableInventroySlot> inventory_slots;

    //Player position
    public SerializableVector3 player_position;

    //Player Camera
    public float camera_x;
    public float camera_y;

    //SaveObjectPositions
    public Dictionary<int, SerializableVector3> SaveObjectPositions;
    public Dictionary<int, SerializableVector3> SaveObjectRotations;

    //NPC Triggers
    //public List<int> activated_trigers;

    //Flags
    public Dictionary<string, bool> Flags;

    //Dialog Workers
    public Dictionary<int, int> dialog_worker_current_dialogs;
    public List<int> dialog_worker_has_marker;

    //Disabled_Objs
    //public List<int> save_enabled_state_ids;

    //Saved Cutscene ID's
    //public List<int> saved_cutscene_ids;

    //SavableIDS
    public List<int> saveable_ids;

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
