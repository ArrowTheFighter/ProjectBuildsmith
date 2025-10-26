using System;
using UnityEngine;
using UnityEngine.Splines;

public class SplineRail : MonoBehaviour, IMoveingPlatform
{
    [SerializeField] Transform Spline;
    [SerializeField] bool rotate_with_spline;
    SplineContainer splineContainer;
    public float speed = 2f;
    public float t = 0f;
    public bool Running;

    public event Action<Vector3> OnPlatformMove;
    public event Action OnBeforePlatformMove;
    public event Action OnAfterPlatformMove;

    void Start()
    {
        splineContainer = Spline.GetComponent<SplineContainer>();
        t += (speed / splineContainer.Spline.GetLength()) * Time.deltaTime;
        Vector3 spline_position = splineContainer.Spline.EvaluatePosition(t);
        transform.position = spline_position;
    }

    void Update()
    {
        if (!Running) return;
        if (splineContainer == null || splineContainer.Spline == null) return;

        t += (speed / splineContainer.Spline.GetLength()) * Time.deltaTime;
        if (t > 1) t = 0;
        t = Mathf.Clamp01(t);

        Vector3 spline_position = splineContainer.Spline.EvaluatePosition(t);
        Vector3 tanget = splineContainer.Spline.EvaluateTangent(t);
        if (tanget != Vector3.zero && rotate_with_spline)
        {
            transform.rotation = Quaternion.LookRotation(tanget);
        }
        Vector3 old_pos = transform.position;

        transform.position = spline_position;

        Vector3 delta_pos = transform.position - old_pos;
        OnPlatformMove?.Invoke(delta_pos);
    }

    public void SetRunning(bool is_running)
    {
        Running = is_running;
    }

    public Transform getInterfaceTransform()
    {
        throw new NotImplementedException();
    }
}
