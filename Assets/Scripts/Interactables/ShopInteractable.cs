using EasyTextEffects.Editor.MyBoxCopy.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ShopInteractable : MonoBehaviour, IInteractable, ISaveable
{
    public string interact_prompt;
    public string INTERACTION_PROMPT => interact_prompt;

    [HideInInspector]
    public item_requirement[] item_requirements;
    public item_requirement[] required_items => item_requirements;

    [SerializeField] bool canInteract;
    public bool CanInteract { get => canInteract; set => canInteract = value; }
    
    [SerializeField] int unique_id;
    public int Get_Unique_ID { get => unique_id; set => unique_id = value; }

    public bool Get_Should_Save {get { return hasBeenBought; } }

    [Header("Shop Settings")]
    [SerializeField] string currency = "coin";
    [SerializeField] int price;
    [SerializeField] string item_id_to_buy;
    [SerializeField] int amount_of_item_to_recieve;
    [SerializeField] bool onlySellOnce;
    [SerializeField] TextMeshPro textMeshProObject;
    bool hasBeenBought;
    public UnityEvent onBuyEvent;
    public UnityEvent clearShopEvent;
    
    public void OnEnable()
    {
        if(textMeshProObject != null)
        {
            textMeshProObject.text = $"${price}";
        }
    }

    public bool Interact(Interactor interactor)
    {
        if(canInteract)
        {
            TryAndBuy();
            return true;
        }
        return false;
    }

    public void TryAndBuy()
    {
        if(hasBeenBought) return;
        int currency_amount = ScriptRefrenceSingleton.instance.gameplayUtils.get_item_holding_amount(currency);
        if(currency_amount >= price)
        {
            ScriptRefrenceSingleton.instance.gameplayUtils.remove_items_from_inventory(currency,price);
            if(!item_id_to_buy.IsNullOrEmpty())
            {
                ScriptRefrenceSingleton.instance.gameplayUtils.add_items_to_inventory(item_id_to_buy, amount_of_item_to_recieve);
            }

            onBuyEvent?.Invoke();

            if(onlySellOnce)
            {
                hasBeenBought = true;
                canInteract = false;
                clearShopEvent?.Invoke();
            }
        }
    }

    public void SaveLoaded(SaveFileStruct saveFileStruct)
    {
        hasBeenBought = true;
        canInteract = false;
        clearShopEvent?.Invoke();
    }
}
