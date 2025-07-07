using UnityEngine;

public class PickupCheck : MonoBehaviour
{
    
    void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent(out ItemPickup itemPickup))
        {
            itemPickup.Pickup();
        }
    }
}
