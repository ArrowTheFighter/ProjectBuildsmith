using System.Collections;
using UnityEngine;
using DG.Tweening;

public class SpawnablePlatform : MonoBehaviour
{
    public float platformScale;
    public float platformDuration;
    float despawnProcess;
    bool runningDelay;

    void Start()
    {
        transform.localScale = Vector3.zero;
    }

    public void SpawnPlatform()
    {
        despawnProcess = 0;
        if (!runningDelay)
        {
            runningDelay = true;
            gameObject.SetActive(true);
            transform.DOScale(platformScale, 0.25f).SetEase(Ease.InOutQuad).OnComplete(() => { StartCoroutine(platformDespawnDelay()); });
        }
    }

    IEnumerator platformDespawnDelay()
    {
        while (despawnProcess < platformDuration)
        {
            despawnProcess += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        transform.DOScale(0, 0.25f).SetEase(Ease.InOutQuad).OnComplete(() =>
        {
            runningDelay = false;
            gameObject.SetActive(false);
        });
    }
    
}
