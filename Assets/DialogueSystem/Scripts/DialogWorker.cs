using DS.ScriptableObjects;
using TMPro;
using UnityEngine;
using Sirenix.OdinInspector;
using DS.Data;
using EasyTextEffects;
using System;
using UnityEngine.Events;

public class DialogWorker : MonoBehaviour, IInteractable
{
    [SerializeField] string StartDialogGraphName;
    [SerializeField] TextMeshProUGUI textBox;
    [SerializeField] TextMeshProUGUI dialogNameTextBox;
    [SerializeField] GameObject DialogMenu;
    [SerializeField] TextEffect textEffect;

    [Header("NPC")]
    [SerializeField] string NPC_Name;

    [Header("Events")]
    [SerializeField] public NPC_Event[] nPC_Events;

    ScriptableObject currentDialogSO;
    DSDialogueSO StarterNode;

    public string PROMPT;
    public string INTERACTION_PROMPT => PROMPT;

    public item_requirement[] item_Requirements;
    public item_requirement[] required_items => item_Requirements;

    bool should_close;
    bool dialog_is_open;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StarterNode = (DSDialogueSO)DialogRetriever.GetStarterNode(StartDialogGraphName);
        //currentDialogSO = DialogRetriever.GetDialogDataByName(StartDialogGraphName, startDialogName);
        //currentDialogSO = DialogRetriever.GetNextDialogSO(StartDialogGraphName,StarterNode);
        //ShowDialog();
    }

    public bool Interact(Interactor interactor)
    {
        GetAndShowNextDialog();
        return true;
    }



    [Button("Show Next Dialog")]
    void GetAndShowNextDialog()
    {
        GameplayUtils.instance.OpenMenu();
        Debug.Log("Getting next dialog");
        GetNextDialog();
        ShowDialog();
        textEffect.Refresh();
        textEffect.StartManualEffects();

    }


    void GetNextDialog()
    {
        if (currentDialogSO == null) currentDialogSO = StarterNode;
        ScriptableObject tempDialogSO = currentDialogSO;
        if (currentDialogSO is DSDialogueSO)
        {
            DSDialogueSO dialogueSO = (DSDialogueSO)currentDialogSO;
            if (dialogueSO.Choices.Count <= 1)
            {
                tempDialogSO = dialogueSO.Choices[0].NextDialogue;
            }
        }
        if (currentDialogSO is DSCloseDialogSO)
        {
            Debug.Log("Dialog was closeDialog Node");
            DSCloseDialogSO dialogueSO = (DSCloseDialogSO)currentDialogSO;
            if (dialogueSO.Choices.Count <= 1)
            {
                tempDialogSO = dialogueSO.Choices[0].NextDialogue;
            }
        }
        bool breakLoop = false;
        for (int i = 0; i < 1000; i++)
        {
            if (tempDialogSO is DSDialogueSO)
            {
                DSDialogueSO dialogueSO = (DSDialogueSO)tempDialogSO;
                switch (dialogueSO.DialogueType)
                {
                    case DS.Enumerations.DSDialogueType.SingleChoice:
                    case DS.Enumerations.DSDialogueType.MultipleChoice:
                        currentDialogSO = dialogueSO;
                        breakLoop = true;
                        break;
                    case DS.Enumerations.DSDialogueType.ReturnToStart:
                        DSDialogueSO startNode = (DSDialogueSO)DialogRetriever.GetStarterNode(StartDialogGraphName);
                        // TODO - Change this so it properly loops through all options if it needs to
                        tempDialogSO = startNode.Choices[0].NextDialogue;
                        continue;
                }
                if (breakLoop) break;
                if (dialogueSO.Choices.Count <= 1)
                {
                    Debug.Log("Dialog was a connector node (probably): " + dialogueSO.DialogueType);
                    tempDialogSO = dialogueSO.Choices[0].NextDialogue;
                    continue;
                }
            }
            switch (tempDialogSO)
            {
                case DSRequireFlagSO requireFlagSO:
                    Debug.Log("Found flag check node");
                    string flag_id = requireFlagSO.FlagID;
                    string check = "IsFalse";
                    FlagManager.Get_Flag_Value(flag_id);
                    if (FlagManager.Get_Flag_Value(flag_id))
                    {
                        check = "IsTrue";
                    }
                    foreach (DSDialogueChoiceData choice in requireFlagSO.Choices)
                    {
                        if (choice.OutputID == check)
                        {
                            tempDialogSO = choice.NextDialogue;
                            break;
                        }
                    }
                    break;
                case DSItemRequirementSO itemRequirementSO:
                    string item_output_check = "IsFalse";
                    if (GameplayUtils.instance.get_item_holding_amount(itemRequirementSO.ItemID) >= int.Parse(itemRequirementSO.ItemAmount))
                    {
                        item_output_check = "IsTrue";
                        if (itemRequirementSO.RemoveItems)
                        {
                            GameplayUtils.instance.remove_items_from_inventory(itemRequirementSO.ItemID, int.Parse(itemRequirementSO.ItemAmount));
                        }
                    }
                    foreach (DSDialogueChoiceData choice in itemRequirementSO.Choices)
                    {
                        if (choice.OutputID == item_output_check)
                        {
                            tempDialogSO = choice.NextDialogue;
                            break;
                        }
                    }
                    break;
                case DSCloseDialogSO closeDialogSO:
                    should_close = true;
                    currentDialogSO = closeDialogSO;
                    CloseDialog();
                    return;
                case DSRunEventSO runEventSO:
                    runEvent(runEventSO.EventID);

                    if (runEventSO.Choices.Count > 0)
                    {
                        tempDialogSO = runEventSO.Choices[0].NextDialogue;
                    }
                    break;

                case DSSetFlagSO setFlagSO:
                    FlagManager.Set_Flag(setFlagSO.FlagID, setFlagSO.IsTrue);

                    if (setFlagSO.Choices.Count > 0)
                    {
                        tempDialogSO = setFlagSO.Choices[0].NextDialogue;
                    }
                    break;
            }
        }
    }

    void runEvent(string event_id)
    {
        for (int i = 0; i < nPC_Events.Length; i++)
        {
            if (nPC_Events[i].id == event_id)
            {
                nPC_Events[i].run_event?.Invoke();
            }
        }
     }

    void CloseDialog()
    {
        Debug.Log("Closing dialog");
        DialogMenu.SetActive(false);
        GameplayUtils.instance.CloseMenu();
    }

    void ShowDialog()
    {
        if (currentDialogSO is DSDialogueSO)
        {
            print("Showing dialog");
            dialogNameTextBox.text = NPC_Name;
            DialogMenu.SetActive(true);
            dialog_is_open = true;
            DSDialogueSO dialogSO = (DSDialogueSO)currentDialogSO;
            textBox.text = dialogSO.Text;
        }
    }



}

