using UnityEngine;

public class SaveEnabledState : MonoBehaviour, ISaveable
{

    public bool SaveWhenActive;
    public bool OnlyCheckSelfActive;

    public int unique_id;

    public int Get_Unique_ID { get => unique_id; set{ unique_id = value; } }

    public bool Get_Should_Save {get 
        {
        if(OnlyCheckSelfActive)
        {
                return SaveWhenActive == gameObject.activeSelf;
            }
         return SaveWhenActive == gameObject.activeInHierarchy; 
         }
    }

    public void SaveLoaded(SaveFileStruct saveFileStruct)
    {
       
        gameObject.SetActive(SaveWhenActive);
    }

   

    
}
