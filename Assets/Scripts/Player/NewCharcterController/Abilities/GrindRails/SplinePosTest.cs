using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class SplinePosTest : MonoBehaviour
{
    public SplineContainer splineContainer;
    public float sphereRadius = 2;
    

    // Update is called once per frame
    void OnDrawGizmosSelected()
    {
        if(splineContainer == null) return;
        Vector3 nearestPoint = GetNearestPointOnSpline(splineContainer, transform.position, out Spline spline, out float curvePos);
        Gizmos.DrawWireSphere(nearestPoint,sphereRadius);
    }


    Vector3 GetNearestPointOnSpline(SplineContainer splineContainer, Vector3 position, out Spline _spline, out float curvePos)
    {
        Vector3 globalNearest = Vector3.zero;
        Vector3 localPos = splineContainer.transform.InverseTransformPoint(position);
        foreach (Spline spline in splineContainer.Splines)
        {
            SplineUtility.GetNearestPoint(spline, localPos, out float3 nearest, out float normalizedCurvePos, SplineUtility.PickResolutionDefault, 2);

            globalNearest = splineContainer.transform.TransformPoint((Vector3)nearest);
            _spline = spline;
            curvePos = normalizedCurvePos;
            return globalNearest;
        }
        _spline = null;
        curvePos = 0;
        return Vector3.zero;
    }
}
