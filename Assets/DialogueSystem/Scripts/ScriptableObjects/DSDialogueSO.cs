using System.Collections.Generic;
using UnityEngine;

namespace DS.ScriptableObjects
{
    using Data;
    using Enumerations;

    public class DSDialogueSO : ScriptableObject
    {
        [field: SerializeField] public string DialogueName { get; set; }
        [field: SerializeField] [field: TextArea()] public string Text { get; set; }
        [field: SerializeField] public string LocalizedKey { get; set; }
        [field: SerializeField] public List<DSDialogueChoiceData> Choices { get; set; }
        [field: SerializeField] public DSDialogueType DialogueType { get; set; }
        [field: SerializeField] public bool IsStartingDialogue { get; set; }
        [field: SerializeField] public bool CloseDialog;
        [field: SerializeField] public int unique_id { get; set; }

        public void Initialize(string dialogueName, string text,string locaizedKey, List<DSDialogueChoiceData> choices, DSDialogueType dialogueType, bool isStartingDialogue, bool closeDialog)
        {
            DialogueName = dialogueName;
            Text = text;
            LocalizedKey = locaizedKey;
            Choices = choices;
            DialogueType = dialogueType;
            IsStartingDialogue = isStartingDialogue;
            CloseDialog = closeDialog;
        }
    }
}