using UnityEngine;
using UnityEngine.Events;

public class MoveObjectTrigger : MonoBehaviour
{
    public bool OnlyForNPC;
    public bool OnlyForPlayer;
    public bool OnlyOnce;
    public bool RotateTarget = true;
    bool activated;

    public Transform ObjectToMove;
    public Transform NewPos;
    public Transform SavePos;
    public bool ReparentObject;

    public UnityEvent ActivateEvent;

    void OnTriggerEnter(Collider other)
    {
        if (OnlyForNPC && other.tag == "NPC")
        {
            if (OnlyOnce && activated) return;
                Activate();
        }
        else if (OnlyForPlayer && other.tag == "Player")
        {

            if (OnlyOnce && activated) return;
            Activate();
        }
        else
        {
            if (other.GetComponent<CharacterMovement>() != null)
            {
                if (OnlyOnce && activated) return;
                Activate();
            }
        }
    }

    public void Activate()
    {
        if (ObjectToMove != null && NewPos != null)
        {
            if (ReparentObject)
                ObjectToMove.SetParent(NewPos);
            else ObjectToMove.SetParent(null);
            ObjectToMove.position = NewPos.position;
            activated = true;
            if (RotateTarget)
            {
                ObjectToMove.transform.forward = NewPos.forward;
            }
        }
        ActivateEvent?.Invoke();
     }
}
