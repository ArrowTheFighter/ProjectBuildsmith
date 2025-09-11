using TMPro;
using UnityEngine;
using DG.Tweening;

public class CoinUITracker : MonoBehaviour
{
    public TextMeshProUGUI coinsAmountText;
    public string ItemID;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameplayUtils.instance.inventoryManager.OnInventoryUpdated += UpdateText;
    }

    // Update is called once per frame
    void UpdateText()
    {
        string coin_text = "<font-weight=500>" + GameplayUtils.instance.get_item_holding_amount(ItemID).ToString("000");
        if (GameplayUtils.instance.get_item_holding_amount(ItemID) == 0)
            coin_text = "<font-weight=500>000";
            if (coinsAmountText.text != coin_text)
            {
                coinsAmountText.text = coin_text;
                coinsAmountText.GetComponent<RectTransform>().DOScale(1f, 0.1f).From(0.9f);
            }
        
    }
}
