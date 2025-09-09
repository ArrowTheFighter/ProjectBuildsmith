using System;
using UnityEngine;

[Serializable]
public abstract class QuestObjective
{
    public string Description;
    public bool StopAtThisObjective;
    public int ObjectiveIDCollection;
    public abstract bool ObjectiveComplete();
}
