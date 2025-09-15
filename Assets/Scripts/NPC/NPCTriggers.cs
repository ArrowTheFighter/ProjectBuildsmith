using UnityEngine;

public class NPCTriggers : MonoBehaviour
{
    public enum NPCTriggerTypes { Jump, Dive, Stop, DontJump, ForceSlide, EnableTriggers }
    public NPCTriggerTypes TriggerType;
    public PlayerSetNPCMovingTrigger activateTrigger;
    public bool OnlyOnce;
    public bool Activated;
    public bool TurnAroundOnNPC;
    public bool EnableQuestMarker;
    
}
