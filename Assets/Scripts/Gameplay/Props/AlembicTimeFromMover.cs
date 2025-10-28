using UnityEngine;
using UnityEngine.Formats.Alembic.Importer; // make sure Alembic package is installed

public class AlembicTimeFromMover : MonoBehaviour
{
    [Header("References")]
    public Transform mover;            // Object that moves up/down
    public AlembicStreamPlayer player; // Alembic stream player to control

    [Header("Settings")]
    public float speedMultiplier = 1f; // How fast time changes per unit of movement
    public bool invert = false;        // Optional: invert the direction

    private float startY;
    private float startTime;

    void Start()
    {
        if (mover == null || player == null)
        {
            Debug.LogWarning("AlembicTimeFromMover: Missing references.");
            enabled = false;
            return;
        }

        // Save starting positions and Alembic time
        startY = mover.position.y;
        startTime = player.CurrentTime;
    }

    void LateUpdate()
    {
        // Calculate how far the mover has moved vertically from start
        float deltaY = mover.position.y - startY;
        if (invert) deltaY *= -1f;

        // Map deltaY to Alembic time
        player.CurrentTime = startTime + deltaY * speedMultiplier;
    }
}
