using UnityEngine;
using UnityEngine.Events;

public class CheckForNPCTrigger : MonoBehaviour
{
    public string NPCSearchType;

    public UnityEvent onNPCEntered;
    public UnityEvent onNPCExited;

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out NPCFollowTargetInput component))
        {
            if (component.NPCType == NPCSearchType)
            {
                onNPCEntered?.Invoke();
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out NPCFollowTargetInput component))
        {
            if (component.NPCType == NPCSearchType)
            {
                onNPCExited?.Invoke();
            }
        }
    }
}
