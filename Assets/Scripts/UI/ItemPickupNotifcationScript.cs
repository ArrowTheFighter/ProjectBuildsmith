using TMPro;
using UnityEngine;
using DG.Tweening;

public class ItemPickupNotifcationScript : MonoBehaviour
{
    CanvasGroup canvasGroup;
    [SerializeField] TextMeshProUGUI notification_text;
    string item_name;
    int cur_amount;
    Tween fadeoutTween;

    [SerializeField] AudioClip pickupNotificationSoundFX;
    

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void ShowItem(ItemData itemData,int amount = 1)
    {
        GetComponent<RectTransform>().DOScale(1.1f, 0.1f).From(1f);
        if (item_name != itemData.item_name)
        {
            cur_amount = 0;
        }
        item_name = itemData.item_name;
        cur_amount += amount;
        int inventory_amount = ScriptRefrenceSingleton.instance.gameplayUtils.get_item_holding_amount(itemData.item_id);
        string inventory_amount_text = inventory_amount.ToString();
        inventory_amount_text = (inventory_amount <= 0) ? "" : $" ({inventory_amount})";
        notification_text.text = item_name + " +" + cur_amount.ToString() + inventory_amount_text;
        if(fadeoutTween.IsActive())
        {
            fadeoutTween.Kill();
        }
        PlayNotificationSound();
        fadeoutTween = DOVirtual.Float(3,0,2,(context) => {
            canvasGroup.alpha = context;
        }).OnComplete(finished_fadeout);
    }

    public void PlayNotificationSound()
    {
        AudioCollection audioCollection = ScriptRefrenceSingleton.instance.playerAudioManager.GetAudioClipByID("Pickup");

        ScriptRefrenceSingleton.instance.soundFXManager.PlayRandomSoundCollection(ScriptRefrenceSingleton.instance.gameplayUtils.PlayerTransform, audioCollection);

        //ScriptRefrenceSingleton.instance.soundFXManager.PlaySoundFXClip(pickupNotificationSoundFX, transform, pickupNotificationSoundFXVolume, pickupNotificationSoundFXPitch);
    }

    public void ShowCustomText(string text, float duration = 4)
    {
        GetComponent<RectTransform>().DOScale(1.1f, 0.1f).From(1f);
        notification_text.text = text;
        if (fadeoutTween.IsActive())
        {
            fadeoutTween.Kill();
        }
        fadeoutTween = DOVirtual.Float(duration, 0, 2, (context) =>
        {
            canvasGroup.alpha = context;
        }).OnComplete(finished_fadeout);
    }

    void finished_fadeout()
    {
        item_name = "";
        cur_amount = 0;
    }

}
