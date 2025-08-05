using System;
using DS.Data.Save;
using UnityEngine;

[Serializable]
public class DSGiveItemSaveData : DSNodeSaveData
{
    [SerializeField] public string item_id;
    [SerializeField] public string item_amount;
}
