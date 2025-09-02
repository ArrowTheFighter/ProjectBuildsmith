using System;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class SkyEngineScript : MonoBehaviour
{
    public int EssenceRequired;
    int collectedEssence;
    public bool Repaired;
    public UnityEvent OnRepaired;
    public MeshRenderer Gauge;


    public void CollectEssence()
    {
        collectedEssence++;
        Gauge.material.DOFloat((float)collectedEssence / (float)EssenceRequired, "_FillAmount",0.5f).SetEase(Ease.InOutQuad);
        //Gauge.material.SetFloat("_FillAmount", (float)collectedEssence / (float)EssenceRequired);
        if (collectedEssence >= EssenceRequired)
        {
            if (!Repaired)
            {
                Invoke("EngineRepaired",0.5f);
            }
        }
    }

    void EngineRepaired()
    {
        Repaired = true;
        OnRepaired?.Invoke();
     }
}
