using UnityEngine;

public class NewCutsceneBuilder : MonoBehaviour, ISaveable
{
    public CutsceneData cutsceneData;
    public bool has_played;
    public bool save_played;
    public int unique_id;

    public int Get_Unique_ID { get => unique_id; set { unique_id = value; } }

    public bool Get_Should_Save => has_played;

    void Start()
    {
        if(save_played)
            (this as ISaveable).AddToManager();
        //if(save_played) SaveLoadManager.instance.savedCustsceneBuilders.Add(this);
    }

    public void PlayCutscene()
    {
        if (save_played) has_played = true;
        CutsceneManager.instance.StartCutscene(cutsceneData);
    }

    public void SaveLoaded(SaveFileStruct saveFileStruct)
    {
        PlayCutscene();
        CutsceneManager.instance.SkipCutscene();
    }
}
