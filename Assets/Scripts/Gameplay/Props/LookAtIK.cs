using UnityEngine;

public class LookAtIK : MonoBehaviour
{
    public Transform target;

    private Quaternion startRotation;
    private Vector3 startDirection;

    void Start()
    {
        if (target == null)
        {
            Debug.LogWarning("RelativeLookAt: No target assigned.");
            enabled = false;
            return;
        }

        // Save the starting rotation and the initial direction to the target
        startRotation = transform.rotation;
        startDirection = (target.position - transform.position).normalized;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Current direction to the target
        Vector3 currentDirection = (target.position - transform.position).normalized;

        // Compute how much the target direction has rotated relative to the original
        Quaternion relativeRotation = Quaternion.FromToRotation(startDirection, currentDirection);

        // Apply that relative rotation to the starting rotation
        transform.rotation = relativeRotation * startRotation;
    }
}
