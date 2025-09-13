using UnityEngine;
using UnityEngine.Events;

public class MoveObjectTrigger : MonoBehaviour
{
    public bool OnlyForNPC;
    public bool OnlyForPlayer;
    public bool OnlyOnce;
    bool activated;

    public Transform ObjectToMove;
    public Transform NewPos;

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
            print("moving target");
            ObjectToMove.position = NewPos.position;
            activated = true;
        }
        ActivateEvent?.Invoke();
     }
}
