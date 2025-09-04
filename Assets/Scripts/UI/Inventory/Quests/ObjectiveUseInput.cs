using System;
using UnityEngine;

[Serializable]
public class ObjectiveUseInput : QuestObjective
{
    public string mapName;
    public string InputName;

    public override bool ObjectiveComplete()
    {
        return GameplayInput.instance.HasUsedInput(mapName, InputName);
    }


}
