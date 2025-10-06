using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public class TrailerCam : MonoBehaviour
{
    [Tooltip("Spline container that defines the path.")]
    public SplineContainer splineContainer;

    [Tooltip("Current position along the spline (0 = start, 1 = end).")]
    [Range(0f, 1f)] public float normalizedPosition = 0f;

    [Tooltip("Movement speed in normalized units per second.")]
    public float speed = 0.1f;

    [Tooltip("Optional: Reference to post-processing volume with Depth of Field.")]
    public Volume postProcessVolume;

    [Header("Depth of Field Settings")]
    public float startFocusDistance = 5f;
    public float endFocusDistance = 0.5f;

    [Header("Fade Range Along Spline (0–1)")]
    [Tooltip("Where along the spline the DoF starts to change.")]
    public float dofStartPosition = 0.5f;
    [Tooltip("Where along the spline the DoF fully reaches its end value.")]
    public float dofEndPosition = 0.8f;
    Tween rotateTween;
    [Header("Camera field of view")]
    float initalFov;
    public float startFOV;
    public float endFOV;
    public float startFOVLerp;
    public float endFOVLerp;

    public float startFOV2;
    public float endFOV2;
    public float startFOVLerp2;
    public float endFOVLerp2;
    Camera _camera;
    [Header("CameraRotationTween")]
    public float rotationStartPos;
    public Vector3 endRotation;
    Quaternion startRotation;
    public float rotationDuration;
    public Ease RotationEase;
    bool hasRotated;

    [Header("CameraRotationTween2")]
    public float rotationStartPos2;
    public Vector3 endRotation2;
    public float rotationDuration2;
    public Ease RotationEase2;
    bool hasRotated2;

    private DepthOfField dof;

    public Transform Parent;

    void Start()
    {
        if (postProcessVolume != null)
            postProcessVolume.profile.TryGet(out dof);
        _camera = GetComponent<Camera>();
        startRotation = transform.rotation;
        initalFov = _camera.fieldOfView;
    }

    void Update()
    {
        if (splineContainer == null || splineContainer.Splines.Count == 0)
            return;

        // Move along the spline at your chosen rate
        normalizedPosition += speed * Time.deltaTime;

        // Loop back to start if past the end
        //if (normalizedPosition > 1f)
        //normalizedPosition -= 1f;

        if (normalizedPosition > rotationStartPos && !hasRotated)
        {
            transform.DORotate(endRotation, rotationDuration).SetEase(RotationEase);
            hasRotated = true;
        }

        if (normalizedPosition > rotationStartPos2 && !hasRotated2)
        {
            transform.DORotate(endRotation2, rotationDuration2).SetEase(RotationEase2);
            hasRotated2 = true;
        }
        // Evaluate the position and rotation along the spline
        var spline = splineContainer.Splines[0];

        // Evaluate position and tangent rotation
        spline.Evaluate(normalizedPosition, out float3 pos, out float3 tangent, out float3 up);
        //Quaternion rot = Quaternion.LookRotation(tangent, up);

        // Apply to this transform
        transform.position = splineContainer.transform.TransformPoint(pos);
        // transform.SetPositionAndRotation(
        //     splineContainer.transform.TransformPoint(pos),
        //     splineContainer.transform.rotation * rot
        // );

        if (dof != null)
        {
            float t = RemapRange01(normalizedPosition, dofStartPosition, dofEndPosition);

            dof.focusDistance.value = Mathf.Lerp(startFocusDistance, endFocusDistance, t);


            if (normalizedPosition > startFOVLerp && normalizedPosition < endFOVLerp)
            {

                float t2 = RemapRange01(normalizedPosition, startFOVLerp, endFOVLerp);

                _camera.fieldOfView = Mathf.Lerp(startFOV, endFOV, t2);

            }
            else if (normalizedPosition > startFOVLerp2 && normalizedPosition < endFOVLerp2)
            {
                float t2 = RemapRange01(normalizedPosition, startFOVLerp2, endFOVLerp2);

                _camera.fieldOfView = Mathf.Lerp(startFOV2, endFOV2, t2);
            }
            //dof.aperture.value = Mathf.Lerp(startAperture, endAperture, t);
        }
    }

    float RemapRange01(float value, float start, float end)
    {
        return Mathf.Clamp01((value - start) / (end - start));
    }

    [ContextMenu("Reset Camera")]
    public void RestartCamera()
    {
        _camera.fieldOfView = initalFov;
        normalizedPosition = 0;
        transform.rotation = startRotation;
        hasRotated = false;
        hasRotated2 = false;
    }


    // void FixedUpdate()
    // {
    //     transform.position = Parent.transform.position;
    // }
}
