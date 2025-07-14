using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class BuildBridge : MonoBehaviour, IInteractable
{

    public string PROMPT;
    public string INTERACTION_PROMPT => PROMPT;


    public GameObject BridgeObj;
    public GameObject BridgeHologram;

    public item_requirement[] itemsRequired;
    public item_requirement[] required_items => itemsRequired;

    bool CInteract;
    public bool CanInteract { get => CInteract; set { CInteract = value; } }

    //public int required_wood;
    bool finished = false;

    public Vector3 scaleInSize;
    public Vector3 scaleOutSize;

    public string flag_name;
    [SerializeField] UnityEvent RepairEvent;

    public bool Interact(Interactor interactor)
    {
        foreach (item_requirement item in itemsRequired)
        {
            int current_item_amount = GameplayUtils.instance.get_item_holding_amount(item.item_id);
            if (current_item_amount < item.item_amount)
            {
                return false;
            }
        }

        if (finished) return false;
        finished = true;

        foreach (item_requirement item in itemsRequired)
        {
            GameplayUtils.instance.remove_items_from_inventory(item.item_id, item.item_amount);
        }

        if (flag_name != "" && flag_name != null)
        {
            FlagManager.Set_Flag(flag_name);
        }

        ScaleInBridge();
        return true;
    }

    public void ScaleInBridge()
    {
        RepairEvent?.Invoke();
        Destroy(BridgeHologram);
        BridgeObj.SetActive(true);
        Sequence sequence = DOTween.Sequence();
        sequence.Append(BridgeObj.transform.DOScale(scaleInSize, 0.9f).SetEase(Ease.InOutExpo)).Append(BridgeObj.transform.DOScale(scaleOutSize, 0.3f).SetEase(Ease.InOutExpo));
        gameObject.SetActive(false);
    }

    // IEnumerator build_delay()
    // {
    //     yield return new WaitForEndOfFrame();
        
    // }
}