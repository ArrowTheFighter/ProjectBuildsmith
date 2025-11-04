using DS.ScriptableObjects;
using TMPro;
using UnityEngine;
using DS.Data;
using EasyTextEffects;
using UnityEngine.UI;
using Unity.Cinemachine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

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

    public ScriptableObject currentDialogSO;
    DSDialogueSO StarterNode;

    public string InactivePrompt = "Talk";
    public string ActivePrompt = "Continue";
    public string PROMPT;
    public string INTERACTION_PROMPT => PROMPT;

    public item_requirement[] item_Requirements;
    public item_requirement[] required_items => item_Requirements;

    public bool NPCCanInteract = true;
    public bool CanInteract { get => NPCCanInteract; set { NPCCanInteract = value; } }

    bool isActive;

    float interactCooldown;

    public bool hasMarker;
    [SerializeField] ParticleSystem MarkerParticle;

    [Header("Cameras")]
    public float blendTime = 4;
    public GameObject NPCCamera;
    GameObject frozenCam;
    public bool TurnTowardsPlayer;
    public bool TurnBackToOrigin;

    public int unique_id;

    void Awake()
    {
        ScriptRefrenceSingleton.OnScriptLoaded += BindInputs;
        PROMPT = InactivePrompt;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ScriptRefrenceSingleton.instance.saveLoadManager.DialogWorkers.Add(this);
        StarterNode = (DSDialogueSO)DialogRetriever.GetStarterNode(StartDialogGraphName);
        //currentDialogSO = DialogRetriever.GetDialogDataByName(StartDialogGraphName, startDialogName);
        //currentDialogSO = DialogRetriever.GetNextDialogSO(StartDialogGraphName,StarterNode);
        //ShowDialog();
        //ScriptRefrenceSingleton.instance.gameplayInput.playerInput.actions["Submit"].performed += context => { ActiveAndInteract(); };
        textEffect = ScriptRefrenceSingleton.instance.dialogManager.text_box.GetComponent<TextEffect>();
    }

    void BindInputs()
    {
        ScriptRefrenceSingleton.instance.gameplayInput.playerInput.actions["Submit"].performed += ActiveAndInteractContext;
        ScriptRefrenceSingleton.instance.gameplayInput.playerInput.actions["Click"].performed += ActiveAndClickInteract;

        ScriptRefrenceSingleton.OnScriptLoaded -= BindInputs;
        ScriptRefrenceSingleton.instance.gameplayUtils.OnStartMoveToMainMenu += UnbindInputs;
    }

    void UnbindInputs()
    {
        ScriptRefrenceSingleton.instance.gameplayInput.playerInput.actions["Submit"].performed -= ActiveAndInteractContext;
        ScriptRefrenceSingleton.instance.gameplayInput.playerInput.actions["Click"].performed -= ActiveAndClickInteract;
        ScriptRefrenceSingleton.instance.gameplayUtils.OnStartMoveToMainMenu -= UnbindInputs;

    }


    public void SetCurrentDialogByID(int id)
    {
        ScriptableObject dialog = DialogRetriever.GetDialogByID(id);
        if(dialog != null)
        {
            currentDialogSO = dialog;
        }
    }

    public bool Interact(Interactor interactor)
    {
        if (!NPCCanInteract) return false;
        if (!isActive) SetIsActive(true);
        ActiveAndInteract();
        return true;
    }

    public void SetCanInteract(bool value)
    {
        NPCCanInteract = value;
    }

    void SetIsActive(bool active)
    {
        if (!isActive && active)
        {
            if (NPCCamera != null)
            {
                if (ScriptRefrenceSingleton.instance.gameplayUtils.cinemachineBrain.TryGetComponent(out CinemachineBrainEvents cinemachineBrainEvents))
                {
                    cinemachineBrainEvents.BlendCreatedEvent.AddListener(setCameraBlend);
                }

                NPCCamera.SetActive(true);
            }
            if (TurnTowardsPlayer)
            {
                if (TryGetComponent(out CharacterMovement characterMovement))
                {
                    Vector3 playerPos = ScriptRefrenceSingleton.instance.gameplayUtils.PlayerTransform.position;
                    playerPos.y = transform.position.y;
                    Vector3 dirToPlayer = playerPos - transform.position;
                    characterMovement.ManualTurn(dirToPlayer.normalized, 0.25f);
                }
            }
        }
        else if (isActive && !active)
        {
            if (NPCCamera != null)
            {
                if (frozenCam != null) Destroy(frozenCam);
                frozenCam = Instantiate(NPCCamera);
                //frozenCam.AddComponent<CinemachineCamera>();
                frozenCam.transform.position = NPCCamera.transform.position;
                frozenCam.transform.rotation = NPCCamera.transform.rotation;
                NPCCamera.SetActive(false);

                if (ScriptRefrenceSingleton.instance.gameplayUtils.cinemachineBrain.TryGetComponent(out CinemachineBrainEvents cinemachineBrainEvents))
                {
                    cinemachineBrainEvents.BlendCreatedEvent.AddListener(setCameraBlendInstant);
                }
            }


            if (TurnTowardsPlayer && TurnBackToOrigin)
            {
                if (TryGetComponent(out CharacterMovement characterMovement))
                {
                    if (TryGetComponent(out NPCFollowTargetInput nPCFollowTargetInput))
                    {
                        characterMovement.ManualTurn(nPCFollowTargetInput.forwardDir, 0.25f);
                    }
                }
            }
        }
        PROMPT = active ? ActivePrompt : InactivePrompt;
        if (TryGetComponent(out CharacterMovement component))
        {
            component.playerAnimationController.animator.SetBool("Talking", active);
        }
        isActive = active;
    }


    public void setCameraBlend(CinemachineCore.BlendEventParams value)
    {
        value.Blend.Duration = blendTime;
        if (ScriptRefrenceSingleton.instance.gameplayUtils.cinemachineBrain.TryGetComponent(out CinemachineBrainEvents cinemachineBrainEvents))
        {
            cinemachineBrainEvents.BlendCreatedEvent.RemoveListener(setCameraBlend);
        }
    }

    public void setCameraBlendInstant(CinemachineCore.BlendEventParams value)
    {
        value.Blend.Duration = 0;
        if (ScriptRefrenceSingleton.instance.gameplayUtils.cinemachineBrain.TryGetComponent(out CinemachineBrainEvents cinemachineBrainEvents))
        {
            cinemachineBrainEvents.BlendCreatedEvent.RemoveListener(setCameraBlend);
            cinemachineBrainEvents.BlendCreatedEvent.AddListener(setCameraBlend);
            frozenCam.SetActive(false);

        }
    }

    void ActiveAndClickInteract(InputAction.CallbackContext callback)
    {
        if (ScriptRefrenceSingleton.instance.dialogManager.ActiveChoices.Count > 0) return;
        ActiveAndInteract();
    }
    
    void ActiveAndInteractContext(InputAction.CallbackContext callback)
    {
        ActiveAndInteract();
    }

    public void ActiveAndInteract()
    {
        if (!isActive) return;
        GetAndShowNextDialog();
    }

    public void ForceDialog()
    {
        if (!isActive) SetIsActive(true);
        GetAndShowNextDialog();
    }

    public void ForceDialog(ScriptableObject providedDialog)
    {
        if (!isActive) SetIsActive(true);
        GetAndShowNextDialog(providedDialog);
    }


    public void GetAndShowNextDialog(ScriptableObject providedDialog = null)
    {
        if (StarterNode == null) return;
        if (interactCooldown > Time.time) return;
        if (hasMarker) EnableMarker(false);
        //List<TextEffectStatus> textEffectStatus = textEffect.QueryEffectStatusesByTag(TextEffectType.Global, TextEffectEntry.TriggerWhen.Manual, "scale_text_in");
        if (ScriptRefrenceSingleton.instance.dialogManager.TextIsAnimating)
        {
            textEffect.StopManualEffects();
            ScriptRefrenceSingleton.instance.dialogManager.SetTextIsAnimating(false);
            return;
        }
        if (!ScriptRefrenceSingleton.instance.gameplayUtils.OpenDialogMenu()) return;
        if(providedDialog != null)
            print(providedDialog.name);
        bool nextDialogResult = GetNextDialog(providedDialog);
        if (nextDialogResult)
        {
            ShowDialog();
            ResetTMPSubMeshes(ScriptRefrenceSingleton.instance.dialogManager.text_box);
            ScriptRefrenceSingleton.instance.dialogManager.text_box.ForceMeshUpdate();
            //textEffect.StopAllEffects();
            textEffect.Refresh();
            textEffect.StartManualEffects();
            ScriptRefrenceSingleton.instance.dialogManager.SetTextIsAnimating(true);
            interactCooldown = Time.time + 0.05f;
        }

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

            if (dialogueSO.DialogueType == DS.Enumerations.DSDialogueType.MultipleChoice && ScriptRefrenceSingleton.instance.dialogManager.ActiveChoices.Count > 0 && ScriptRefrenceSingleton.instance.uIInputHandler.currentScheme == "Keyboard&Mouse")
            {
                tempDialogSO = ScriptRefrenceSingleton.instance.dialogManager.ActiveChoices[0];
                ScriptRefrenceSingleton.instance.dialogManager.ActiveChoices = new List<ScriptableObject>();
            }
            else if (dialogueSO.Choices.Count <= 1)
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
                            ScriptRefrenceSingleton.instance.dialogManager.Setup_Choices(dialogueSO.Choices, this, UseLocalization);
                        }
                        currentDialogSO = dialogueSO;
                        breakLoop = true;
                        break;
                    case DS.Enumerations.DSDialogueType.SingleChoice:
                        if (ScriptRefrenceSingleton.instance.dialogManager.get_active_choices() > 0)
                        {
                            ScriptRefrenceSingleton.instance.dialogManager.Clear_choices();
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
                    if (ScriptRefrenceSingleton.instance.gameplayUtils.get_item_holding_amount(itemRequirementSO.ItemID) >= int.Parse(itemRequirementSO.ItemAmount))
                    {
                        item_output_check = "IsTrue";
                        if (itemRequirementSO.RemoveItems)
                        {
                            ScriptRefrenceSingleton.instance.gameplayUtils.remove_items_from_inventory(itemRequirementSO.ItemID, int.Parse(itemRequirementSO.ItemAmount));
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
                    ItemData item = ScriptRefrenceSingleton.instance.gameplayUtils.GetItemDataByID(giveItemSO.ItemID);
                    //ScriptRefrenceSingleton.instance.gameplayUtils.inventoryManager.AddItemToInventory(item, int.Parse(giveItemSO.ItemAmount));
                    ScriptRefrenceSingleton.instance.gameplayUtils.add_items_to_inventory(giveItemSO.ItemID, int.Parse(giveItemSO.ItemAmount), true);
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
                    ScriptRefrenceSingleton.instance.gameplayUtils.inventoryManager.AssignQuest(assignQuestSO.QuestID);

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
        SetIsActive(false);
        ScriptRefrenceSingleton.instance.dialogManager.Clear_choices();
        ScriptRefrenceSingleton.instance.dialogManager.DialogUI.SetActive(false);
        ScriptRefrenceSingleton.instance.gameplayUtils.CloseDialogMenu();
    }

    void ShowDialog()
    {
        if (currentDialogSO is DSDialogueSO)
        {
            string fontWeight = "<font-weight=400>";
            ScriptRefrenceSingleton.instance.dialogManager.name_text.text = fontWeight + NPC_Name;
            ScriptRefrenceSingleton.instance.dialogManager.DialogUI.SetActive(true);
            DSDialogueSO dialogSO = (DSDialogueSO)currentDialogSO;
            if (UseLocalization)
            {
                string localizedText = LocalizationManager.GetLocalizedString(Localized_Table, dialogSO.LocalizedKey);
                //string formatedText = ScriptRefrenceSingleton.instance.uIIconHandler.FormatText(localizedText);
                ScriptRefrenceSingleton.instance.dialogManager.DialogTextFormater.SetText(localizedText);
                //ScriptRefrenceSingleton.instance.dialogManager.text_box.text = formatedText;
            }
            else
            {
                //string formatedText = ScriptRefrenceSingleton.instance.uIIconHandler.FormatText(dialogSO.Text);
                ScriptRefrenceSingleton.instance.dialogManager.DialogTextFormater.SetText(dialogSO.Text);
                //ScriptRefrenceSingleton.instance.dialogManager.text_box.text = formatedText;
            }
        }
    }

    public void EnableMarker(bool enabled)
    {
        if (enabled)
        {
            ScriptRefrenceSingleton.instance.compassScript.AddNewQuestMarker(transform);
            MarkerParticle.Play();
            hasMarker = true;
        }
        else
        {
            ScriptRefrenceSingleton.instance.compassScript.RemoveQuestMarker(transform);
            MarkerParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            hasMarker = false;
        }
    }

    public void SetCurrentDialogNode(ScriptableObject scriptableObject)
    {
        currentDialogSO = scriptableObject;
    }

    public void SetCurrentNodeToStart()
    {
        currentDialogSO = StarterNode;
     }



}

