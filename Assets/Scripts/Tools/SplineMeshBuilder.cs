using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;

[ExecuteAlways]
public class SplineMeshBuilder : MonoBehaviour
{
    [Header("Spline")]
    public SplineContainer splineContainer;

    [Header("Segment")]
    public GameObject segmentPrefab;
    public float segmentLength = 1f;
    public Vector3 segmentRotationOffset;

    [Header("Start Cap")]
    public bool useStartCap = true;
    public GameObject startCapPrefab;
    public Vector3 startCapRotationOffset;

    [Header("End Cap")]
    public bool useEndCap = true;
    public GameObject endCapPrefab;
    public Vector3 endCapRotationOffset;

    [Header("Generation")]
    public bool regenerate;
    public bool clearChildren = true;

    void Update()
    {
        if (regenerate)
        {
            regenerate = false;
            Generate();
        }
    }

    public void Generate()
    {
        if (!splineContainer)
            return;

        if (clearChildren)
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
#if UNITY_EDITOR
                DestroyImmediate(transform.GetChild(i).gameObject);
#else
                Destroy(transform.GetChild(i).gameObject);
#endif
            }
        }

        var spline = splineContainer.Spline;
        float splineLength = spline.GetLength();
        Transform splineTransform = splineContainer.transform;

        // ----------------------
        // SEGMENTS
        // ----------------------
        if (segmentPrefab && segmentLength > 0f)
        {
            int count = Mathf.FloorToInt(splineLength / segmentLength);

            for (int i = 0; i < count; i++)
            {
                float distance = (i + 0.5f) * segmentLength;
                float t = Mathf.Clamp01(distance / splineLength);

                spline.Evaluate(t, out float3 localPos, out float3 localTangent, out float3 localUp);

                Vector3 worldPos = splineTransform.TransformPoint(localPos);
                Vector3 worldTangent = splineTransform.TransformDirection(localTangent);
                Vector3 worldUp = splineTransform.TransformDirection(localUp);

                Quaternion splineRotation = Quaternion.LookRotation(worldTangent, worldUp);
                Quaternion finalRotation = splineRotation * Quaternion.Euler(segmentRotationOffset);

                Instantiate(segmentPrefab, worldPos, finalRotation, transform);
            }
        }

        // ----------------------
        // START CAP
        // ----------------------
        if (useStartCap && startCapPrefab)
        {
            spline.Evaluate(0f, out float3 localPos, out float3 localTangent, out float3 localUp);

            Vector3 worldPos = splineTransform.TransformPoint(localPos);
            Vector3 worldTangent = splineTransform.TransformDirection(localTangent);
            Vector3 worldUp = splineTransform.TransformDirection(localUp);

            Quaternion splineRotation = Quaternion.LookRotation(worldTangent, worldUp);
            Quaternion finalRotation = splineRotation * Quaternion.Euler(startCapRotationOffset);

            Instantiate(startCapPrefab, worldPos, finalRotation, transform);
        }

        // ----------------------
        // END CAP
        // ----------------------
        if (useEndCap && endCapPrefab)
        {
            spline.Evaluate(1f, out float3 localPos, out float3 localTangent, out float3 localUp);

            Vector3 worldPos = splineTransform.TransformPoint(localPos);

            // Flip tangent so cap faces outward
            Vector3 worldTangent = -splineTransform.TransformDirection(localTangent);
            Vector3 worldUp = splineTransform.TransformDirection(localUp);

            Quaternion splineRotation = Quaternion.LookRotation(worldTangent, worldUp);
            Quaternion finalRotation = splineRotation * Quaternion.Euler(endCapRotationOffset);

            Instantiate(endCapPrefab, worldPos, finalRotation, transform);
        }
    }
}
