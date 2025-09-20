using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FadeUIToBlack : MonoBehaviour
{
    [SerializeField] CanvasGroup blackImageGroup;
    [SerializeField] float fade_duration;

    public void Fade_To_Black()
    {
        blackImageGroup.DOFade(1, fade_duration).SetUpdate(true);
    }

    public void Fade_From_Black()
    {
        blackImageGroup.DOFade(0, fade_duration).SetUpdate(true);
    }

    public void SetToBlack()
    {
        blackImageGroup.alpha = 1;
    }
}
