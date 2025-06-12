using DS.ScriptableObjects;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using DS;
using DS.Data;
using System;


public class DialogRetriever : MonoBehaviour
{
    public static List<ScriptableObject> cached_data;

    void Awake()
    {
        LoadAllData();
    }

    void LoadAllData()
    {
        cached_data = Resources.LoadAll<ScriptableObject>("DialogueSystem/Dialogues").ToList();
    }


    public static ScriptableObject GetStarterNode(string graphName)
    {
        foreach (ScriptableObject scriptableObject in cached_data)
        {
            if (scriptableObject is DSDialogueContainerSO && scriptableObject.name.ToLower() == graphName.ToLower())
            {
                DSDialogueContainerSO container = (DSDialogueContainerSO)scriptableObject;
                foreach (ScriptableObject dialogObj in container.UngroupedDialogues)
                {
                    if (dialogObj is DSDialogueSO)
                    {
                        DSDialogueSO dialogSO = (DSDialogueSO)dialogObj;
                        if (dialogSO.DialogueType == DS.Enumerations.DSDialogueType.StartDialog)
                        {
                            return dialogObj;
                        }
                    }
                }
                foreach (List<DSDialogueSO> groupedLists in container.DialogueGroups.Values)
                {
                    foreach (DSDialogueSO dialogSO in groupedLists)
                    {
                        if (dialogSO.DialogueType == DS.Enumerations.DSDialogueType.StartDialog)
                        {
                            return dialogSO;
                        }
                    }
                }
            }
        }
        return null;
    }

    public static ScriptableObject GetDialogDataByName(string dialog_graph_name, string dialog_name)
    {
        foreach (ScriptableObject scriptableObject in cached_data)
        {
            if (scriptableObject is DSDialogueContainerSO)
            {
                if (scriptableObject.name != dialog_graph_name) continue;
                DSDialogueContainerSO container = (DSDialogueContainerSO)scriptableObject;
                foreach (ScriptableObject dialogObj in container.UngroupedDialogues)
                {
                    if (dialogObj.name == dialog_name)
                    {
                        return dialogObj;
                    }
                }
                foreach (List<DSDialogueSO> groupedLists in container.DialogueGroups.Values)
                {
                    foreach (DSDialogueSO dialogSO in groupedLists)
                    {
                        if (dialogSO.name == dialog_name)
                        {
                            return dialogSO;
                        }
                    }
                }
            }
        }

        return null;
    }

    public static ScriptableObject GetNextDialogSO(string graph_name, DSDialogueSO startNode)
    {
        ScriptableObject currentNode = startNode.Choices[0].NextDialogue;
        for (int i = 0; i < 1000; i++)
        {
            if (currentNode is DSDialogueSO)
            {
                DSDialogueSO dialogueSO = (DSDialogueSO)currentNode;
                switch (dialogueSO.DialogueType)
                {
                    case DS.Enumerations.DSDialogueType.ReturnToStart:
                        currentNode = GetStarterNode(graph_name);
                        continue;
                    case DS.Enumerations.DSDialogueType.Connector:
                    case DS.Enumerations.DSDialogueType.StartDialog:
                        currentNode = GetNextChoice(currentNode);
                        continue;
                    case DS.Enumerations.DSDialogueType.SingleChoice:
                    case DS.Enumerations.DSDialogueType.MultipleChoice:

                        return currentNode;
                }
            }
            currentNode = GetNextChoice(currentNode);
        }
        return null;
    }

    public static ScriptableObject GetNextDialogNode(string graph_name, DSDialogueSO startNode)
    {
        ScriptableObject currentNode = startNode.Choices[0].NextDialogue;
        for (int i = 0; i < 1000; i++)
        {
            if (currentNode is DSDialogueSO)
            {
                DSDialogueSO dialogueSO = (DSDialogueSO)currentNode;
                switch (dialogueSO.DialogueType)
                {
                    case DS.Enumerations.DSDialogueType.ReturnToStart:
                        currentNode = GetStarterNode(graph_name);
                        continue;
                    case DS.Enumerations.DSDialogueType.Connector:
                    case DS.Enumerations.DSDialogueType.StartDialog:
                        currentNode = GetNextChoice(currentNode);
                        continue;
                    case DS.Enumerations.DSDialogueType.SingleChoice:
                    case DS.Enumerations.DSDialogueType.MultipleChoice:

                        return currentNode;
                }
            }
            currentNode = GetNextChoice(currentNode);
        }
        return null;
    }

    public static ScriptableObject GetNextChoice(ScriptableObject currentChoice)
    {
        var Choices = currentChoice.GetType().GetProperty("Choices", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        if (Choices != null)
        {
            var value = Choices.GetValue(currentChoice);
            if (value is List<DSDialogueChoiceData> myList)
            {
                ScriptableObject nextCoice = myList[0].NextDialogue;
                return nextCoice;
            }
        }
        return null;
    }

    public static List<ScriptableObject> GetSettingsToNextDialog(ScriptableObject currentDialog)
    {
        List<ScriptableObject> settingsToNextDialog = new List<ScriptableObject>();

        // ScriptableObject nextSetting = GetNextChoice(currentDialog);

        // if (nextSetting is DSDialogueSO)
        // {
        //     DSDialogueSO dialogueSO = (DSDialogueSO)nextSetting;
        //     switch (dialogueSO.DialogueType)
        //     {
        //         case DS.Enumerations.DSDialogueType.SingleChoice:
        //         case DS.Enumerations.DSDialogueType.MultipleChoice:

        //             return settingsToNextDialog;
        //     }
        // }
        settingsToNextDialog.Add(currentDialog);
        for (int i = 0; i < 1000; i++)
        {
            if (i >= settingsToNextDialog.Count) return settingsToNextDialog;
            ScriptableObject currentSettings = settingsToNextDialog[i];
            if (currentSettings is DSDialogueSO)
            {
                DSDialogueSO dialogueSO = (DSDialogueSO)currentSettings;
                switch (dialogueSO.DialogueType)
                {
                    case DS.Enumerations.DSDialogueType.SingleChoice:
                    case DS.Enumerations.DSDialogueType.MultipleChoice:
                        continue;
                }
            }
            var Choices = currentSettings.GetType().GetProperty("Choices", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (Choices != null)
            {
                var value = Choices.GetValue(currentSettings);
                if (value is List<DSDialogueChoiceData> myList)
                {
                    foreach (DSDialogueChoiceData choice in myList)
                    {
                        if (choice.NextDialogue == null) continue;
                        if (choice.NextDialogue is DSDialogueSO)
                        {
                            DSDialogueSO dialogueSO = (DSDialogueSO)choice.NextDialogue;
                            switch (dialogueSO.DialogueType)
                            {
                                case DS.Enumerations.DSDialogueType.SingleChoice:
                                case DS.Enumerations.DSDialogueType.MultipleChoice:
                                    continue;
                            }
                        }
                        settingsToNextDialog.Add(choice.NextDialogue);
                    }
                    continue;
                }
            }

        }

        Debug.LogWarning("GetSettingsToNextDialog ran out of iterations. This has most likly caused an error");
        return settingsToNextDialog;
    }

    public static bool Choice_is_valid(ScriptableObject choice, DialogWorker dialogWorker)
    {
        if (choice == null) return false;
        ScriptableObject currentChoice = choice;
        for (int i = 0; i < 1000; i++)
        {

            switch (currentChoice)
            {
                case DSDialogueSO dialogueSO:
                    switch (dialogueSO.DialogueType)
                    {
                        case DS.Enumerations.DSDialogueType.SingleChoice:
                        case DS.Enumerations.DSDialogueType.MultipleChoice:
                            return true;
                        case DS.Enumerations.DSDialogueType.ReturnToStart:
                            currentChoice = DialogRetriever.GetStarterNode(dialogWorker.StartDialogGraphName);
                            break;

                    }

                    break;
                case DSItemRequirementSO itemRequirementSO:
                    string item_output_check = "IsFalse";
                    if (GameplayUtils.instance.get_item_holding_amount(itemRequirementSO.ItemID) >= int.Parse(itemRequirementSO.ItemAmount))
                    {
                        item_output_check = "IsTrue";
                    }
                    foreach (DSDialogueChoiceData item_choice in itemRequirementSO.Choices)
                    {
                        if (item_choice.OutputID == item_output_check)
                        {
                            if (item_choice.NextDialogue == null) return false;
                            currentChoice = item_choice.NextDialogue;
                            break;
                        }
                    }
                    break;
                case DSRequireFlagSO requireFlagSO:
                    string flag_id = requireFlagSO.FlagID;
                    string check = "IsFalse";
                    if (FlagManager.Get_Flag_Value(flag_id))
                    {
                        check = "IsTrue";
                    }
                    foreach (DSDialogueChoiceData flag_choice in requireFlagSO.Choices)
                    {
                        if (flag_choice.OutputID == check)
                        {
                            if (flag_choice.NextDialogue == null) return false;
                            currentChoice = flag_choice.NextDialogue;
                            break;
                        }
                    }
                    break;
            }
            var Choices = currentChoice.GetType().GetProperty("Choices", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (Choices != null)
            {
                var value = Choices.GetValue(currentChoice);
                if (value is List<DSDialogueChoiceData> myList)
                {
                    foreach (DSDialogueChoiceData listed_choice in myList)
                    {
                        if (listed_choice.NextDialogue == null) return false;
                        currentChoice = listed_choice.NextDialogue;
                        break;
                    }
                }
            }
        }

        return true;
    }

    public static string GetSettingNodeString(ScriptableObject settingNode)
    {
        switch (settingNode)
        {
            case DSDialogueSO dialogueSO:
                switch (dialogueSO.DialogueType)
                {
                    case DS.Enumerations.DSDialogueType.SingleChoice:
                    case DS.Enumerations.DSDialogueType.MultipleChoice:
                        return "was_dialog";
                    case DS.Enumerations.DSDialogueType.Connector:
                        return "clear";
                }
                break;

            case DSCloseDialogSO:
                return "close_dialog";

            case DSItemRequirementSO:
                // CHECK FOR ITEMS HERE
                if (true)
                {
                    return "cleared_true";
                }
                else
                {
                    return "cleared_false";
                }

            case DSRequireFlagSO:
                // CHECK FOR FLAG HERE
                if (true)
                {
                    return "cleared_true";
                }
                else
                {
                    return "cleared_false";
                }

            case DSRunEventSO:
                // SEND FLAG ID BACK
                return $"flag_id[FLAGID]";

            case DSSetFlagSO:
                return "clear";
        }
        Debug.LogError("Couldn't get node type");
        return "";
    }

    

}
