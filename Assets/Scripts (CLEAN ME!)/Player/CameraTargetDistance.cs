using UnityEngine;
using Unity.Cinemachine;

public class CameraTargetDistance : MonoBehaviour
{
    [Tooltip("The Transform you want to measure distance to.")]
    public Transform target;

    Camera cam;

    [SerializeField] float max_distance;
    [SerializeField] float min_distance;
    float lerp_value;
    [SerializeField] float close_distance_damping;
    [SerializeField] float far_distance_damping;

    /// <summary>
    /// The most recently measured distance (in world units).
    /// </summary>
    public float CurrentDistance { get; private set; }

    CinemachineRotationComposer rotationComposer;

    void Awake()
    {
        if (cam == null)
            cam = Camera.main;
        if (cam == null)
            Debug.LogWarning("CameraTargetDistance: No Camera assigned and Camera.main is null.");
    }

    void Start()
    {
        rotationComposer = GetComponent<CinemachineRotationComposer>();   
    }

    void LateUpdate()
    {
        if (target == null || cam == null)
            return;

        // Compute the straight-line distance
        CurrentDistance = Vector3.Distance(cam.transform.position, target.position);

        lerp_value = Mathf.InverseLerp(min_distance, max_distance, CurrentDistance);

        rotationComposer.Damping = Vector2.one * Mathf.Lerp(close_distance_damping, far_distance_damping, lerp_value);

    }
}
