using System;
using UnityEngine;

namespace DS.Data
{
    using ScriptableObjects;

    [Serializable]
    public class DSDialogueChoiceData
    {
        [field: SerializeField] public string Text { get; set; }
        [SerializeField] public string LocalizeKey;
        [SerializeField] public string OutputID;
        [field: SerializeField] public ScriptableObject NextDialogue { get; set; }
    }
}