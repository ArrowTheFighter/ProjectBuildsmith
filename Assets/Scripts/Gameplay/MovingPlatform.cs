using System;
using DG.Tweening;
using UnityEngine;

public class MovingPlatform : MonoBehaviour, IMoveingPlatform
{
    [SerializeField] Transform start_transform;
    [SerializeField] Transform end_transform;
    Vector3 start_pos;
    Vector3 end_pos;
    [SerializeField] float duration;
    [SerializeField] Ease ease;
    float lerp_value;
    Vector3 old_pos;

    public event Action<Vector3> OnPlatformMove;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        start_pos = start_transform.position;
        end_pos = end_transform.position;
        DOVirtual.Float(0, 1, duration, (context) =>
        {
            lerp_value = context;
        }).SetLoops(-1, LoopType.Yoyo).SetEase(ease);
    }

    // Update is called once per frame
    void Update()
    {
        Move_Platform(Vector3.Lerp(start_pos, end_pos, lerp_value));
    }

    void Move_Platform(Vector3 new_pos)
    {
        old_pos = transform.position;
        transform.position = new_pos;

        Vector3 delta_pos = transform.position - old_pos;
        OnPlatformMove?.Invoke(delta_pos);
    }
}
