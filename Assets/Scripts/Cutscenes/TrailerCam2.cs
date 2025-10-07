using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.Rendering;
using DG.Tweening;

public class TrailerCam2 : MonoBehaviour
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
    public float slowdownStart = 0.8f; // when slowdown begins (80% through the spline)
    public float minSpeedFactor = 0.1f; // how slow it gets at the end (10% of normal speed)

    // Update is called once per frame
    void Update()
    {
        if (splineContainer == null || splineContainer.Splines.Count == 0)
            return;

        // Define parameters
        

        // Calculate a slowdown multiplier based on position
        float slowdownFactor = 1f; // default (no slowdown)
        if (normalizedPosition >= slowdownStart)
        {
            float t = (normalizedPosition - slowdownStart) / (1f - slowdownStart);
            // Smoothly interpolate from full speed to slow
            slowdownFactor = Mathf.Lerp(1f, minSpeedFactor, t);
        }

        // Move along the spline at your chosen rate
        normalizedPosition += speed * slowdownFactor * Time.deltaTime;

        normalizedPosition = Mathf.Clamp01(normalizedPosition);


        // Evaluate the position and rotation along the spline
        var spline = splineContainer.Splines[0];

        // Evaluate position and tangent rotation
        spline.Evaluate(normalizedPosition, out float3 pos, out float3 tangent, out float3 up);
        transform.position = splineContainer.transform.TransformPoint(pos);
    }
}
