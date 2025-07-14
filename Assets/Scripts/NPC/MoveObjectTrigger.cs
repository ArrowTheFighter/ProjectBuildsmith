using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class MoveObjectTrigger : MonoBehaviour
{
    public bool OnlyForNPC;
    public bool OnlyForPlayer;
    public bool OnlyOnce;
    bool activated;

    public Transform ObjectToMove;
    public Transform NewPos;

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

    void Activate()
    {
        if (ObjectToMove != null && NewPos != null)
        {
            ObjectToMove.position = NewPos.position;
            activated = true;
        }
     }
}
