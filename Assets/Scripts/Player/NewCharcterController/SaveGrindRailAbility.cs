using Unity.VisualScripting;
using UnityEngine;

public class SaveGrindRailAbility : MonoBehaviour, ISaveable
{
    [SerializeField] private int unique_id;
    public int Get_Unique_ID { get => unique_id; set => unique_id = value; }

    public bool Get_Should_Save
    {
        get { return GetComponent<RailGrindAbility>(); }   
    }
    
    public void SaveLoaded(SaveFileStruct saveFileStruct)
    {
        AddGrindRailAbility(false);
    }

    public void AddGrindRailAbility(bool showPopup)
    {
        if (GetComponent<RailGrindAbility>() == null)
        {
            GetComponent<CharacterMovement>().AddAbility<RailGrindAbility>();
        }
        if(showPopup)
        {
            ScriptRefrenceSingleton.instance.gameplayUtils.ShowCustomNotifCenter("Grind Rails Unlocked",6);
        }
    }
}
