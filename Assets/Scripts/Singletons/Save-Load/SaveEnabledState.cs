using UnityEngine;

public class SaveEnabledState : MonoBehaviour, ISaveable
{

    public bool SaveWhenActive;

    public int unique_id;

    public int Get_Unique_ID { get => unique_id; set{ unique_id = value; } }

    public bool Get_Should_Save {get { return SaveWhenActive == gameObject.activeInHierarchy; }}

    public void SaveLoaded(SaveFileStruct saveFileStruct)
    {
       
        gameObject.SetActive(SaveWhenActive);
    }

   

    
}
