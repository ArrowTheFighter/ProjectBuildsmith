using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using System.Collections;
using Sirenix.OdinInspector;

public class DespawnObjectsAnimation : MonoBehaviour
{
    public List<GameObject> ObjectsToDespawn;

    public float delay;
    public float scaleOutDuration;
    public Ease scaleOutEase;
    [Header("Particle")]
    public GameObject scaleOutParticle;
    public float particleScale = 1;
    public Vector3 particleOffset;

    [Header("Finished Event")]
    public UnityEvent OnFinishedUnityEvent;

    public AudioCollection[] scaleOutAudio;

    [Button]
    public void StartDespawnSequence()
    {
        StartCoroutine(DespawnSequenceCoroutine());
    }

    IEnumerator DespawnSequenceCoroutine()
    {
        for (int i = 0; i < ObjectsToDespawn.Count; i++)
        {
            Instantiate(scaleOutParticle,ObjectsToDespawn[i].transform.position + particleOffset,Quaternion.identity).transform.localScale = Vector3.one * particleScale;
            ObjectsToDespawn[i].transform.DOScale(0,scaleOutDuration).SetEase(scaleOutEase);
            //.OnComplete(() => {ObjectsToDespawn[i].SetActive(false);});
            ScriptRefrenceSingleton.instance.soundFXManager.PlayAllSoundCollection(ObjectsToDespawn[i].transform, scaleOutAudio);
            yield return new WaitForSeconds(delay);
        }
        OnFinishedUnityEvent?.Invoke();
        gameObject.SetActive(false);
    }
}
