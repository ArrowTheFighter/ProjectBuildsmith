using UnityEngine;

public class PlayerEnableDisableCheck : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<IEnableDisable>(out var enableDisable))
        {
            enableDisable.EnableInterface();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<IEnableDisable>(out var enableDisable))
        {
            enableDisable.DisableInterface();
        }
    }
}
