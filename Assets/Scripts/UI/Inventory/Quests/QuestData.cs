using UnityEngine;
using Sirenix.Serialization;
using Sirenix.OdinInspector;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Quests/QuestData")]
public class QuestData : SerializedScriptableObject
{

    public string ID;
    public string Name;
    public string Description;

    [SerializeReference]
    public List<QuestObjective> questObjectives = new List<QuestObjective>();

}
