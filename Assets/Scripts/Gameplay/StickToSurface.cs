using UnityEngine;

public class StickToSurface : MonoBehaviour
{
    [SerializeField] Transform start_pos;
    [SerializeField] Transform direction_to_check;
    [SerializeField] Transform sticky_transform;
    [SerializeField] LayerMask layerMask;
    // Update is called once per frame
    void Update()
    {
        RaycastHit raycastHit;
        Vector3 dir = direction_to_check.position - start_pos.position;
        float distance = Vector3.Distance(start_pos.position, direction_to_check.position);
        Physics.Raycast(start_pos.position, dir, out raycastHit, distance, layerMask, QueryTriggerInteraction.Ignore);
        sticky_transform.position = raycastHit.point;
    }
}
