using System;
using DS.Data.Save;
using UnityEngine;

[Serializable]
public class DSUnlockRecipeSaveData : DSNodeSaveData
{
    [SerializeField] public string recipe_id;
}
