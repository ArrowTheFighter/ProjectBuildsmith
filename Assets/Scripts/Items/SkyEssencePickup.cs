using System;
using UnityEngine;

public class SkyEssencePickup : MonoBehaviour
{
    public SkyEngineScript SkyEngine;
    public ParticleSystem touchedVFX;
    public bool FlyTowardsEngine;
    public Vector3 SkyEngineOffset;
    public float maxSpeed;
    float speed;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (FlyTowardsEngine) return;
            touchedVFX.Play();
            FlyTowardsEngine = true;
            if (GetComponent<FloatingItem>() != null)
            {
                GetComponent<FloatingItem>().StopAnimating();
             }
        }
    }

    void FixedUpdate()
    {
        if (FlyTowardsEngine)
        {
            Vector3 dir = SkyEngine.transform.position + SkyEngineOffset - transform.position;
            transform.position += dir.normalized * speed * Time.fixedDeltaTime;
            speed = Mathf.Lerp(speed, maxSpeed, 0.1f);
            if (Vector3.Distance(transform.position, SkyEngine.transform.position + SkyEngineOffset) < 2.5f)
            {
                SkyEngine.CollectEssence();
                Destroy(gameObject);
             }
         }
    }
}