using UnityEngine;

public class WorldBottomReset : MonoBehaviour
{
    public float WorldBottom;
    public bool tryUseNPCPosition;

    void FixedUpdate()
    {
        if (transform.position.y < WorldBottom)
        {
            if(tryUseNPCPosition && TryGetComponent(out NPCFollowTargetInput component))
        {
            if(component.target != null)
            {
                transform.position = component.target.transform.position;
                return;
            }
        }
            if(TryGetComponent(out PlayerCheckpointPosition playerCheckpointPosition))
            {
                playerCheckpointPosition.SetPlayerToCheckpointPosition();
            }
         }
    }
}