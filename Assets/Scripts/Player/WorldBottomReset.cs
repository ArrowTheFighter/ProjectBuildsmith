using UnityEngine;

public class WorldBottomReset : MonoBehaviour
{
    public float WorldBottom;

    void FixedUpdate()
    {
        if (transform.position.y < WorldBottom)
        {
            if(TryGetComponent(out PlayerCheckpointPosition playerCheckpointPosition))
            {
                playerCheckpointPosition.SetPlayerToCheckpointPosition();
            }
         }
    }
}