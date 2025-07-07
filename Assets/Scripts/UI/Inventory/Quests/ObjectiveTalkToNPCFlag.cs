using UnityEngine;

public class ObjectiveTalkToNPCFlag : QuestObjective
{
    public string flag_id;

    public override bool ObjectiveComplete()
    {
        return FlagManager.Get_Flag_Value(flag_id);
    }
   
}
