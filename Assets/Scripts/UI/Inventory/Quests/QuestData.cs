using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Quests/QuestData")]
public class QuestData : SerializedScriptableObject
{

    public string ID;
    public string QuestName;
    public string Description;
    public bool AutoPinQuest;

    [SerializeReference]
    public List<QuestObjective> questObjectives = new List<QuestObjective>();

}
