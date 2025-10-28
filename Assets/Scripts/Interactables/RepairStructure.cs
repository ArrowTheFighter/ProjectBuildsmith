using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
using System.Collections.Generic;
using System;
using UnityEngine.InputSystem;
using System.Collections;

public class RepairStructure : MonoBehaviour, IInteractable, ISaveable
{

    public string PROMPT;
    public string INTERACTION_PROMPT => PROMPT;

    public GameObject FinishedStructure;
    public GameObject HologramStructure;

    public item_requirement[] itemsRequired;
    item_requirement[] itemsRequiredDisplay;
    public item_requirement[] required_items => itemsRequiredDisplay;

    [SerializeField] List<Item_Check> added_items = new List<Item_Check>();

    public bool CInteract = true;
    public bool CanInteract { get => CInteract; set { CInteract = value; } }

    bool isInteracting;
    bool runningCoroutine;
    public float AddItemsDelay = 0.2f;
    float currentItemDelay;

    public bool is_repaired = false;

    public float ScaleInDuration = 0.9f;
    public Ease ScaleInEase = Ease.OutExpo;
    public Vector3 scaleInSize;
    public float ScaleOutDuration = 0.3f;
    public Ease ScaleOutEase = Ease.InOutExpo;
    public Vector3 scaleOutSize;

    public string flag_name;
    [SerializeField] UnityEvent RepairEvent;

    public int Get_Unique_ID { get => unique_id; set { unique_id = value; } }
    public int unique_id;

    public bool Get_Should_Save { get => is_repaired; }

    Interactor last_interactor;

    [Header("particles")]
    public GameObject ParticleForceField;
    public Collider ParticleKillTrigger;
    public ParticleSystem startParticle;
    public Action<Collider, GameObject,RepairStructure> OnSpawnMaterialParticle;

    void Start()
    {
        itemsRequiredDisplay = itemsRequired;
        foreach (var item_req in itemsRequired)
        {
            Item_Check item_check = new Item_Check();
            item_check.item_id = item_req.item_id;
            item_check.current_amount = 0;
            item_check.required_amount = item_req.item_amount;
            added_items.Add(item_check);
        }

        ScriptRefrenceSingleton.instance.gameplayInput.playerInput.actions["Interact"].canceled += StopInteract;
        ScriptRefrenceSingleton.instance.gameplayUtils.OnStartMoveToMainMenu += UnBindInputs;
    }

    void UnBindInputs()
    {
        ScriptRefrenceSingleton.instance.gameplayInput.playerInput.actions["Interact"].canceled -= StopInteract;
        ScriptRefrenceSingleton.instance.gameplayUtils.OnStartMoveToMainMenu -= UnBindInputs;
    }

    void LostInteractor()
    {
        if (last_interactor != null)
        {
            last_interactor.InteractorLostAllInteractions -= LostInteractor;
        }
        disableIsInteracting();
    }

    void StopInteract(InputAction.CallbackContext context)
    {

        disableIsInteracting();
    }
    
    void disableIsInteracting()
    {
        if (last_interactor != null)
        {
            if (last_interactor.TryGetComponent(out AddMaterialParticleManager component))
            {
                OnSpawnMaterialParticle -= component.SpawnParticle;
            }
        }
            if (isInteracting)
        {
            isInteracting = false;
        }
    }

    public bool Interact(Interactor interactor)
    {
        currentItemDelay = AddItemsDelay;
        last_interactor = interactor;
        last_interactor.InteractorLostAllInteractions += LostInteractor;
        isInteracting = true;
        if(interactor.TryGetComponent(out AddMaterialParticleManager component))
        {
            print("hooking up spawner");
            component.SetActiveForceField(ParticleForceField);
            OnSpawnMaterialParticle += component.SpawnParticle;
        }
        if (!runningCoroutine)
            StartCoroutine(RepairCoroutine());

        return true;
        if (!CanInteract) return false;
        foreach (item_requirement item in itemsRequired)
        {
            int current_item_amount = ScriptRefrenceSingleton.instance.gameplayUtils.get_item_holding_amount(item.item_id);
            if (current_item_amount < item.item_amount)
            {
                ScriptRefrenceSingleton.instance.gameplayUtils.ShowCustomNotifCenter("Not enough resources");
                return false;
            }
        }

        if (is_repaired) return false;
        is_repaired = true;

        foreach (item_requirement item in itemsRequired)
        {
            print($"removing {item.item_amount} {item.item_name} from the players inventory");
            ScriptRefrenceSingleton.instance.gameplayUtils.remove_items_from_inventory(item.item_id, item.item_amount);
        }

        if (flag_name != "" && flag_name != null)
        {
            FlagManager.Set_Flag(flag_name);
        }

        ScaleInStructure();
        return true;
    }

    IEnumerator RepairCoroutine()
    {
        if (runningCoroutine) yield break;
        runningCoroutine = true;
        while (isInteracting)
        {
            for (int i = 0; i < added_items.Count;)
            {
                if (added_items[i].current_amount >= added_items[i].required_amount)
                {

                    i++;
                    if (i >= added_items.Count)
                    {
                        if (is_repaired) break;
                        is_repaired = true;
                        print("we repaired it!!");
                        Invoke(nameof(ScaleInStructure), 0.75f);
                        break;
                    }
                    continue;
                }
                if (ScriptRefrenceSingleton.instance.gameplayUtils.get_item_holding_amount(added_items[i].item_id) > 0)
                {
                    itemsRequiredDisplay[i].item_amount--;
                    added_items[i].current_amount++;
                    ScriptRefrenceSingleton.instance.gameplayUtils.remove_items_from_inventory(added_items[i].item_id, 1);

                    ItemData itemData = ScriptRefrenceSingleton.instance.gameplayUtils.GetItemDataByID(added_items[i].item_id);
                    if (itemData != null && itemData.item_prefab_obj != null)
                    {

                        print("Sending spawn event");
                        OnSpawnMaterialParticle?.Invoke(ParticleKillTrigger, itemData.item_prefab_obj,this);
                    }

                    // if (particleKillOnEnterTrigger != null && ParticleKillTrigger != null)
                    // {
                        
                        
                    //         //particleKillOnEnterTrigger.LaunchParticle(itemData.item_prefab_obj, ParticleKillTrigger);
                    // }
                    currentItemDelay = Mathf.Max(currentItemDelay - 0.015f,0.075f);
                }
                break;
            }

            yield return new WaitForSeconds(currentItemDelay);
        }
        runningCoroutine = false;
    }
    
    public void SpawnStartParticle(Vector3 position, ParticleKillOnEnterTrigger particleKillOnEnterTrigger)
    {
        particleKillOnEnterTrigger.OnParticleEnter -= SpawnStartParticle;
        if (startParticle == null) return;
        print(position);
        startParticle.transform.position = position;
        startParticle.Play();   
    }


    public void ScaleInStructure()
    {
        RepairEvent?.Invoke();
        HologramStructure.SetActive(false);
        FinishedStructure.SetActive(true);
        Sequence sequence = DOTween.Sequence();
        sequence.Append(
            FinishedStructure.transform.DOScale(scaleInSize, ScaleInDuration).SetEase(ScaleInEase))
            .Append(
                FinishedStructure.transform.DOScale(scaleOutSize, ScaleOutDuration).SetEase(ScaleOutEase));
        gameObject.SetActive(false);
    }

    void LoadStructure()
    {
        is_repaired = true;
        HologramStructure.SetActive(false);
        FinishedStructure.SetActive(true);
        FinishedStructure.transform.localScale = scaleOutSize;
        gameObject.SetActive(false);
    }

    public void SaveLoaded(SaveFileStruct saveFileStruct)
    {
        LoadStructure();
    }
}

[Serializable]
class Item_Check
{
    public string item_id;
    public int required_amount;
    public int current_amount;
}