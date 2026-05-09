using UnityEngine;

public class TeleportObject : MonoBehaviour
{
    public GameObject ObjectToMove;
    public Transform PosToTeleportTo;
    public bool FaceTargetsRotation;

    public void Teleport()
    {
        ObjectToMove.transform.position = PosToTeleportTo.position;
        if(FaceTargetsRotation)
        {
            ObjectToMove.transform.forward = PosToTeleportTo.forward;
        }
    }
}
