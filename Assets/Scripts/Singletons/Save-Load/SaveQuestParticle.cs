using UnityEngine;

public class SaveQuestParticle : MonoBehaviour, ISaveable
{
    [SerializeField, TextArea]
    string INSTRUCTION = "Any quest marker needs to have this script on it's parent to properly save. Don't add any other childred to this gameobject";

    public int unique_id;
    public int Get_Unique_ID { get => unique_id; set { unique_id = value; } }

    public bool Get_Should_Save {get { return transform.childCount > 0; }}

    public void SaveLoaded(SaveFileStruct saveFileStruct)
    {
        ScriptRefrenceSingleton.instance.compassScript.AddNewQuestMarkerWithWorldMarker(transform);
    }

}
