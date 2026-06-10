using UnityEngine;
using UnityEngine.Splines;

[ExecuteAlways]
public class SplineEndCaps : MonoBehaviour
{
    [SerializeField] Mesh sphereMesh;
    [SerializeField] Material sphereMaterial;
    [Header("Cap Settings")]
    [SerializeField] GameObject m_SpherePrefab;
    [SerializeField] bool m_UpdateContinuously = true;

    [Header("Material")]
    [Tooltip("Optional material override for the caps. If null, uses the SplineExtrude MeshRenderer material.")]
    [SerializeField] Material m_CapMaterial;

    GameObject m_StartCap;
    GameObject m_EndCap;

    SplineContainer m_SplineContainer;
    SplineExtrude m_SplineExtrude;
    MeshRenderer m_ExtrudeRenderer;

    void OnEnable()
    {
        TryInitialize();
        //UpdateCaps();
    }

    void OnDisable()
    {
        Cleanup();
    }

    void Update()
    {
        if (!Application.isPlaying || m_UpdateContinuously)
            UpdateCaps();
    }

    void TryInitialize()
    {
        if (m_SplineContainer == null)
            m_SplineContainer = GetComponent<SplineContainer>();

        if (m_SplineExtrude == null)
            m_SplineExtrude = GetComponent<SplineExtrude>();

        if (m_ExtrudeRenderer == null)
            m_ExtrudeRenderer = GetComponent<MeshRenderer>();

        if (m_SplineContainer == null || m_SplineExtrude == null)
            return;

        // if (m_SpherePrefab == null)
        // {
        //     m_SpherePrefab = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //     m_SpherePrefab.hideFlags = HideFlags.HideAndDontSave;
        //     DestroyImmediate(m_SpherePrefab.GetComponent<Collider>());
        // }

        // if (m_StartCap == null)
        //     m_StartCap = Instantiate(m_SpherePrefab, transform);

        // if (m_EndCap == null)
        //     m_EndCap = Instantiate(m_SpherePrefab, transform);

        // ApplyMaterial(m_StartCap);
        // ApplyMaterial(m_EndCap);
    }

    void UpdateCaps()
    {
        if (m_SplineContainer == null || m_SplineExtrude == null)
            return;

        var spline = m_SplineContainer.Spline;
        if (spline == null)
            return;

        PlaceCap(m_StartCap, spline, 0f, m_SplineExtrude.Radius);
        PlaceCap(m_EndCap, spline, 1f, m_SplineExtrude.Radius);
    }

    void PlaceCap(GameObject cap, Spline spline, float t, float radius)
    {
        // if (cap == null)
        //     return;

        Vector3 localPos = spline.EvaluatePosition(t);
        //Vector3 localTangent = ((Vector3)spline.EvaluateTangent(t)).normalized;

        float diameter = radius * 2f;
        //cap.transform.localScale = Vector3.one * diameter;

        DrawSphere(sphereMesh, transform.TransformPoint(localPos), diameter,sphereMaterial);

        // cap.transform.position = transform.TransformPoint(localPos);
        // cap.transform.rotation = Quaternion.LookRotation(transform.TransformDirection(localTangent));

    }

    void ApplyMaterial(GameObject cap)
    {
        if (cap == null)
            return;

        var renderer = cap.GetComponent<MeshRenderer>();
        if (renderer == null)
            return;

        if (m_CapMaterial != null)
            renderer.sharedMaterial = m_CapMaterial;
        else if (m_ExtrudeRenderer != null)
            renderer.sharedMaterial = m_ExtrudeRenderer.sharedMaterial;
    }

    void Cleanup()
    {
        if (m_StartCap != null)
            DestroyImmediate(m_StartCap);

        if (m_EndCap != null)
            DestroyImmediate(m_EndCap);
    }


    public static void DrawSphere(Mesh sphereMesh, Vector3 position, float size, Material material)
    {
        if (sphereMesh == null)
        {
            sphereMesh = Resources.GetBuiltinResource<Mesh>("Sphere.fbx");
        }

        Graphics.DrawMesh(
            sphereMesh,
            Matrix4x4.TRS(
                position,
                Quaternion.identity,
                Vector3.one * size
            ),
            material,
            0
        );
    }
}
