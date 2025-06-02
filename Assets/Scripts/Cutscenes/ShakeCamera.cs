using UnityEngine;
using DG.Tweening;

public class ShakeCamera : MonoBehaviour
{
    [SerializeField] Transform camera_transform;
    [SerializeField] float shake_from_intensity;
    [SerializeField] float shake_to_intensity;
    [SerializeField] float shake_length;

    public void StartShake()
    {
        DOVirtual.Float(shake_from_intensity, shake_to_intensity, shake_length, (context) =>
        {
            camera_transform.localPosition = Random.insideUnitSphere * context;
        });
    } 
}
