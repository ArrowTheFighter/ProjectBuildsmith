using UnityEngine;
using DG.Tweening;
using TMPro;

public class FlashingUIText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textToFade;
    [SerializeField] private float targetFadeValue;
    [SerializeField] private float fadeLoopDuration;

    private void Start()
    {
        if(textToFade != null)
        {
            textToFade.DOFade(targetFadeValue, fadeLoopDuration)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }
    }
}