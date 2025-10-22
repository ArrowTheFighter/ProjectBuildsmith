using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using System.Linq;
using System.Collections;
using Newtonsoft.Json;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager instance;
    private static string FilePath => Path.Combine(Application.persistentDataPath + "/saves/");

    //Info to save
    public List<int> special_items_collected;


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

    void Start()
    {
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
        foreach(InventorySlot slot in GameplayUtils.instance.inventoryManager.inventorySlots)
        {
            saveFile.inventory_slots.Add(new SaveableInventroySlot(slot.isEmpty, slot.slot_id, slot.inventoryItemStack));
        }

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
    public string file_name;
    public int[] special_items_collected;

    //Inventory
    public Dictionary<string, int> special_items;
    public List<SaveableInventroySlot> inventory_slots;

}
