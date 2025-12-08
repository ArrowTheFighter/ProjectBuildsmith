using UnityEngine;

public class MapleSapBucketFill : MonoBehaviour
{
    [SerializeField] private GameObject bucketEmpty;
    [SerializeField] private GameObject bucketFilled;

    [SerializeField] private TimeManager timeManager;

    private void Start()
    {
        timeManager.OnSunrise += HandleSunrise;
    }

    private void OnDestroy()
    {
        timeManager.OnSunrise -= HandleSunrise;
    }

    void HandleSunrise()
    {
        bucketEmpty.SetActive(false);
        bucketFilled.SetActive(true);
    }
}
