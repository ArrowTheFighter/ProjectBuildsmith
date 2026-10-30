using UnityEngine;
using DG.Tweening;

public class ForceFieldController : MonoBehaviour
{
    Material forceFieldShader;

    void Start()
    {
        forceFieldShader = GetComponent<Renderer>().material;
    }


    public void TransitionFromNothing(float duration)
    {
        forceFieldShader.DOFloat(1,"_VerticalCut",duration).SetEase(Ease.InOutSine);
    }
}
