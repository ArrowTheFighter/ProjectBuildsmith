using UnityEngine;

public class ObjectiveTalkToNPCFlag : QuestObjective
{
    public string flag_id;

    protected override bool CheckObjectiveComplete()
    {
        return FlagManager.Get_Flag_Value(flag_id);
    }
   
}
