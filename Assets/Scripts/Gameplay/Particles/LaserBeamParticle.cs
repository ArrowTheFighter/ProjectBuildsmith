using UnityEngine;

public class LaserBeamParticle : MonoBehaviour
{
    [SerializeField] Transform sparksParticle;
    [SerializeField] LayerMask layersToIgnore;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] Transform lookAtTarget;
    [SerializeField] float maxRotation;
    [SerializeField] Vector3 targetOffset;
    [SerializeField] float maxDistance = 100;

    void OnEnable()
    {
        Update();
    }

    // Update is called once per frame
    void Update()
    {
        if (lookAtTarget != null)
        {
            if(maxRotation <= 0)
            {
                transform.LookAt(lookAtTarget.position + targetOffset);
            }
            else
            {
                Vector3 lookDirection = (lookAtTarget.position + targetOffset - transform.position).normalized;
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection, Vector3.up);

                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, maxRotation * Time.deltaTime);
            }
        }

        if(Physics.Raycast(transform.position, transform.forward, out RaycastHit ray,maxDistance, ~layersToIgnore,QueryTriggerInteraction.Ignore))
        {
            if (!sparksParticle.gameObject.activeSelf) { sparksParticle.gameObject.SetActive(true); }
            Vector3 newPosition = transform.forward * (ray.distance - 0.75f);
            sparksParticle.position = transform.position + newPosition;
            sparksParticle.rotation = Quaternion.LookRotation(ray.normal,Vector3.up);
            lineRenderer.SetPosition(1, new Vector3(0,0,ray.distance));
        }
        else if(lookAtTarget != null)
        {
            if(sparksParticle.gameObject.activeSelf) { sparksParticle.gameObject.SetActive(false); }

            lineRenderer.SetPosition(1, new Vector3(0, 0, maxDistance));
        }
    }
}
