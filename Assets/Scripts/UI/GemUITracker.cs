using TMPro;
using UnityEngine;
using DG.Tweening;

public class GemUITracker : MonoBehaviour
{
    public TextMeshProUGUI coinsAmountText;
    public string ItemID;
    public string Suffix = " / 20";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameplayUtils.instance.inventoryManager.OnInventoryUpdated += UpdateText;
        UpdateText();
    }

    // Update is called once per frame
    void UpdateText()
    {
        string coin_text = "<font-weight=500>" + GameplayUtils.instance.get_item_holding_amount(ItemID).ToString("00") + Suffix;
        if (GameplayUtils.instance.get_item_holding_amount(ItemID) == 0)
            coin_text = "<font-weight=500>00 / 20";
            if (coinsAmountText.text != coin_text)
            {
                coinsAmountText.text = coin_text;
                coinsAmountText.GetComponent<RectTransform>().DOScale(1f, 0.1f).From(0.9f);
            }
        
    }
}
