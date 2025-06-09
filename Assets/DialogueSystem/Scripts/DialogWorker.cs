using DS.ScriptableObjects;
using TMPro;
using UnityEngine;
using Sirenix.OdinInspector;
using DS.Data;
using System.Collections;
using EasyTextEffects;
using System.Collections.Generic;
using Unity.VisualScripting;

public class DialogWorker : MonoBehaviour, IInteractable
{
    [SerializeField] string StartDialogGraphName;
    [SerializeField] TextMeshProUGUI textBox;
    [SerializeField] TextEffect textEffect;
    ScriptableObject currentDialogSO;
    DSDialogueSO StarterNode;

    public string PROMPT;
    public string INTERACTION_PROMPT => PROMPT;

    public item_requirement[] item_Requirements;
    public item_requirement[] required_items => item_Requirements;

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
                    tempDialogSO = dialogueSO.Choices[0].NextDialogue;
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
                    // TODO CLOSE DIALOG STUFF HERE

                    if (closeDialogSO.Choices.Count > 0)
                    {
                        tempDialogSO = closeDialogSO.Choices[0].NextDialogue;
                    }
                    break;
                case DSRunEventSO runEventSO:
                    // TODO SETUP RUN EVENT STUFF HERE

                    if (runEventSO.Choices.Count > 0)
                    {
                        tempDialogSO = runEventSO.Choices[0].NextDialogue;
                    }
                    break;

                case DSSetFlagSO setFlagSO:
                    //TODO ADD SET FLAG STUFF HERE

                    if (setFlagSO.Choices.Count > 0)
                    {
                        tempDialogSO = setFlagSO.Choices[0].NextDialogue;
                    }
                    break;

            }
        }
    }

    void ShowDialog()
    {
        if (currentDialogSO is DSDialogueSO)
        {
            DSDialogueSO dialogSO = (DSDialogueSO)currentDialogSO;
            textBox.text = dialogSO.Text;
        }
    }


   
}
