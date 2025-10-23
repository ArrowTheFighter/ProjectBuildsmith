using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using System.Linq;
using System.Collections;
using Newtonsoft.Json;
using Unity.Cinemachine;
using System.Reflection;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager instance;
    private static string FilePath => Path.Combine(Application.persistentDataPath + "/saves/");

    //Info to save
    public List<int> special_items_collected;
    [Header("Player Safe Zone")]
    public PlayerSafeZone playerSafeZone;
    [Header("Player Camera")]
    [SerializeField] public CinemachineOrbitalFollow OrbitalFollow;

    //SaveObjectPosition List
    public List<SaveObjectPosition> saveObjectPositions = new List<SaveObjectPosition>();

    //NPC Triggers
    public List<NPCTriggers> NPCTriggers = new List<NPCTriggers>();

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


        //Flags
        foreach(var flag in saveFileStruct.Flags)
        {
            if (flag.Value) FlagManager.Set_Flag(flag.Key);
        }

        //NPC Triggers
        foreach (var trigger in NPCTriggers)
        {
            if (saveFileStruct.activated_trigers.Contains(trigger.unique_id))
                trigger.Activated = true;

        }

        //Player Position
        playerSafeZone.transform.position = saveFileStruct.player_position.ToVector3();

        //Player Camera
        OrbitalFollow.HorizontalAxis.Value = saveFileStruct.camera_x;
        OrbitalFollow.VerticalAxis.Value = saveFileStruct.camera_y;



        //Save Object Positions and Rotations
        foreach (var saveObject in saveObjectPositions)
        {
            if (saveFileStruct.SaveObjectPositions.TryGetValue(saveObject.SaveObjectID, out var pos))
                saveObject.transform.position = pos.ToVector3();
            if (saveFileStruct.SaveObjectRotations.TryGetValue(saveObject.SaveObjectID, out var rotation))
                saveObject.transform.eulerAngles = rotation.ToVector3();
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
        foreach (var trigger in NPCTriggers)
        {
            if (trigger.Activated) saveFile.activated_trigers.Add(trigger.unique_id);
        }

        //Flags
        saveFile.Flags = FlagManager.Get_Flag_Dictionary();

        string json = JsonConvert.SerializeObject(saveFile, Formatting.Indented); // true = pretty print
        File.WriteAllText(SaveFile, json);
        Debug.Log("Saved settings to " + SaveFile);
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
        activated_trigers = new List<int>();
        Flags = new Dictionary<string, bool>();
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
    public List<int> activated_trigers;

    //Flags
    public Dictionary<string, bool> Flags;

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
