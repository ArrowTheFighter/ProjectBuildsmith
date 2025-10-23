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

    public AudioSource EngineRunningAudioSource;


    public void CollectEssence()
    {
        collectedEssence++;
        Gauge.material.DOFloat((float)collectedEssence / (float)EssenceRequired, "_FillAmount", 0.5f).SetEase(Ease.InOutQuad);
        //Gauge.material.SetFloat("_FillAmount", (float)collectedEssence / (float)EssenceRequired);
        if (collectedEssence >= EssenceRequired)
        {
            if (!Repaired)
            {
                Invoke("EngineRepaired", 0.5f);
            }
        }
    }

    public void LoadCollectedEssence()
    {
        collectedEssence++;
        Gauge.material.DOFloat((float)collectedEssence / (float)EssenceRequired, "_FillAmount", 0.5f).SetEase(Ease.InOutQuad);
        if (collectedEssence >= EssenceRequired)
        {
            if (!Repaired)
            {
                Repaired = true;
                EngineRunningAudioSource.Play();
                if (OnRepaired != null)
                {
                    int count = OnRepaired.GetPersistentEventCount();
                    for (int i = 0; i < count; i++)
                    {
                        var target = OnRepaired.GetPersistentTarget(i) as ISkippable;
                        if (target != null)
                        {
                            target.Skip();
                        }
                        else
                        {
                            // if not ISkippable, just invoke normally
                            //point.cutsceneEvent.pointEvent.Invoke();
                        }
                    }
                }
            }
        }
    }

    void EngineRepaired()
    {
        Repaired = true;
        OnRepaired?.Invoke();
        EngineRunningAudioSource.Play();
     }
}
