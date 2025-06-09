using System;
using DS.Data.Save;
using UnityEngine;

[Serializable]
public class DSItemRequirementSaveData : DSNodeSaveData
{
    [SerializeField] public string item_id;
    [SerializeField] public string item_amount;
    [SerializeField] public bool remove_items;
}
