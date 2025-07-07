using System;
using DS.Data.Save;
using UnityEngine;

[Serializable]
public class DSAssignQuestSaveData : DSNodeSaveData
{
    [SerializeField] public string quest_id;
}
