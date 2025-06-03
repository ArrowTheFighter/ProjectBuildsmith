using UnityEngine;
using DG.Tweening;

public class SpinningCoin : MonoBehaviour
{
    public static SpinningCoin instance;
    [SerializeField] float spinSpeed = 3;
    [SerializeField] float fastSpinSpeed = 8;
    [SerializeField] float speed_up_duration = 0.5f;
    [SerializeField] Ease speed_up_ease = Ease.OutQuad;
    Tween speed_up_tween;
    float current_speed;
    float spin_lerp_value = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (instance != this)
        {
            Destroy(instance);
        }
        instance = this;
    }

    void Start()
    {
        current_speed = spinSpeed;
    }

    void Update()
    {
        transform.Rotate(0, Mathf.Lerp(spinSpeed, fastSpinSpeed, spin_lerp_value) * Time.deltaTime, 0);
        transform.localScale = new Vector3(Mathf.Lerp(1,1.1f,spin_lerp_value) , Mathf.Lerp(1,0.9f,spin_lerp_value), 1);
    }

    public void SpeedUp()
    {
        if (speed_up_tween != null && speed_up_tween.IsPlaying())
        {
            speed_up_tween.Kill();
         }
        speed_up_tween = DOVirtual.Float(1, 0, speed_up_duration, (context) =>
        {
            spin_lerp_value = context;
        }).SetEase(speed_up_ease);
    }

}
