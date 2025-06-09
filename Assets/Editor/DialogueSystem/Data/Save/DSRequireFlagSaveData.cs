using System;
using DS.Data.Save;
using UnityEngine;

[Serializable]
public class DSRequireFlagSaveData : DSNodeSaveData
{
    [SerializeField] public string flag_id;
    [SerializeField] public bool is_true;
}
