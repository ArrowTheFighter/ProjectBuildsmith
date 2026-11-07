using UnityEngine;

public class PulseScale : MonoBehaviour
{
    [SerializeField] float minScale;
    [SerializeField] float maxScale;
    [SerializeField] float duration;
    [SerializeField] AnimationCurve curve;
    

    // Update is called once per frame
    void Update()
    {
        float currentScale = Mathf.PingPong(Time.time,duration);
        float lerpValue = curve.Evaluate(currentScale / duration);

        transform.localScale = Vector3.Lerp(Vector3.one * minScale, Vector3.one * maxScale, lerpValue);
    }
}
