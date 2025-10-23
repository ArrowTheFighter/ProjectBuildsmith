using UnityEngine;

public class SaveEnabledState : MonoBehaviour, ISaveable
{
    public int unique_id;

    public int Get_Unique_ID { get => unique_id; set{ unique_id = value; } }

    public bool Get_Should_Save {get { return !gameObject.activeInHierarchy; }}

    public void SaveLoaded(SaveFileStruct saveFileStruct)
    {
        gameObject.SetActive(false);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        (this as ISaveable).AddToManager();
        //SaveLoadManager.instance.saveEnabledStates.Add(this);
    }

    
}
