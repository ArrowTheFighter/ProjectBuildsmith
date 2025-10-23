using UnityEngine;

public class NPCTriggers : MonoBehaviour, ISaveable
{
    public enum NPCTriggerTypes { Jump, Dive, Stop, DontJump, ForceSlide, EnableTriggers }
    public NPCTriggerTypes TriggerType;
    public PlayerSetNPCMovingTrigger activateTrigger;
    public bool OnlyOnce;
    public bool Activated;
    public bool TurnAroundOnNPC;
    public bool EnableQuestMarker;
    public int unique_id;

    public int Get_Unique_ID { get => unique_id; set { unique_id = value; } }

    public bool Get_Should_Save => Activated;

   

    public void SaveLoaded(SaveFileStruct saveFileStruct)
    {
        Activated = true;
    }
}
