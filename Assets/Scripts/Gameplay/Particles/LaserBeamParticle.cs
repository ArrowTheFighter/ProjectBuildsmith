using UnityEngine;

public class LaserBeamParticle : MonoBehaviour
{
    [SerializeField] Transform sparksParticle;
    [SerializeField] LayerMask layersToIgnore;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] Transform lookAtTarget;
    [SerializeField] Vector3 targetOffset;

    void OnEnable()
    {
        Update();
    }

    // Update is called once per frame
    void Update()
    {
        if (lookAtTarget != null)
        {
            transform.LookAt(lookAtTarget.position + targetOffset);
        }

        if(Physics.Raycast(transform.position, transform.forward, out RaycastHit ray,200, ~layersToIgnore,QueryTriggerInteraction.Ignore))
        {
            Vector3 newPosition = transform.forward * (ray.distance - 0.75f);
            sparksParticle.position = transform.position + newPosition;
            sparksParticle.rotation = Quaternion.LookRotation(ray.normal,Vector3.up);
            lineRenderer.SetPosition(1, new Vector3(0,0,ray.distance));
        }
        else if(lookAtTarget != null)
        {
            sparksParticle.position = lookAtTarget.position + targetOffset; 
            sparksParticle.rotation = Quaternion.LookRotation(Vector3.up, Vector3.up);

            lineRenderer.SetPosition(1, new Vector3(0, 0, Vector3.Distance(transform.position,lookAtTarget.position + targetOffset)));
        }
    }
}
