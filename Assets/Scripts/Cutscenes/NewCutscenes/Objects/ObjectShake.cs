using DG.Tweening;
using UnityEngine;

public class ObjectShake : MonoBehaviour
{
    public float startIntensity;
    public float endIntensity;
    public float duration;

    Vector3 startLocalPos;

    public void StartShake()
    {
        startLocalPos = transform.localPosition;
        DOVirtual.Float(startIntensity, endIntensity, duration, (context) =>
        {
            transform.localPosition = startLocalPos + Random.insideUnitSphere * context;
        });
    }
}
