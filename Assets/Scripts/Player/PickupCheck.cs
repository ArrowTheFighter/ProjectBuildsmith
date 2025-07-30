using UnityEngine;

public class PickupCheck : MonoBehaviour
{
    void FixedUpdate()
    {
        CapsuleCollider capsule = GetComponent<CapsuleCollider>();
        Vector3 center = capsule.center;
        float height = Mathf.Max(capsule.height / 2f - capsule.radius, 0f);
        Vector3 point1, point2;

        switch (capsule.direction)
        {
            case 0: // X axis
                point1 = transform.TransformPoint(center + Vector3.left * height);
                point2 = transform.TransformPoint(center + Vector3.right * height);
                break;
            case 1: // Y axis (default)
                point1 = transform.TransformPoint(center + Vector3.down * height);
                point2 = transform.TransformPoint(center + Vector3.up * height);
                break;
            case 2: // Z axis
                point1 = transform.TransformPoint(center + Vector3.back * height);
                point2 = transform.TransformPoint(center + Vector3.forward * height);
                break;
            default:
                point1 = point2 = transform.position;
                break;
        }

        float radius = capsule.radius;

        // Perform the overlap
        Collider[] hits = Physics.OverlapCapsule(point1, point2, radius);
        foreach (Collider collider in hits)
        {
            if (collider.TryGetComponent(out ItemPickup itemPickup))
            {
                itemPickup.Pickup();
            }
         }
    }
    // void OnTriggerStay(Collider other)
    // {
    //     if (other.TryGetComponent(out ItemPickup itemPickup))
    //     {
    //         itemPickup.Pickup();
    //     }
    // }
}
