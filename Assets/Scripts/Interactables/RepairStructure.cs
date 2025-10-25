using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class RepairStructure : MonoBehaviour, IInteractable, ISaveable
{

    public string PROMPT;
    public string INTERACTION_PROMPT => PROMPT;

    public GameObject FinishedStructure;
    public GameObject HologramStructure;

    public item_requirement[] itemsRequired;
    public item_requirement[] required_items => itemsRequired;

    public bool CInteract;
    public bool CanInteract { get => CInteract; set { CInteract = value; } }


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

    

    public bool Interact(Interactor interactor)
    {
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