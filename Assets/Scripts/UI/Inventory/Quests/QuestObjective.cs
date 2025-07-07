using System;
using UnityEngine;

[Serializable]
public abstract class QuestObjective
{
    public string Description;
    public abstract bool ObjectiveComplete();


}
