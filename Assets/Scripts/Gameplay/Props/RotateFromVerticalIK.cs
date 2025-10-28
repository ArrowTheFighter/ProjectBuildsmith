using UnityEngine;

public class RotateFromVerticalIK : MonoBehaviour
{
    [Header("References")]
    public Transform mover;    // The object that moves up and down
    public Transform rotator;  // The object that will rotate (can be 'this' if you want)

    [Header("Settings")]
    public float rotationMultiplier = 10f;          // Degrees per unit of downward movement
    public Vector3 rotationAxis = Vector3.right;    // Axis to rotate around (X by default)

    private float startY;
    private Quaternion startRotation;

    void Start()
    {
        if (mover == null)
        {
            Debug.LogWarning("RotateFromVerticalMovement: No mover assigned!");
            enabled = false;
            return;
        }

        // Store starting Y position and starting rotation
        startY = mover.position.y;
        if (rotator == null)
            rotator = transform;
        startRotation = rotator.localRotation;
    }

    void LateUpdate()
    {
        if (mover == null) return;

        // Calculate how far the mover has moved *down* from start
        float deltaY = startY - mover.position.y; // positive when moving down

        // Compute rotation relative to the starting orientation
        Quaternion relativeRotation = Quaternion.AngleAxis(deltaY * rotationMultiplier, rotationAxis);

        // Apply relative rotation on top of the starting rotation
        rotator.localRotation = startRotation * relativeRotation;
    }
}
