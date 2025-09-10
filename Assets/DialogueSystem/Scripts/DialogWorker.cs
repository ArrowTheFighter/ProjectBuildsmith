using DS.ScriptableObjects;
using TMPro;
using UnityEngine;
using Sirenix.OdinInspector;
using DS.Data;
using EasyTextEffects;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

public class DialogWorker : MonoBehaviour, IInteractable
{
    [SerializeField] public string StartDialogGraphName;
    TextEffect textEffect;

    [Header("NPC")]
    [SerializeField] string NPC_Name;
    [SerializeField] string Localized_Table = "NPC";

    [Header("Events")]
    [SerializeField] public NPC_Event[] nPC_Events;

    [Header("Localization")]
    [SerializeField] bool UseLocalization;

    ScriptableObject currentDialogSO;
    DSDialogueSO StarterNode;

    public string PROMPT;
    public string INTERACTION_PROMPT => PROMPT;

    public item_requirement[] item_Requirements;
    public item_requirement[] required_items => item_Requirements;

    public bool NPCCanInteract = true;
    public bool CanInteract { get => NPCCanInteract; set { NPCCanInteract = value; } }

    bool isActive;

    float interactCooldown;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StarterNode = (DSDialogueSO)DialogRetriever.GetStarterNode(StartDialogGraphName);
        //currentDialogSO = DialogRetriever.GetDialogDataByName(StartDialogGraphName, startDialogName);
        //currentDialogSO = DialogRetriever.GetNextDialogSO(StartDialogGraphName,StarterNode);
        //ShowDialog();
        GameplayInput.instance.playerInput.actions["Submit"].performed += context => { ActiveAndInteract(); };
        textEffect = DialogManager.instance.text_box.GetComponent<TextEffect>();
    }

    public bool Interact(Interactor interactor)
    {
        if (!NPCCanInteract) return false;
        if (!isActive) isActive = true;
        ActiveAndInteract();
        return true;
    }

    public void ActiveAndInteract()
    {
        if (!isActive) return;
        GetAndShowNextDialog();
    }

    public void ForceDialog()
    {
        if (!isActive) isActive = true;
        GetAndShowNextDialog();    
    }

    public void ForceDialog(ScriptableObject providedDialog)
    {
        if (!isActive) isActive = true;
        GetAndShowNextDialog(providedDialog);
    }


    public void GetAndShowNextDialog(ScriptableObject providedDialog = null)
    {
        if (interactCooldown > Time.time) return;
        //List<TextEffectStatus> textEffectStatus = textEffect.QueryEffectStatusesByTag(TextEffectType.Global, TextEffectEntry.TriggerWhen.Manual, "scale_text_in");
        if (DialogManager.instance.TextIsAnimating)
        {
            print("text is animating");
            textEffect.StopManualEffects();
            DialogManager.instance.SetTextIsAnimating(false);
            return;
        }

        if (!GameplayUtils.instance.OpenDialogMenu()) return;
        bool nextDialogResult = GetNextDialog(providedDialog);
        if (nextDialogResult)
        {
            ShowDialog();
            ResetTMPSubMeshes(DialogManager.instance.text_box);
            DialogManager.instance.text_box.ForceMeshUpdate();
            //textEffect.StopAllEffects();
            textEffect.Refresh();
            print("Starting manual effects");
            textEffect.StartManualEffects();
            DialogManager.instance.SetTextIsAnimating(true);
        }
        interactCooldown = Time.time + 0.05f;

    }

    void ResetTMPSubMeshes(TextMeshProUGUI tmp)
    {
        if (tmp == null) return;
        if (!tmp.text.Contains("<sprite"))
        {
            tmp.ForceMeshUpdate(true, true);
            var subMeshes = tmp.GetComponentsInChildren<TMP_SubMeshUI>();
            foreach (var subMesh in subMeshes)
            {
                if (subMesh != null)
                {
#if UNITY_EDITOR
                    if (!Application.isPlaying)
                        DestroyImmediate(subMesh.gameObject);
                    else Destroy(subMesh.gameObject);
#else
                    Destroy(subMesh.gameObject);
#endif
                }
            }
            tmp.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
            LayoutRebuilder.ForceRebuildLayoutImmediate(tmp.rectTransform);
         }
     }



    bool GetNextDialog(ScriptableObject providedDialog = null)
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
            else if (dialogueSO.DialogueType == DS.Enumerations.DSDialogueType.StartDialog)
            {

            }
            else
            {
                if (providedDialog == null)
                {
                    return false;
                }
            }
        }
        if (currentDialogSO is DSCloseDialogSO)
        {
            DSCloseDialogSO dialogueSO = (DSCloseDialogSO)currentDialogSO;
            if (dialogueSO.Choices.Count <= 1)
            {
                tempDialogSO = dialogueSO.Choices[0].NextDialogue;
            }
        }
        if (providedDialog != null)
        {
            tempDialogSO = providedDialog;
        }
        bool breakLoop = false;
        for (int i = 0; i < 1000; i++)
        {
            if (tempDialogSO is DSDialogueSO)
            {
                bool continueLoop = false;
                DSDialogueSO dialogueSO = (DSDialogueSO)tempDialogSO;
                switch (dialogueSO.DialogueType)
                {
                    case DS.Enumerations.DSDialogueType.MultipleChoice:
                        if (currentDialogSO == dialogueSO)
                        {
                            print("on same multi");
                            return false;
                         }
                        if (dialogueSO.Choices.Count > 0)
                        {
                            DialogManager.instance.Setup_Choices(dialogueSO.Choices, this,UseLocalization);
                        }
                        currentDialogSO = dialogueSO;
                        breakLoop = true;
                        break;
                    case DS.Enumerations.DSDialogueType.SingleChoice:
                        if (DialogManager.instance.get_active_choices() > 0)
                        {
                            DialogManager.instance.Clear_choices();
                        }
                        currentDialogSO = dialogueSO;
                        breakLoop = true;
                        break;
                    case DS.Enumerations.DSDialogueType.ReturnToStart:
                        DSDialogueSO startNode = (DSDialogueSO)DialogRetriever.GetStarterNode(StartDialogGraphName);
                        // TODO - Change this so it properly loops through all options if it needs to
                        tempDialogSO = startNode;
                        continue;
                    case DS.Enumerations.DSDialogueType.StartDialog:
                        for (int o = 0; o < dialogueSO.Choices.Count; o++)
                        {
                            if (DialogRetriever.Choice_is_valid(dialogueSO.Choices[o].NextDialogue, this))
                            {
                                tempDialogSO = dialogueSO.Choices[o].NextDialogue;
                                continueLoop = true;
                                break;
                            }
                            else
                            {
                                continue;
                            }
                        }
                        break;
                }
                if (continueLoop) continue;
                if (breakLoop)
                {
                    break;
                }
                if (dialogueSO.Choices.Count <= 1)
                {
                    tempDialogSO = dialogueSO.Choices[0].NextDialogue;
                    continue;
                }
            }
            // -- Indavidual node functionality goes here --
            switch (tempDialogSO)
            {
                case DSRequireFlagSO requireFlagSO:
                    string flag_id = requireFlagSO.FlagID;
                    string check = "IsFalse";
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
                case DSGiveItemSO giveItemSO:
                    ItemData item = GameplayUtils.instance.GetItemDataByID(giveItemSO.ItemID);
                    GameplayUtils.instance.inventoryManager.AddItemToInventory(item, int.Parse(giveItemSO.ItemAmount));
                    foreach (DSDialogueChoiceData choice in giveItemSO.Choices)
                    {
                        tempDialogSO = choice.NextDialogue;
                        break;
                    }
                    break;
                case DSCloseDialogSO closeDialogSO:
                    currentDialogSO = closeDialogSO;
                    CloseDialog();
                    return false;
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
                case DSAssignQuestSO assignQuestSO:
                    GameplayUtils.instance.inventoryManager.AssignQuest(assignQuestSO.QuestID);

                    if (assignQuestSO.Choices.Count > 0)
                    {
                        tempDialogSO = assignQuestSO.Choices[0].NextDialogue;
                    }
                    break;
            }
        }
        return true;
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
        isActive = false;
        DialogManager.instance.DialogUI.SetActive(false);
        GameplayUtils.instance.CloseDialogMenu();
    }

    void ShowDialog()
    {
        if (currentDialogSO is DSDialogueSO)
        {
            string fontWeight = "<font-weight=500>";
            DialogManager.instance.name_text.text = fontWeight + NPC_Name;
            DialogManager.instance.DialogUI.SetActive(true);
            DSDialogueSO dialogSO = (DSDialogueSO)currentDialogSO;
            if (UseLocalization)
            {
                string localizedText = LocalizationManager.GetLocalizedString(Localized_Table, dialogSO.LocalizedKey);
                DialogManager.instance.text_box.text = localizedText;
            }
            else
            {
                DialogManager.instance.text_box.text = dialogSO.Text;
            }
        }
    }



}

