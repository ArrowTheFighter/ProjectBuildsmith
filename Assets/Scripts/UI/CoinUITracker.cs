using TMPro;
using UnityEngine;
using DG.Tweening;

public class CoinUITracker : MonoBehaviour
{
    public TextMeshProUGUI coinsAmountText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        string coin_text = "<font-weight=500>" +GameplayUtils.instance.get_item_holding_amount("gold_coin").ToString("000");
        if (coinsAmountText.text != coin_text)
        {
            coinsAmountText.text = coin_text;
            coinsAmountText.GetComponent<RectTransform>().DOScale(1f, 0.1f).From(0.9f);
         }
        
    }
}
