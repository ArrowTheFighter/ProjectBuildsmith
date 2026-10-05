using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class GameSavedPopup : MonoBehaviour
{
    CanvasGroup canvasGroup;
    public float visible_duration;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void ShowPopup()
    {
        GetComponentInChildren<TextMeshProUGUI>().SetText("Game Saved");
        Sequence sequence = DOTween.Sequence();
        sequence.Append(canvasGroup.DOFade(1, 0.5f).SetUpdate(true))
            .AppendInterval(visible_duration).SetUpdate(true)
            .Append(canvasGroup.DOFade(0, 1f).SetUpdate(true)).SetUpdate(true);
    }

    public void ShowCantSavePopup()
    {
        GetComponentInChildren<TextMeshProUGUI>().SetText("Can't Save Here");
        Sequence sequence = DOTween.Sequence();
        sequence.Append(canvasGroup.DOFade(1, 0.5f).SetUpdate(true))
            .AppendInterval(visible_duration).SetUpdate(true)
            .Append(canvasGroup.DOFade(0, 1f).SetUpdate(true)).SetUpdate(true);
    }
}
