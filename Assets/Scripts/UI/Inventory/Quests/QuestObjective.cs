using System;
using UnityEngine;

[Serializable]
public abstract class QuestObjective
{
    public string Description;
    public bool StopAtThisObjective;
    public bool StayCompleted;
    [NonSerialized] public bool isComplete;
    public int ObjectiveIDCollection;
    
    public bool ObjectiveComplete()
    {
        if (isComplete)
            return true;

        bool result = CheckObjectiveComplete();
        if (result && StayCompleted)
        {
            isComplete = true;
        }
        return result;
    }

    protected abstract bool CheckObjectiveComplete();
}
