using UnityEngine;

public interface ISaveable
{
    public int Get_Unique_ID { get; set; }
    public bool Get_Should_Save { get; }
    public void SaveLoaded(SaveFileStruct saveFileStruct);

    public void AddToManager()
    {
        SaveLoadManager.instance.saveables.Add(this);
    }
}
