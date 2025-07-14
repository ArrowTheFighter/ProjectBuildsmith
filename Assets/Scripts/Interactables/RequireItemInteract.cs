using UnityEngine;
using UnityEngine.Events;

public class RequireItemInteract : MonoBehaviour, IInteractable
{
    [SerializeField] string PROMT;
    public string INTERACTION_PROMPT => PROMT;

    [SerializeField] item_requirement[] items_required_to_set;
    [SerializeField] item_requirement[] items_required;
    public item_requirement[] required_items => items_required;

    bool CInteract;
    public bool CanInteract { get => CInteract; set { CInteract = value; } }

    [SerializeField] bool remove_items = true;
    [SerializeField] bool only_once;
    [SerializeField] string finished_prompt;
    [SerializeField] bool can_interact;
    public UnityEvent activateEvent;


    public bool Interact(Interactor interactor)
    {
        if (!can_interact) return false;
        foreach (item_requirement item in items_required)
        {
            if (GameplayUtils.instance.get_item_holding_amount(item.item_id) < item.item_amount)
            {
                return false;
            }
        }
        if (remove_items)
        {
            foreach (item_requirement item in items_required)
            {
                GameplayUtils.instance.remove_items_from_inventory(item.item_id, item.item_amount);
            }
        }
        activateEvent?.Invoke();
        if (only_once)
        {
            PROMT = finished_prompt;
            items_required = new item_requirement[0];
        }
        return true;
    }

    public void SetPrompt(string _promt)
    {
        PROMT = _promt;
    }

    public void SetRequiredItems()
    {
        items_required = items_required_to_set;
    }

    public void SetCanInteract(bool _can_interact)
    {
        can_interact = _can_interact;
     }

}
