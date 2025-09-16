using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class RepairStructure : MonoBehaviour, IInteractable
{

    public string PROMPT;
    public string INTERACTION_PROMPT => PROMPT;

    public GameObject FinishedStructure;
    public GameObject HologramStructure;

    public item_requirement[] itemsRequired;
    public item_requirement[] required_items => itemsRequired;

    public bool CInteract;
    public bool CanInteract { get => CInteract; set { CInteract = value; } }

    bool finished = false;

    public Vector3 scaleInSize;
    public Vector3 scaleOutSize;

    public string flag_name;
    [SerializeField] UnityEvent RepairEvent;

    public bool Interact(Interactor interactor)
    {
        if (!CanInteract) return false;
        foreach (item_requirement item in itemsRequired)
        {
            int current_item_amount = GameplayUtils.instance.get_item_holding_amount(item.item_id);
            if (current_item_amount < item.item_amount)
            {
                GameplayUtils.instance.ShowCustomNotifCenter("Not enough resources");
                return false;
            }
        }

        if (finished) return false;
        finished = true;

        foreach (item_requirement item in itemsRequired)
        {
            print($"removing {item.item_amount} {item.item_name} from the players inventory");
            GameplayUtils.instance.remove_items_from_inventory(item.item_id, item.item_amount);
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
        Destroy(HologramStructure);
        FinishedStructure.SetActive(true);
        Sequence sequence = DOTween.Sequence();
        sequence.Append(FinishedStructure.transform.DOScale(scaleInSize, 0.9f).SetEase(Ease.OutExpo)).Append(FinishedStructure.transform.DOScale(scaleOutSize, 0.3f).SetEase(Ease.InOutExpo));
        gameObject.SetActive(false);
    }

}