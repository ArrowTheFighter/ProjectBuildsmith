using UnityEngine;

public class PlayerSetNPCMovingTrigger : MonoBehaviour
{
    public bool isActive;
    public NPCFollowTargetInput NPC;
    void OnTriggerEnter(Collider other)
    {
        if (!isActive) return;
        if (other.tag == "Player")
        {
            print("Setting NPC to move");
            NPC.isMoving = true;
        }
    }

}

